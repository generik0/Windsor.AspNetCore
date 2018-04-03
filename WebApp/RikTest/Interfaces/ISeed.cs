namespace WebApp.RikTest.Interfaces
{
    public interface ISeed<in T> where T : IDbContext
    {
        void Up(T db);
    }
}