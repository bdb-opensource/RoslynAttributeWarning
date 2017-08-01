using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDemo1
{
    //Extract this Attribute to a seperate dll to use in production
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class IDEMessageAttribute : System.Attribute
    {
        public string Message;

        public IDEMessageAttribute(string message)
        {
            this.Message = message;
        }
    }
}
