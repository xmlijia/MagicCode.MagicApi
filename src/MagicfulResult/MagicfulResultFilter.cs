using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MagicCode.MagicApi.MagicfulResult
{
    internal class MagicfulResultFilter : IResultFilter
    {

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            bool magicFul = MagicApi.MagicfulResult;
             
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
                var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                var noMagicfulAttribute = descriptor.MethodInfo.GetCustomAttribute<NoMagicfulResultAttribute>();
                if (noMagicfulAttribute != null)
                {
                    magicFul = false;
                }

                var magicfulAttribute = descriptor.MethodInfo.GetCustomAttribute<MagicfulResultAttribute>();

                if(magicfulAttribute != null) 
                {
                    magicFul = true;
                }
            }
            else if (context.ActionDescriptor is PageActionDescriptor || context.ActionDescriptor is CompiledPageActionDescriptor)
            {
                magicFul = false;
            } 

            if (magicFul)
            {
                var result = new MagicfulResult<object>();
                if (context.Result is ObjectResult)
                {
                    var r = ((ObjectResult)context.Result).Value;
                    var resultType = typeof(MagicfulResult<>).MakeGenericType(new Type[] { r.GetType() });
                    var result2 = Activator.CreateInstance(resultType);
                    resultType.GetProperty("Data").SetValue(result2, r);
                    resultType.GetProperty("Succeeded").SetValue(result2, true);


                    if (context.HttpContext.Items.ContainsKey(MagicContext.contextItemKey))
                    {
                        resultType.GetProperty("Extras").SetValue(result2, context.HttpContext.Items[MagicContext.contextItemKey]);
                    }
                    context.Result = new JsonResult(result2);

                }
                context.HttpContext.Response.StatusCode = 200;
            }

        }
    }
}
