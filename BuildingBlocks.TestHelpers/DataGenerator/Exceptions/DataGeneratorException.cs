using System;

namespace BuildingBlocks.TestHelpers.DataGenerator.Exceptions
{
    public class DataGeneratorException : Exception
    {
        public DataGeneratorException(string message)
            : base(message)
        {
        }

        public DataGeneratorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}