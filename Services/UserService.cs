namespace BudjetManagement.Services
{
    public interface IUserService
    {
        int ObtainUserId();
    }
    public class UserService: IUserService
    {
        public int ObtainUserId()
        {
            return 1;
        }
    }
}
