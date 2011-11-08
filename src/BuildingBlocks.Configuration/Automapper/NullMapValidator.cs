using System;

namespace BuildingBlocks.Configuration.Automapper
{
    class NullMapValidator : IMapValidator
    {
        public void ValidateMapForType(Type destinationType)
        {
        }
    }
}