using WebApp.RikTest.Interfaces;

namespace WebApp.RikTest
{
    public class SeedInjection<T> : ISeedInjection<T> where T : IDbContext
    {
        private readonly ISeed<IDbContext> _seed;

        public SeedInjection(ISeed<IDbContext> seed)
        {
            _seed = seed;
        }

        public void Up( )
        {
        }
    }
}