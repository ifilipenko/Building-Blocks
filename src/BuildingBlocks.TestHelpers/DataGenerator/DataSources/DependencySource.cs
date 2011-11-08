using System;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Exceptions;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class DependencySource<T> : IDatasource<T>
    {
        private readonly Type _parentType;
        private readonly Func<IObjectGenerator<T>, IObjectGenerator<T>> _dependencyGeneratorSetup;

        public DependencySource()
        {
            _dependencyGeneratorSetup = null;
        }

        public DependencySource(Type parentType, Func<IObjectGenerator<T>, IObjectGenerator<T>> dependencyGeneratorSetup)
        {
            if (typeof (T).IsEnum)
                throw new ArgumentException("Dependency type can not be enumerator");
            if (!typeof (T).IsClass)
                throw new ArgumentException("Dependency type should be class");

            _parentType = parentType;
            _dependencyGeneratorSetup = dependencyGeneratorSetup;
        }

        public object Next(IGenerationContext context)
        {
            try
            {
                var objectGenerator = context.Single<T>();
                if (_dependencyGeneratorSetup != null)
                {
                    objectGenerator = _dependencyGeneratorSetup(objectGenerator);
                }
                return objectGenerator.Get();
            }
            catch (ArgumentException ex)
            {
                var message = _parentType != null
                                     ? string.Format("Dependency [{0}] generation failed for [{1}], see inner exception for details",
                                         typeof(T), _parentType)
                                     : string.Format("Dependency [{0}] generation failed, see inner exception for details",
                                        typeof(T));
                throw new DataGeneratorException(message, ex);
            }
        }
    }
}