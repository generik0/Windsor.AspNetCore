﻿using System;

namespace WebApp.RikTest.Attributes
{
    [NoDefault]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class Transient : AttributeBase
    {
    }
}
