using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi.Attributes
{
    public class MagicApiAttribute : Attribute
    {
        public MagicApiAttribute(string routeTemplate = "")
        {
            RouteTemplate = routeTemplate;
        }
        public string RouteTemplate { get; set; }
        public Type TemplateParserProvider { get; set; }
    }
}
