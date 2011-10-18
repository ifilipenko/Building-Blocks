using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common;
using BuildingBlocks.Common.Utils;
using CuttingEdge.Conditions;
using NHibernate.Proxy;

namespace BuildingBlocks.Persistence
{
    public class GenericEntityEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly IEqualityComparer _comparer;

        public GenericEntityEqualityComparer(Action<PropertyValueCompareResultEventArgs> onPropertyValueComparedEvent = null)
        {
            _comparer = new GenericEntityEqualityComparer(onPropertyValueComparedEvent);
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    public class GenericEntityEqualityComparer : IEqualityComparer
    {
        private readonly Action<PropertyValueCompareResultEventArgs> _onPropertyValueComparedEvent;

        public GenericEntityEqualityComparer(Action<PropertyValueCompareResultEventArgs> onPropertyValueComparedEvent = null)
        {
            _onPropertyValueComparedEvent = onPropertyValueComparedEvent ?? NullPropertyValueComparedEventHandler;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return new InnerComparer(_onPropertyValueComparedEvent).Equal(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        private static void NullPropertyValueComparedEventHandler(PropertyValueCompareResultEventArgs eventArgs)
        {
        }

        private struct ComparePair
        {
            public object Value1 { get; set; }
            public object Value2 { get; set; }

            public bool Equals(ComparePair other)
            {
                return Equals(other.Value1, Value1) &&
                       Equals(other.Value2, Value2);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof(ComparePair)) return false;
                return Equals((ComparePair)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Value1 != null ? Value1.GetHashCode() : 0) * 397) ^
                           (Value2 != null ? Value2.GetHashCode() : 0);
                }
            }
        }

        class InnerComparer
        {
            private readonly Action<PropertyValueCompareResultEventArgs> _onPropertyValueComparedEvent;
            private readonly HashSet<ComparePair> _comparedPairs;
            private readonly List<Type> _skipPropWithTypes;
            private string _currentProperty;

            public InnerComparer(Action<PropertyValueCompareResultEventArgs> onPropertyValueComparedEvent)
            {
                _onPropertyValueComparedEvent = onPropertyValueComparedEvent;
                _comparedPairs = new HashSet<ComparePair>();
                _skipPropWithTypes = new List<Type>();
            }

            public bool Equal(object x, object y)
            {
                return EqualCore(x, y, string.Empty);
            }

            private bool EqualCore(object x, object y, string rootProperty)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                {
                    OnRefEqualsValues(x, y, rootProperty, true);
                    return true;
                }
                if (ReferenceEquals(x, y))
                {
                    OnRefEqualsValues(x, y, rootProperty, true);
                    return true;
                }

                var comparePair = new ComparePair { Value1 = x, Value2 = y };
                if (_comparedPairs.Contains(comparePair))
                    return true;
                _comparedPairs.Add(comparePair);

                FindProxy(ref x, ref y);

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null) ||
                    !GetTypeUnproxied(x.GetType()).IsAssignableFrom(GetTypeUnproxied(y.GetType())))
                {
                    OnRefEqualsValues(x, y, rootProperty, false);
                    return false;
                }

                var properties = GetTypeUnproxied(x.GetType()).GetProperties();
                var notCollectionProperties = (from property in properties
                                               where (typeof (string).IsAssignableFrom(property.PropertyType) ||
                                                      !typeof (IEnumerable).IsAssignableFrom(property.PropertyType)) &&
                                                     PropertyFilter(property)
                                               select property).ToList();
                var collectionProperties = (from property in properties
                                           where !typeof(string).IsAssignableFrom(property.PropertyType) &&
                                                 typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                                                 PropertyFilter(property)
                                            select property).ToList();

