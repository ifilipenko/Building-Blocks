namespace BuildingBlocks.Common.VisitorInterfaces
{
    public interface IVisitorElement
    {
        void Accept(IVisitor visitor);
    }

    public abstract class Element : IVisitorElement
    {
        public abstract void Accept(IVisitor visitor);
    }
}