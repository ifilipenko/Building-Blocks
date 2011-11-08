using System.ServiceModel;
using BuildingBlocks.Persistence;

namespace BuildingBlocks.Wcf.Persistence
{
    public class UnitOfWorkContext : IExtension<InstanceContext>
    {
        private UnitOfWork _uow;

        public UnitOfWork UnitOfWork 
        {
            get { return _uow; }
        }

        public static UnitOfWorkContext Current()
        {
            return OperationContext.Current
                .InstanceContext.Extensions
                .Find<UnitOfWorkContext>();
        }

        public void Attach(InstanceContext owner)
        {
            _uow = UnitOfWork.Scope();
        }

        public void Detach(InstanceContext owner)
        {
            if (_uow != null)
            {
                _uow.Dispose();
            }
        }
    }
}