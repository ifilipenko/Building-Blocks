using System;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class StringToBoolTransform
    {
        [StepArgumentTransformation]
        public bool StringToBool(string value)
        {
            switch (value)
            {
                case "существует":
                    return true;
                case "не существует":
                    return false;
            }
            throw new ArgumentException("Unexpected string value", "value");
        }
    }
}