namespace Sample.Data.Repositories
{
    public abstract class BaseRepository
    {
        protected IDatabaseContext Context { get; private set; }
        protected BaseRepository(IDatabaseContext context)
        {
            Context = context;
        }
    }
}
