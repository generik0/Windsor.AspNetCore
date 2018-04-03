using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castle.Windsor;
using WebApp.RikTest.Attributes;

#pragma warning disable 162


namespace WebApp.RikTest.Extensions
{
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    public static class TypeExtensions
    {
        public static bool IoCCommonFilter<T>(this Type type, IWindsorContainer container) where T : AttributeBase
        {
            return HasNotIoCNoDefaultAndIsNotRegistered(type, container) && HasAttribute<T>(type)
                    && HasBaseOrInterface(type);
        }

        public static bool HasAttribute<T>(Type type) where T : AttributeBase
        {
#if NET40 || NET452 || NET471
            return (bool) type.GetCustomAttributes(typeof(T), true)?.Any();
#endif
#if NETSTANDARD2 
            return type.GetTypeInfo().GetCustomAttribute<T>(true) != null;
#endif
            return false;
        }

        public static bool HasBaseOrInterface(this Type type)
        {
            return type.BaseType != null || type.GetInterfaces().Any();
        }

        public static bool HasNotIoCNoDefaultAndIsNotRegistered(this Type type, IWindsorContainer container)
        {
            return !HasAttribute<NoDefault>(type) && !container.IsRegistered(type);
        }

        public static bool HasNoIocAttributeAndIsNotRegistered(this Type type, IWindsorContainer container)
        {
            return !HasAttribute<AttributeBase>(type) && !container.IsRegistered(type);
        }

        public static bool HasNoIoCAttributes(this Type type)
        {
            return !HasAttribute<NoDefault>(type);
        }
    }
}