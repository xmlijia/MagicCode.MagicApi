using MagicCode.MagicApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MagicCode.MagicApi.Providers
{
    public class MagicApiControllerFeatureProvider : ControllerFeatureProvider
    {
        //private ISelectController _selectController;

        //public MagicApiControllerFeatureProvider(ISelectController selectController)
        //{
        //    _selectController = selectController;
        //}

        protected override bool IsController(TypeInfo typeInfo)
        {
            if (typeInfo.IsInterface)
            {
                return false;
            }
            else if (typeInfo.IsGenericType)
            {
                return false;
            }
            else if (!typeInfo.IsPublic)
            {
                return false;
            }
            else if (typeInfo.IsAbstract)
            {
                return false;
            }


            if (typeof(ControllerBase).IsAssignableFrom(typeInfo.AsType()))
            {

            }
            else if (typeof(IMagicApi).IsAssignableFrom(typeInfo.AsType()))
            {

            }
            else if (typeInfo.GetCustomAttributes<MagicApiAttribute>().Any())
            {

            }
            else
            {
                return false;
            }


            return true;
        }
    }
}
