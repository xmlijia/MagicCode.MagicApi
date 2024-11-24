using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi.MagicfulResult
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoMagicfulResultAttribute : ActionFilterAttribute
    {
    }
}
