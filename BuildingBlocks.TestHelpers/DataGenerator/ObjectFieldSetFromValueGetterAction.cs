using System;
using AutoPoco.Configuration;
using AutoPoco.Engine;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    class ObjectFieldSetFromValueGetterAction<T, TMember> : IObjectAction
    {
        private readonly EngineTypeMember _member;
        private readonly Func<T, TMember> _valueGetter;

        public ObjectFieldSetFromValueGetterAction(EngineTypeMember member, Func<T, TMember> valueGetter)
        {
            _member = member;
            _valueGetter = valueGetter;
        }

        public void Enact(IGenerationContext context, object target)
        {
            if (_member.IsField)
            {
                _member.CastTo<EngineTypeFieldMember>().FieldInfo.SetValue(target, _valueGetter((T)target));
            }
            else if (_member.IsProperty)
            {
                _member.CastTo<EngineTypePropertyMember>().PropertyInfo.SetValue(target, _valueGetter((T)target), null);
            }
        }
    }
}