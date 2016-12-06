namespace Sample.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IDatabaseContext context)
            :base(context)
        {
        }
    }

    public interface IUserRepository
    {
        
    }
}
