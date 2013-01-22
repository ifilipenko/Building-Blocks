using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoPoco.Actions;
using AutoPoco.Configuration;
using AutoPoco.Engine;
using AutoPoco.Util;
using Fasterflect;

namespace BuildingBlocks.Autopoco.Helpers
{
    public static class CollectionContextHelpers
    {
        public static ICollectionContext<TPoco, TCollection> Call<TPoco, TCollection>(this ICollectionContext<TPoco, TCollection> context, Action<TPoco> action)
            where TCollection : ICollection<TPoco>
        {
            var collectionContext = ((CollectionContext<TPoco, TCollection>)context);
            var generators = (IEnumerable<IObjectGenerator<TPoco>>)collectionContext.GetFieldValue("mGenerators", Flags.InstancePrivate);
            foreach (var generator in generators.OfType<ObjectGenerator<TPoco>>())
            {
                generator.AddAction(new ObjectMethodInvokeActionAction<TPoco>(action));
            }
            return context;
        }

        public static ICollectionContext<TPoco, TCollection> Call<TPoco, TCollection, TMember>(this ICollectionContext<TPoco, TCollection> context, Func<TPoco, TMember> func)
            where TCollection : ICollection<TPoco>
        {
            var collectionContext = ((CollectionContext<TPoco, TCollection>)context);
            var generators = (IEnumerable<IObjectGenerator<TPoco>>)collectionContext.GetFieldValue("mGenerators", Flags.InstancePrivate);
            foreach (var generator in generators.OfType<ObjectGenerator<TPoco>>())
            {
                generator.AddAction(new ObjectMethodInvokeFuncAction<TPoco, TMember>(func));
            }
            return context;
        }

        public static ICollectionContext<TPoco, TCollection> SourceForPrivatePropety<TPoco, TCollection, TMember>(this ICollectionContext<TPoco, TCollection> context, Expression<Func<TPoco, TMember>> propertyExpr, IDatasource dataSource)
            where TCollection : ICollection<TPoco>
        {
            var member = ReflectionHelper.GetMember(propertyExpr);

            var collectionContext = ((CollectionContext<TPoco, TCollection>)context);
            var generators = (IEnumerable<IObjectGenerator<TPoco>>)collectionContext.GetFieldValue("mGenerators", Flags.InstancePrivate);

            foreach (var generator in generators.OfType<ObjectGenerator<TPoco>>())
            {
                generator.AddAction(new PrivateMemberSetFromSourceAction(member, dataSource));
            }
            return context;
        }

        public static ICollectionSequenceSelectionContext<TPoco, TCollection> SourceForPrivatePropety<TPoco, TCollection, TMember>(this ICollectionSequenceSelectionContext<TPoco, TCollection> context, Expression<Func<TPoco, TMember>> propertyExpr, IDatasource dataSource)
            where TCollection : ICollection<TPoco>
        {
            var member = ReflectionHelper.GetMember(propertyExpr);

            var collectionContext = (CollectionSequenceSelectionContext<TPoco, TCollection>)context;
            var generators = (IEnumerable<IObjectGenerator<TPoco>>)collectionContext.GetFieldValue("mSelected", Flags.InstancePrivate);

            foreach (var generator in generators.OfType<ObjectGenerator<TPoco>>())
            {
                generator.AddAction(new PrivateMemberSetFromSourceAction(member, dataSource));
            }
            return context;
        }

        class PrivateMemberSetFromSourceAction : IObjectAction
        {
            private readonly EngineTypeMember _member;
            private readonly IDatasource _datasource;

            public PrivateMemberSetFromSourceAction(EngineTypeMember member, IDatasource source)
            {
                _member = member;
                _datasource = source;
            }

            public void Enact(IGenerationContext context, object target)
            {
                if (_member.IsField)
                {
                    var generationContext = new GenerationContext(context.Builders, new TypeFieldGenerationContextNode((TypeGenerationContextNode)context.Node, (EngineTypeFieldMember)_member));
                    var value = _datasource.Next(generationContext);
                    ((EngineTypeFieldMember)_member).FieldInfo.Set(target, value);
                }
                else
                {
                    var generationContext = new GenerationContext(context.Builders, new TypePropertyGenerationContextNode((TypeGenerationContextNode)context.Node, (EngineTypePropertyMember)_member));
                    var value = _datasource.Next(generationContext);
                    ((EngineTypePropertyMember)_member).PropertyInfo.Set(target, value);
                }
            }
        }
    }
}