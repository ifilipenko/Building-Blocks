namespace BuildingBlocks.Membership
{
    public interface IForcedPasswordChangeProvider
    {
        bool ChangePassword(string username, string newpassword);
    }
}