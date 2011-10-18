using System;
using System.Linq.Expressions;
using AutoPoco.Engine;
using AutoPoco.Util;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class DependencyHelper
    {
        public static IObjectGenerator<T> Impose<T, TMember>(
            this IObjectGenerator<T> objectGenerator, 
            Expression<Func<T, TMember>> propertyExpr, 
            Func<T, TMember> valueGetter)
        {
            var generator = objectGenerator.CastTo<ObjectGenerator<T>>();
            var member = ReflectionHelper.GetMember(propertyExpr);
            generator.AddAction(new ObjectFieldSetFromValueGetterAction<T, TMember>(member, valueGetter));
            return objectGenerator;
        }
    }
}