using System;
using System.Linq;
using System.Reflection;
using AutoPoco.Configuration;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class EngineConfigurationTypeBuilderHelpers
    {
        public static IEngineConfigurationTypeBuilder<T> ConstructBy<T>(this IEngineConfigurationTypeBuilder<T> typeBuilder, Func<T> factoryMethod)
        {
            var valueGetter = new Func<IGenerationContext, T>(ctx => factoryMethod());
            return typeBuilder.ConstructWith<DelegateSource<T>>(valueGetter);
        }

        public static IEngineConfigurationTypeBuilder<T> ConstructBy<T>(this IEngineConfigurationTypeBuilder<T> typeBuilder, Func<IGenerationContext, T> factoryMethod)
        {
            var valueGetter = new Func<IGenerationContext, T>(factoryMethod);
            return typeBuilder.ConstructWith<DelegateSource<T>>(valueGetter);
        }

        public static IEngineConfigurationTypeBuilder<T> ConstructByActivator<T>(this IEngineConfigurationTypeBuilder<T> typeBuilder)
        {
            return typeBuilder.ConstructWith<FallbackObjectFactory<T>>();
        }

        public static IEngineConfigurationTypeBuilder<T> ConstructByCtor<T>(this IEngineConfigurationTypeBuilder<T> typeBuilder, Func<ConstructorInfo, bool> ctorSelector)
        {
            var constructors = typeof (T).GetConstructors(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor =constructors.First(ctorSelector);
            return typeBuilder.ConstructWith<CtorSource<T>>(ctor);
        }

        public static IEngineConfigurationTypeBuilder<T> ConstructByCtor<T>(this IEngineConfigurationTypeBuilder<T> typeBuilder, Func<ConstructorInfo, bool> ctorSelector, Func<ConstructorInfo, T> factoryMethod)
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = constructors.First(ctorSelector);
            var valueGetter = new Func<IGenerationContext, T>(ctx => factoryMethod(ctor));
            return typeBuilder.ConstructWith<DelegateSource<T>>(valueGetter);
        }
    }
}