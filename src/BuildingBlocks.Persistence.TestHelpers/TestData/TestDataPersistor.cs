using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BuildingBlocks.Persistence.Scope;
using CuttingEdge.Conditions;
using NHibernate;
using NHibernate.Stat;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    public class TestDataPersistor
    {
        private readonly HashSet<object> _savedObjects;
        private readonly List<string> _traceLog;

        public TestDataPersistor()
        {
            _traceLog = new List<string>();
            _savedObjects = new HashSet<object>();
        }

        public IStatistics Statistics 
        {
            get { return PersistenceEnvironment.SessionFactory.Statistics; }
        }

        public IEqualityComparer EntityComparer
        {
            get { return new GenericEntityEqualityComparer(LogPropertyValueCompareResult); }
        }

        public IEnumerable<object> SavedObjects
        {
            get { return _savedObjects; }
        }

        public IEnumerable<T> SavedObjectsOf<T>()
        {
            return _savedObjects.OfType<T>();
        }

        public TProp GetMax<T, TProp>(Expression<Func<T, TProp>> maxProperty)
            where T : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                return repository.Query<T>().Max(maxProperty);
            }
        }

        public TProp GetMin<T, TProp>(Expression<Func<T, TProp>> minProperty)
            where T : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                return repository.Query<T>().Min(minProperty);
            }
        }

        public long GetCount<T>()
            where T : class
        {
            using (UnitOfWork.Scope())
            {
                return new Repository().GetCount<T>();
            }
        }

        public long GetCount<T>(Expression<Func<T, bool>> condition)
            where T : class
        {
            using (UnitOfWork.Scope())
            {
                return new Repository().Query<T>().Count(condition);
            }
        }

        public bool Exists<T>(Expression<Func<T, bool>> condition)
            where T : class
        {
            Condition.Requires(condition, "condition").IsNotNull();

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var exists = repository.Query<T>().Where(condition).Any();
                return exists;
            }
        }

        public bool Exists<T>(T value)
            where T : class
        {
            Condition.Requires(value, "value").IsNotNull();

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var exists = repository.QueryOver<T>().Where(t => t == value).RowCount() > 0;
                return exists;
            }
        }

        public TestDataPersistor Persist(params object[] objects)
        {
            int saveCounter = 0;
            PersistCore(objects, (obj, sessionFactory) =>
            {
                WriteTrace("[Perists " + obj + "]");
                SaveObject(obj, sessionFactory);
                saveCounter++;
                WriteTrace("\t[Success perists " + obj + "]");
            });

            Console.WriteLine("Test data log: SAVED " + saveCounter + " ENTITIES");
            return this;
        }

        public TestDataPersistor PersistAssociations(params object[] objects)
        {
            PersistCore(objects, (obj, sessionFactory) =>
            {
                WriteTrace("[Perists references for " + obj + "]");
                SaveAssociations(obj, sessionFactory);
                WriteTrace("\t[Success perists references for " + obj + "]");
            });
            return this;
        }

        public void DeleteEntityData<TEntity>(Expression<Func<TEntity, bool>> condition = null)
            where TEntity : class
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                if (condition == null)
                {
                    repository.Delete("from " + typeof(TEntity));
                }
                else
                {
                    var matchedEntities = repository.Query<TEntity>().Where(condition).ToList();
                    foreach (var matchedEntity in matchedEntities)
                    {
                        repository.Delete(matchedEntity);
                    }
                }
                uow.SubmitChanges();
            }
        }

        public void DeleteAllData()
        {
            DeleteEntityData<object>();
        }

        private void PersistCore(object[] objects, Action<object, ISessionFactory> persistObject)
        {
            try
            {
                using (var uow = UnitOfWork.TransactionScope())
                {
                    var sessionFactory = SessionLocator.Get().SessionFactory;

                    foreach (var obj in objects)
                    {
                        if (obj is IEnumerable)
                        {
                            foreach (var innerObj in (IEnumerable)obj)
                            {
                                persistObject(innerObj, sessionFactory);
                            }
                        }
                        else
                        {
                            persistObject(obj, sessionFactory);
                        }
                    }

                    uow.SubmitChanges();
                }
            }
            catch (Exception)
            {
                FlushTraceLog();
                throw;
            }
        }

        private void FlushTraceLog()
        {
            Console.WriteLine("[PERISIST FAILED!]");
            foreach (var log in _traceLog)
            {
                Console.WriteLine(log);
            }
        }

        private void SaveAssociations(object obj, ISessionFactory sessionFactory, 
            HashSet<object> currentGraphObjects = null, 
            string rootPath = null)
        {
            if (currentGraphObjects == null)
            {
                currentGraphObjects = new HashSet<object>();
            }
            var objectParser = new ObjectByMappingParser(sessionFactory);
            var parsedValues = objectParser.ParseObject(obj);

            currentGraphObjects.Add(obj);

            var associations = parsedValues.Associations.Where(v => v.Value != null).ToList();
            foreach (var objectValue in associations)
            {
                if (currentGraphObjects.Contains(objectValue.Value))
                    continue;
                var propertyPath = GetPropertyPath(rootPath, objectValue);
                WriteTrace("\t -> Save reference by path [" + propertyPath + "] of [" + (objectValue.Value ?? "<null>") + "]");
                SaveObject(objectValue.Value, sessionFactory, currentGraphObjects, propertyPath);
                WriteTrace("\t [] success save reference [" + propertyPath + "]");
            }
        }

        private void SaveObject(object obj, ISessionFactory sessionFactory, HashSet<object> currentGraphObjects = null, string rootPath = null)
        {
            _savedObjects.Add(obj);

            SaveAssociations(obj, sessionFactory, currentGraphObjects, rootPath);

            var repository = new Repository();
            repository.Save(obj);
        }

        private string GetPropertyPath(string rootPath, ObjectValue objectValue)
        {
            return string.IsNullOrEmpty(rootPath) ? objectValue.Property : rootPath + "." + objectValue.Property;
        }

        private static void LogPropertyValueCompareResult(PropertyValueCompareResultEventArgs eventArgs)
        {
            var value1 = GetValueLogString(eventArgs.ValueFromSource);
            var value2 = GetValueLogString(eventArgs.ValueFromOther);

            if (eventArgs.Result)
            {
                Console.WriteLine("Compared values from property \"{0}\": {1} == {2}",
                    eventArgs.PropertyPath, value1, value2);
            }
            else
            {
                Console.WriteLine("Compared values from property \"{0}\" are not equals", eventArgs.PropertyPath);
                Console.WriteLine("\t\t{0} != {1}", value1, value2);
            }
        }

        private static string GetValueLogString(object value)
        {
            if (value == null)
                return "<null>";
            if (value is string)
                return "\"" + value + "\"";
            if (value is IEnumerable)
                return "[count = " + ((IEnumerable)value).OfType<object>().Count() + "]";
            return value.ToString();
        }

        private void WriteTrace(string message)
        {
            _traceLog.Add(message);
        }
    }
}