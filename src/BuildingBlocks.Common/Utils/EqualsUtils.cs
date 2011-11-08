namespace BuildingBlocks.Common.Utils
{
    public class EqualsUtils
    {
        public bool BothAreNotNullOrBothIsNull(object x, object y)
        {
            return (x == null && y == null) || (x != null && y != null);
        }
    }
}