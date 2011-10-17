namespace BuildingBlocks.Common.Utils.Hierarchy
{
    /// <summary>
    /// Represents an item in a recursive projection.
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    public interface IRecursion<T>
    {
        /// <summary>
        /// Gets the recursive depth.
        /// </summary>
        /// <value>The depth.</value>
        int Depth { get; }
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        T Item { get; }
    }
}