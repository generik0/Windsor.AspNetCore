using System;
using System.Linq;

namespace WebApp.RikTest.Attributes
{
    [NoDefault]
    [AttributeUsage(AttributeTargets.Class)]
    public class AppSettingJson : AttributeBase
    {
        public string[] Sections { get; }

        public AppSettingJson(params string[] sections)
        {
            if (!sections?.Any() ?? true)
            {
                sections = new[] {"AppConfiguration"};
            }
            Sections = sections;
        }
    }

}