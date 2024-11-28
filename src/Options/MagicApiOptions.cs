using MagicCode.MagicApi.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi.Options
{
    public class MagicApiOptions
    {
        public string BaseRoute { get; set; } = "api";
        public string[] RemoveClassSuffixWords { get; set; } = new string[]
        {
            "AppService","AppServices","ApplicationService","ApplicationServices"
        };
        public Dictionary<string, string[]> VerbPreWordMapper { get; set; } = new Dictionary<string, string[]>
        {
             {"GET",new string[]{"get"} },
            {"POST",new string[]{ "post", "create", "add", "insert"} } ,
            {"PUT",new string[]{ "put", "update", "save" } },
            {"DELETE",new string[]{ "delete", "remove" } },
            {"PATCH",new string[]{ "patch" } }
        };
        public IMagicApiRouteParserProvider RouteParserProvider { get; set; }
    }
}
