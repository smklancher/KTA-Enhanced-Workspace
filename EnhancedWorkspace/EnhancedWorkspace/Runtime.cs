using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            KtaLocalSystem.SetResolveHandler();

            msg += $"Reporting: \n{ReportingInfo()}\n\n";

            msg += $"AppSettings: \n{StaticPropertyList(typeof(Agility.Server.Common.Configuration.ApplicationSettings))}\n\n";

            msg += $"HttpContext: \n{PropertyList(HttpContext.Current)}\n\n";

            msg += $"HttpContextRequest: \n{PropertyList(HttpContext.Current.Request)}\n\n";

            msg += $"HttpContextResponse: \n{PropertyList(HttpContext.Current.Response)}\n\n";

            msg += $"HttpContextApplication: \n{PropertyList(HttpContext.Current.Application)}\n\n";

            msg += $"HttpContextServer: \n{PropertyList(HttpContext.Current.Server)}\n\n";

            return msg;
        }

        private string ReportingInfo()
        {
            string msg = string.Empty;

            var envvar = GetInstanceOfInternalType("Kofax.CEBPM.Reporting.AzureETL.Config.EnvVariableConfiguration",
                typeof(Kofax.CEBPM.Reporting.AzureETL.Tasks.AzureEtlTaskFactory));

            msg += $"ReportingSettings: \n{PropertyList(envvar)}\n\n";

            return msg;
        }

        public static object GetInstanceOfInternalType(string FullyQualifiedInternalType, Type PublicType)
        {
            var intype = GetInternalType(FullyQualifiedInternalType, PublicType);
            return Activator.CreateInstance(intype);
        }

        public static Type GetInternalType(string FullyQualifiedInternalType, Type PublicType)
        {
            var assem = PublicType.Assembly;
            var desiredtype = assem.GetType(FullyQualifiedInternalType);
            return desiredtype;
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
                catch (Exception ex)
                {
                    Debug.Print($"Error accessing instance property of {obj.GetType().ToString()}.{p.Name}: {ex.Message}");
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
                catch (Exception ex)
                {
                    Debug.Print($"Error accessing static property of {T.ToString()}.{p.Name}: {ex.Message}");
                }
            }
            return sb.ToString();
        }

    }
}
