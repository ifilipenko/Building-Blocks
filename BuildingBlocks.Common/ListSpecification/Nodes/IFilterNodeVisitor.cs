namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public interface IFilterNodeVisitor<TResult>
    {
        TResult Visit(GroupFilterNode node);
        TResult Visit(IsNullFilterNode node);
        TResult Visit(NotFilterNode node);
        TResult Visit(PropertyValueFilterNode node);
    }
}