using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi.MagicfulResult
{
    public class MagicfulExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (context.HttpContext.Response.StatusCode < 400 || context.HttpContext.Response.StatusCode == 404)
            {
                var result = new MagicfulResult<string>
                {
                    Message = exception.Message,
                    Succeeded = false,
                    Data = MagicApi.Envrionment == "Development"? exception.StackTrace:"",
                    StatusCode = context.HttpContext.Response.StatusCode
                };
                context.HttpContext.Response.StatusCode = 200;

                context.Result = new JsonResult(result);

            }
        }
    }
}
