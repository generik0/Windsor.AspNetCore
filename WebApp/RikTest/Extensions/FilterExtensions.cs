using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;

namespace WebApp.RikTest.Extensions
{
    public static class FilterExtensions
    {
        public static AssemblyFilter Net40FilterByName(Type type)
        {
            var typeNamespace = Assembly.GetEntryAssembly().GetName().Name?.Split('.').First();
            if(string.IsNullOrWhiteSpace(typeNamespace)) throw new NullReferenceException("AssemblyFilter namespace cannot be null");
            return new AssemblyFilter(".").FilterByName(an => an.Name.StartsWith(typeNamespace, true, CultureInfo.InvariantCulture));
        }
    }
}