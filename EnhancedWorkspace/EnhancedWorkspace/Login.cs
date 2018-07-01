using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedWorkspace
{
    public class Login
    {

        public Agility.Sdk.Model.Users.Session2 TryWindowsLogin()
        {
            Agility.Sdk.Model.Users.Session2 session = new Agility.Sdk.Model.Users.Session2();
            session.IsValid = false;

            try
            {
                // create a UserService object
                TotalAgility.Sdk.UserService userService = new TotalAgility.Sdk.UserService();
                // call Logon2 and capture the result in a new Session2 object
                session = userService.LogOnWithWindowsAuthentication2(7, true);
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
            }

            return session;
        }
    }
}
