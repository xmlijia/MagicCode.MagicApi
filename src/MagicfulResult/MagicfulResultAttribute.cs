using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi
{
    public class MagicfulResultAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (MagicApi.MagicfulResult)
            {
                var result = new MagicfulResult<object>();

                if (context.Exception != null)
                {
                    result.Succeeded = false;
                    result.Message = context.Exception.Message;
                    result.Extras = context.Exception.StackTrace;
                    result.Data = null;
                    context.Result = new JsonResult(result);

                }
                else if (context.Result is ObjectResult)
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
            else
            {
                base.OnActionExecuted(context);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

        }
    }
}
