using System;

namespace WebApp.RikTest.Attributes
{
    [NoDefault]
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class Factory : AttributeBase
    {
    }
}
