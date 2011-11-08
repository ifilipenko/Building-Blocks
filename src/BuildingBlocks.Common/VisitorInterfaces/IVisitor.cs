namespace BuildingBlocks.Common.VisitorInterfaces
{
    public interface IVisitor
    {
        void Visit(IVisitorElement element);
    }
}