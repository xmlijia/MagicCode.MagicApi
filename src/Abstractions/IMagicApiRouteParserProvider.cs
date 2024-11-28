using MagicCode.MagicSir.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicCode.MagicApi.Abstractions
{
    public interface IMagicApiRouteParserProvider
    {
        public string Parse(string template);
    }
}
