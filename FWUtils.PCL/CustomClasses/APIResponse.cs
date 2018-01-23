using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWUtils.PCL.CustomClasses
{
    public class APIResponse<T>
    {
        public string Message { get; set; }
        public T Result { get; set; }
        public bool Succeed { get; set; }
    }
}
