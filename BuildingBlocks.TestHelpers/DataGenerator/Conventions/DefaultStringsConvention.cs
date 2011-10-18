using System;
using System.Collections.Generic;
using AutoPoco.Configuration;
using AutoPoco.DataSources;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;

namespace BuildingBlocks.TestHelpers.DataGenerator.Conventions
{
    public class DefaultStringsConvention : ITypeFieldConvention, ITypePropertyConvention
    {
        private static readonly Dictionary<string, Type> _sourcesByMemberNames = new Dictionary<string, Type>
        {
            {"username", typeof (FirstNameSource)},
            {"firstname", typeof (FirstNameSource)},
            {"lastname", typeof (LastNameSource)},
            {"surname", typeof (LastNameSource)},
            {"middlename", typeof (LastNameSource)},
            {"patrname", typeof (LastNameSource)},
            {"email", typeof (EmailAddressSource)},
            {"country", typeof (CountrySource)}
        };

        public void SpecifyRequirements(ITypeMemberConventionRequirements requirements)
        {
            requirements.Type(x => x == typeof (string));
        }

        public void Apply(ITypePropertyConventionContext context)
        {
            if (context.Member.PropertyInfo.PropertyType != typeof(string))
                return;
            var memberName = context.Member.Name.TrimStart('_').ToLower();
            Type dataSourcetype;
            if (_sourcesByMemberNames.TryGetValue(memberName, out dataSourcetype))
            {
                context.SetSource(dataSourcetype);
            }
            else
            {
                context.SetSource<RandomValueSource<string>>();
            }
            //context.SetValue(string.Empty);
        }

        public void Apply(ITypeFieldConventionContext context)
        {
            if (context.Member.FieldInfo.FieldType != typeof(string))
                return;
            var memberName = context.Member.Name.TrimStart('_').ToLower();
            Type dataSourcetype;
            if (_sourcesByMemberNames.TryGetValue(memberName, out dataSourcetype))
            {
                context.SetSource(dataSourcetype);
            }
            else
            {
                context.SetSource<RandomValueSource<string>>();
            }
            //context.SetValue(string.Empty);
        }
    }
}