using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MagicfulResultAttribute : Attribute
    { 
    }
}
