using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystWindows
{
    public class Filter
    {
        public string AppName { get; set; }
        public string ClassName { get; set; }

        public Filter(string appName)
        {
            AppName = appName;
            ClassName = "*";
        }

        public Filter(string appName, string className)
        {
            AppName = appName;
            ClassName = className;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
