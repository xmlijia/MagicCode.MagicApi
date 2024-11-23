using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi
{
    public class MagicContext
    {
        internal static string contextItemKey = "FILLDATA";
        public static void Fill(object obj)
        {

            if (!MagicApi.HttpContext.Items.ContainsKey(contextItemKey))
            {
                MagicApi.HttpContext.Items.Add(contextItemKey, obj);

            }
            else
            {
                MagicApi.HttpContext.Items[contextItemKey] = obj;
            }
        }
    }
}
