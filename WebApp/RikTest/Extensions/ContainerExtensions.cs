using System;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace WebApp.RikTest.Extensions
{
    public static class ContainerExtensions
    {
        
        public static bool DoesKernelNotAlreadyContainFacility<T>(IWindsorContainer container)
        {
            return container.Kernel.GetFacilities().ToList().FirstOrDefault(x => x.GetType() == typeof(T)) == null;
        }

        public static bool IsRegistered<T>(IWindsorContainer container )
        {
            return container.IsRegistered(typeof(T));
        }
        public static bool IsRegistered(this IWindsorContainer container, Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return container.Kernel.HasComponent(type);
            }
            return false; //if no registrar, default to not registered
        }

        

        public static void RunInstallers(this IWindsorContainer container, Assembly assembly = null)
        {
            if (assembly == null)
            {
                try
                {
                    assembly = Assembly.GetEntryAssembly();
                }
                catch
                {
                    // ignored
                }
            }
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            container.Install(FromAssembly.InThisApplication(assembly));
        }
    }
}