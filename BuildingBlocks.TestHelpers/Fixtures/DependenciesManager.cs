using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using StructureMap;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class DependenciesManager
    {
        private readonly Dictionary<Type, object> _mocks;
        private readonly Container _container;

        public DependenciesManager()
        {
            _mocks = new Dictionary<Type, object>();
            _container = new Container();
        }

        public Mock<T> MockOf<T>()
            where T : class
        {
            object mock;
            if (_mocks.TryGetValue(typeof(T), out mock) && mock is Mock<T>)
            {
                return ((Mock<T>)mock);
            }

            var newMock = new Mock<T>();
            _mocks[typeof(T)] = newMock;
            _container.Configure(x => x.For<T>().Use(newMock.Object));

            return newMock;
        }

        public void SelfResolveDependency<T>(T dependency)
        {
            _container.Configure(x => x.For<T>().Use(dependency));
        }

        public T GetDependency<T>()
        {
            return _container.GetInstance<T>();
        }

        public T CreateWithDependenciesInjection<T>()
        {
            try
            {
                return _container.GetInstance<T>();
            }
            catch (StructureMapException)
            {
                AutomaticallyRegisterDependenciesAsMocks(typeof(T));
                return _container.GetInstance<T>();
            }
        }

        private void AutomaticallyRegisterDependenciesAsMocks(Type type)
        {
            var ctor = type.GetConstructors().Single();
            foreach (var parameter in ctor.GetParameters())
            {
                var ensureMockMethod = GetType().GetMethod("MockOf")
                    .MakeGenericMethod(parameter.ParameterType);
                ensureMockMethod.Invoke(this, null);
            }
        }
    }
}