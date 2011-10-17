using System;

namespace BuildingBlocks.Configuration.Automapper
{
    interface IMapValidator
    {
        void ValidateMapForType(Type destinationType);
    }
}