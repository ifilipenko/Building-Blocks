using System;
using AutoPoco.Engine;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class MiddleNameSource : DatasourceBase<string>
    {
        private static readonly string[] MiddleNames = new[]
                                                           {
                                                               "Beck",
                                                               "Belle",
                                                               "Blue",
                                                               "Bree",
                                                               "Bryn",
                                                               "Chan",
                                                               "Claude",
                                                               "Day",
                                                               "Doe",
                                                               "Dot",
                                                               "Dove",
                                                               "Dream",
                                                               "Dune",
                                                               "Fay",
                                                               "Fleur",
                                                               "Flynn",
                                                               "Frost",
                                                               "Gray",
                                                               "Grove",
                                                               "Jade",
                                                               "James",
                                                               "Jaz",
                                                               "Jude",
                                                               "Kai",
                                                               "Kit",
                                                               "Lake",
                                                               "Lark",
                                                               "Lil",
                                                               "Liv",
                                                               "Maeve",
                                                               "Maize",
                                                               "Mame",
                                                               "March",
                                                               "Mauve",
                                                               "Max",
                                                               "Muse",
                                                               "Neal",
                                                               "Neve",
                                                               "Paz",
                                                               "Pearl",
                                                               "Pine",
                                                               "Plum",
                                                               "Poe",
                                                               "Rain",
                                                               "Ray",
                                                               "Reese",
                                                               "Sage",
                                                               "Scout",
                                                               "Snow",
                                                               "Teal",
                                                               "True",
                                                               "Wren"
                                                           };

        private readonly Random _random = new Random((int) DateTime.Now.Ticks);

        public override string Next(IGenerationContext context)
        {
            return MiddleNames[_random.Next(0, MiddleNames.Length)];
        }
    }
}