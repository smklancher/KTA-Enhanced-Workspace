using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnhancedWorkspace
{
    public class Runtime
    {
        

        public string Report()
        {
            string msg=string.Empty;

            msg += $"AppSettings: \n{StaticPropertyList(typeof(Agility.Server.Common.Configuration.ApplicationSettings))}\n\n";

            msg += $"HttpContext: \n{PropertyList(HttpContext.Current)}\n\n";

            msg += $"HttpContextRequest: \n{PropertyList(HttpContext.Current.Request)}\n\n";

            msg += $"HttpContextResponse: \n{PropertyList(HttpContext.Current.Response)}\n\n";

            msg += $"HttpContextApplication: \n{PropertyList(HttpContext.Current.Application)}\n\n";

            msg += $"HttpContextServer: \n{PropertyList(HttpContext.Current.Server)}\n\n";

            return msg;
        }

        public static string PropertyList(object obj)
        {
            if (obj == null) { return string.Empty; }
            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                try
                {
                    sb.AppendLine(p.Name + ": " + p.GetValue(obj, null));
                }
                catch (Exception)
                {

                }
            }
            return sb.ToString();
        }

        public static string StaticPropertyList(Type T)
        {
            var props = T.GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                try
                {
                    sb.AppendLine(p.Name + ": " + p.GetValue(null));
                }
                catch (Exception)
                {

                }
            }
            return sb.ToString();
        }

    }
}
