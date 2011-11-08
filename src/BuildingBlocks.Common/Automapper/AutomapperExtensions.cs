using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using BuildingBlocks.Common.Reflection;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.Common.Automapper
{
    public static class AutomapperExtensions
    {
        public static TResult MapValue<TSource, TResult>(this TSource sourceItem)
            where TSource : struct
        {
            ValidateMapExists<TSource, TResult>();

            var mapValue = Mapper.Map<TSource, TResult>(sourceItem);
            return mapValue;
        }

        public static TResult Map<TSource, TResult>(this TSource sourceItem, Action<TResult> mapPostProcessing = null)
            where TSource : class
            where TResult : class
        {
            if (sourceItem == null)
            {
                return null;
            }
            ValidateMapExists<TSource, TResult>();

            var result = Mapper.Map<TSource, TResult>(sourceItem);
            ProcessItem(result.ToEnumerable(), mapPostProcessing);
            return result;
        }

        public static TDestinationParent MapToSubclassOf<TDestinationParent>(this object source)
        {
            var inheritedFrom = typeof (TDestinationParent).IsInterface
                                    ? (Func<TypeMap, Type, bool>) ((m, t) => m.DestinationType.IsImplementInterface(t))
                                    : ((m, t) => m.DestinationType.InheritedFrom(t));

            var sourceType = source.GetType();
            var map = Mapper.GetAllTypeMaps()
                .FirstOrDefault(m => m.SourceType == sourceType && inheritedFrom(m, typeof(TDestinationParent)));
            if (map == null)
            {
                throw new InvalidOperationException("Type [" + sourceType + "] not mapped to subclass of [" + typeof(TDestinationParent) + "]");
            }

            var options = new MappingOperationOptions();
            var context = new ResolutionContext(map, source, sourceType, map.DestinationType, options);
            var destination = ((IMappingEngineRunner) Mapper.Engine).Map(context);

            return (TDestinationParent) destination;
        }

        public static TResult MapTo<TResult>(this object sourceItem)
            where TResult : class
        {
            if (sourceItem == null)
            {
                return null;
            }

            var sourceType = sourceItem.GetType();
            var destinationType = typeof (TResult);
            ValidateMapExists(sourceType, destinationType);
            return (TResult) Mapper.Map(sourceItem, sourceType, destinationType);
        }

        public static IEnumerable<TResult> MapItems<TSource, TResult>(this IEnumerable<TSource> sourceItems, Action<TResult> mapPostProcessing = null)
            where TSource : class
            where TResult : class
        {
            ValidateMapExists<TSource, TResult>();

            var result = Mapper.Map<IEnumerable<TSource>, IEnumerable<TResult>>(sourceItems);
            ProcessItem(result, mapPostProcessing);
            return result;
        }

        public static IEnumerable<TResult> MapItemsTo<TResult>(this IEnumerable sourceItems, Action<TResult> mapPostProcessing = null)
        {
            var result = (IEnumerable<TResult>) Mapper.Map(sourceItems, sourceItems.GetType(), typeof (IEnumerable<TResult>));
            ProcessItem(result, mapPostProcessing);
            return result;
        }

        public static IEnumerable<TResult> MapToEnum<TResult>(this IEnumerable sourceItems, Action<IEnumMappingSetup<TResult>> enumMappingSetup = null)
            where TResult : struct
        {
            return sourceItems.OfType<object>().Select(e => e.MapToEnum(enumMappingSetup));
        }

        public static TResult MapToEnum<TResult>(this object sourceItem, Action<IEnumMappingSetup<TResult>> enumMappingSetup = null)
            where TResult : struct
        {
            if (!typeof(TResult).IsEnum)
                throw new InvalidOperationException("Expected enum result type");
            if (!sourceItem.GetType().IsEnum)
                throw new InvalidOperationException("Can not convert not enum to enum");

            var options = new EnumMappingOptions<TResult>();
            if (enumMappingSetup != null) 
                enumMappingSetup(options);

            var name = Enum.GetName(sourceItem.GetType(), sourceItem);

            TResult result;
            if (!options.TryMapByRules(sourceItem, out result))
            {
                if (options.ShouldBeIgnored(sourceItem))
                {
                    throw new InvalidOperationException("Source enum " + sourceItem + " ignored");
                }
                result = (TResult) Enum.Parse(typeof (TResult), name);
            }
            return result;
        }

        public static IEnumerable<TResult> TryMapToEnum<TResult>(this IEnumerable sourceItems, Action<IEnumMappingSetup<TResult>> enumMappingSetup = null) 
            where TResult : struct
        {
            // ReSharper disable PossibleInvalidOperationException
            return sourceItems.OfType<object>().Select(e => e.TryMapToEnum(enumMappingSetup)).Where(r => r.HasValue).Select(r => r.Value);
            // ReSharper restore PossibleInvalidOperationException
        }

        public static TResult? TryMapToEnum<TResult>(this object sourceItem, Action<IEnumMappingSetup<TResult>> enumMappingSetup = null)
            where TResult : struct
        {
            try
            {
                return MapToEnum(sourceItem, enumMappingSetup);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        [Conditional("DEBUG")]
        private static void ValidateMapExists<TSource, TResult>()
        {
            ValidateMapExists(typeof (TSource), typeof (TResult));
        }

        [Conditional("DEBUG")]
        private static void ValidateMapExists(Type sourceType, Type resultType)
        {
            var map = Mapper.FindTypeMapFor(sourceType, resultType);
            if (map == null)
            {
                throw new InvalidOperationException("Type [" + sourceType + "] not mapped to [" + resultType + "]");
            }
        }

        private static void ProcessItem<T>(IEnumerable<T> result, Action<T> action)
        {
            if (action == null || result == null)
                return;

            foreach (var item in result)
            {
                action(item);
            }
        }
    }
}