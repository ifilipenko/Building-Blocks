namespace BuildingBlocks.Membership.Contract
{
    public interface IRepositoryFactory
    {
        IUserRepository CreateUserRepository();
        IRoleRepository CreateRoleRepository();
    }
}