                var comparer = EntityComparer(rootProperty);
                var collectionItemComparer = CreateCollectionComparer(rootProperty);
                return PropertiesValuesAreEquals(notCollectionProperties, x, y, rootProperty, comparer) &&
                       PropertiesValuesAreEquals(collectionProperties, x, y, rootProperty, collectionItemComparer);
            }

            private void FindProxy(ref object first, ref object second)
            {
                var isFirstProxy = typeof(INHibernateProxy).IsAssignableFrom(first.GetType());
                var isSecondProxy = typeof(INHibernateProxy).IsAssignableFrom(second.GetType());

                if (!isFirstProxy && isSecondProxy)
                {
                    var buff = second;
                    second = first;
                    first = buff;
                }
            }

            private bool PropertyFilter(PropertyInfo property)
            {
                return property.CanRead &&
                       !_skipPropWithTypes.Contains(property.PropertyType) &&
                       !EntityTypeDecriptor.IsPropertyOfEntityType(property);
            }

            private bool PropertiesValuesAreEquals(IEnumerable<PropertyInfo> properties, object x, object y, string rootProperty, IEqualityComparer<object> equalityComparer)
            {
                foreach (var property in properties)
                {
                    _currentProperty = property.Name;

                    var xValue = property.GetValue(x, null);
                    var yValue = property.GetValue(y, null);


                    var result = equalityComparer.Equals(xValue, yValue);
                    
                    var currentPropertyPath = string.IsNullOrEmpty(rootProperty)
                                                  ? property.Name
                                                  : rootProperty + "." + property.Name;
                    _onPropertyValueComparedEvent(new PropertyValueCompareResultEventArgs
                    {
                        PropertyPath = currentPropertyPath ?? string.Empty,
                        ValueFromSource = xValue,
                        ValueFromOther = yValue,
                        Result = result
                    });

                    if (result)
                        continue;
                    return false;
                }

                return true;
            }

            private bool CompareValues(object arg1, object arg2, string rootProperty)
            {
                var dateTimeComparer = new PersistenceDateTimeComparer();
                bool result;

                if (arg1 == null && arg2 == null)
                {
                    result = true;
                }
                else if ((arg1 != null && arg2 == null) || (arg1 == null))
                {
                    result = false;
                }
                else if (arg1 is DateTime && arg2 is DateTime)
                {
                    result = dateTimeComparer.Equals((DateTime)arg1, (DateTime)arg2);
                }
                else if (arg1 is MicDateTime && arg2 is MicDateTime)
                {
                    result = dateTimeComparer.Equals((MicDateTime)arg1, (MicDateTime)arg2);
                }
                else if (arg1 is DateTime? && arg2 is DateTime?)
                {
                    result = dateTimeComparer.Equals((DateTime?)arg1, (DateTime?)arg2);
                }
                else if (Type.GetTypeCode(arg1.GetType()) == TypeCode.Object)
                {
                    var newRootPropety = string.IsNullOrEmpty(rootProperty)
                                             ? _currentProperty
                                             : rootProperty + "." + (_currentProperty ?? string.Empty);
                    return EqualCore(arg1, arg2, newRootPropety);
                }
                else
                {
                    // ReSharper disable EqualExpressionComparison
                    result = Equals(arg1, arg1);
                    // ReSharper restore EqualExpressionComparison
                }

                return result;
            }

            private void OnRefEqualsValues(object x, object y, string property, bool result)
            {
                _onPropertyValueComparedEvent(new PropertyValueCompareResultEventArgs
                {
                    PropertyPath = property,
                    ValueFromSource = x,
                    ValueFromOther = y,
                    Result = result
                });
            }

            private IEqualityComparer<object> CreateCollectionComparer(string rootProperty)
            {
                return ComparerFactory.Create<object>((o1, o2) =>
                {
                    if (ReferenceEquals(o1, o2))
                        return true;
                    if (ReferenceEquals(null, o2))
                        return false;                    

                    var collection1 = (IEnumerable) o1;
                    var collection2 = (IEnumerable) o2;

                    return collection1.OfType<object>()
                        .SequenceEqual(collection2.OfType<object>(), EntityComparer(rootProperty));
                },
                o => o.GetHashCode());
            }

            private IEqualityComparer<object> EntityComparer(string rootProperty)
            {
                return ComparerFactory
                    .Create<object>((v1, v2) =>
                                    CompareValues(v1, v2, rootProperty),
                                    o => o.GetHashCode());
            }

            private Type GetTypeUnproxied(Type type)
            {
                var isProxy = typeof(INHibernateProxy).IsAssignableFrom(type);
                if (isProxy)
                {
                    return type.BaseType;
                }
                return type;
            }
        }

        static class EntityTypeDecriptor
        {
            private static readonly Type _entityType;
            private static readonly PropertyInfo[] _entityPublicProperties;

            static EntityTypeDecriptor()
            {
                _entityType = typeof(Entity);
                _entityPublicProperties = _entityType.GetProperties();
            }

            public static bool IsEntityType(Type type)
            {
                return _entityType.IsAssignableFrom(type);
            }

            public static bool IsPropertyOfEntityType(PropertyInfo property)
            {
                Condition.Requires(property, "property").IsNotNull();

                if (IsEntityType(property.DeclaringType))
                {
                    return _entityPublicProperties
                        .Any(p => p.Name == property.Name && p.PropertyType == property.PropertyType);
                }
                return false;
            }
        }
    }
}