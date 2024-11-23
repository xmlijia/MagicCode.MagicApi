using System;
using System.Collections.Generic;
using System.Text;

namespace MagicCode.MagicApi
{
    public class MagicfulResult<T>  
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public object Extras { get; set; }

    }
}
