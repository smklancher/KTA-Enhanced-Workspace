using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnhancedWorkspace
{
    public class Forms
    {
       

        public string FormUrl(string FormName, string SiteName, string CurrentUrl)
        {
            //http://localhost/TotalAgility/forms/debug/ScanModule.form
            //regex centered around "forms" to get IISSite/left-side, KTASite/right-side
            string pattern = @"(?<base>.*?)(?<iissite>\w+)\/forms\/(?:(?<ktasite>\w+)\/)?(?<formname>\w+\.form)?";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(CurrentUrl);

            string form = String.IsNullOrWhiteSpace(FormName) ? m.Groups["formname"].Value : FormName;
            form = form.EndsWith(".form", StringComparison.OrdinalIgnoreCase) ? form : $"{form}.form";  //add .form if needed

            string site = String.IsNullOrWhiteSpace(SiteName) ? m.Groups["ktasite"].Value : SiteName;
            site = String.IsNullOrWhiteSpace(site) ? String.Empty : $"{site}/";  //add slash if kta site is used

            string newurl=$"{m.Groups["base"]}{m.Groups["iissite"]}/forms/{site}{form}";
            return newurl;
        }
    }
}
