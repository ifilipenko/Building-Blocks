using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Common
{
    public class NaturalComparer : Comparer<string>
    {
        private static readonly Regex _splitRegex;

        static NaturalComparer()
        {
            _splitRegex = new Regex("([0-9]+)", RegexOptions.Compiled);
        }

        public override int Compare(string x, string y)
        {
            if (x == y)
                return 0;

            var xParts = _splitRegex.Split(CutSpaces(x));
            var yParts = _splitRegex.Split(CutSpaces(y));
            for (var i = 0; i < xParts.Length && i < yParts.Length; i++)
            {
                if (xParts[i] != yParts[i])
                {
                    return PartCompare(xParts[i], yParts[i]);
                }
            }

            return yParts.Length.CompareTo(xParts.Length);
        }

        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
            {
                return string.Compare(left, right, StringComparison.Ordinal);
            }
            if (!int.TryParse(right, out y))
            {
                return string.Compare(left, right, StringComparison.Ordinal);
            }

            return x.CompareTo(y);
        }

        private static string CutSpaces(string x)
        {
            return x.Replace(" ", "");
        }
    }
}