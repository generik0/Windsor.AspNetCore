namespace WebApp
{
    public class Injector
    {
        public static T Locate<T>()
        {
            return Startup.Container.Resolve<T>();
        }
    }
}