namespace WebApp.RikTest.Interfaces
{
    public interface ISeedInjection<T> where T : IDbContext
    {
        void Up();
    }
}