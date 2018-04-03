using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebApp.RikTest.Extensions;

namespace WebApp.RikTest
{
    public class InstallerTransient : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetEntryAssembly())
                .Where(t => t.HasNoIocAttributeAndIsNotRegistered(container)
                            && t.HasNoIoCAttributes()
                            && t.HasBaseOrInterface())
                .WithServiceAllInterfaces()
                .Configure(component =>
                {
                    component.IsFallback();
                    component.Named($"{component.Implementation.FullName}-TransientAuto");
                })
                .WithServiceAllInterfaces()
                .LifestyleTransient());
        }
    }
}