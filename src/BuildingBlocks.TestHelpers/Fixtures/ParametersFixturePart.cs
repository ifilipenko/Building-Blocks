using System;
using System.Collections.Generic;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class ParametersFixturePart : FixturePart
    {
        public ParametersFixturePart()
        {
            Parameters = new List<object>();
        }

        public List<object> Parameters { get; set; }

        public DateTime current_date()
        {
            return DateTime.Now;
        }
    }
}