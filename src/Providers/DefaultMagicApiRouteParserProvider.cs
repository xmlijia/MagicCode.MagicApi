using MagicCode.MagicApi.Abstractions;
using MagicCode.MagicSir.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicCode.MagicApi.Providers
{
    public class DefaultMagicApiRouteParserProvider : IMagicApiRouteParserProvider
    {
        public string Parse(string template)
        {
            return string.Join('/', template.Split('/').Select(s => s.StartsWith('{') ? s : s.FromCamelCase("-")));
        }
    }
}
