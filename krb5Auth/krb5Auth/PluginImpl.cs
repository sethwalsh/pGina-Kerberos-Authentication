/**
 * krbAuth pGina authentication plugin - An authentication for the pGina system that allows for authentication against a kerberos realm.
 * This plugin requires a secondary dll to function properly.  In order to work around some of the legacy kerberos authentication functions within
 * Windows that were not adequately ported over to the C# language from C++, it was necessary for me to write a second DLL to deal with those.  
 * It proved to be more trouble than it was worth trying to get the memory correct when converting the InitializeSecurityContext() and
 * AcquireCredentialsHandle() functions to C# so I kept them in unmanaged code and simply make a call to them passing in the user name,
 * password, krbtgt, and domain.
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.DirectoryServices;

namespace krb5Plugin
{
    public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication, pGina.Shared.Interfaces.IPluginConfiguration
    {
        private log4net.ILog m_logger;
        private static dynamic m_settings;
        internal static dynamic Settings { get { return m_settings; } }
        private static readonly Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
                
        public PluginImpl()
        {
            m_logger = log4net.LogManager.GetLogger("pGina.Plugin.krb5Plugin");

            m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);

            m_settings.SetDefault("Realm", "");            
        }

        public void Configure()
        {
            pGina.Plugin.krb5Auth.Configuration myDialog = new pGina.Plugin.krb5Auth.Configuration();
            myDialog.ShowDialog();
            
            m_logger.InfoFormat("Realm after config: {0}", myDialog.realm);
        }

        public string Name
        {
            get { return "KRB5 Authentication Plugin"; }
        }
        
        public Guid Uuid
        {
            get { return m_uuid; }
        }

        public string Description
        {
            get { return "Authenticates all users through kerberos"; }
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void Starting() { }
        public void Stopping() { }
                
        /**
         * P/Invoke for the unmanaged function that will deal with the actual athentication of a user through Kerberos
         * */        
        [DllImport("authdll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int auth_user(string u, string p, string d, string t);

        
        public pGina.Shared.Types.BooleanResult AuthenticateUser(pGina.Shared.Types.SessionProperties properties)
        {
            pGina.Shared.Types.UserInformation userInfo = properties.GetTrackedSingle<pGina.Shared.Types.UserInformation>();

            // Get the Kerberos Realm we are authenticating against from the registry
            string krbRealm = m_settings.Realm;
            //m_logger.InfoFormat("Kerberos Target Realm: {0}", krbRealm);

            /**
             * Call unmanaged DLL that will deal with Microsofts AcquireCredentialHandle() and InitializeSecurityContext() calls after creating a new SEC_WIN_AUTH_IDENTITY structure
             * from the supplied user name, password, and domain.  The return result will indicate either success or various kerberos error messages.
             * */
            int r = -1;
            try
            {
                r = auth_user(userInfo.Username, userInfo.Password, krbRealm, "krbtgt/" + krbRealm.ToUpper());
            }
            catch (Exception e)
            {
                m_logger.InfoFormat("Exception: {0}", e.Message);
            }
            
            switch (r)
            {
                //
                 // The SPN kerberos target service could not be reached.  Format should be <service-name>/REALM where the service is usually krbtgt (kerberos ticket granting ticket) followed by
                 // the realm you are targeting (all capitals) such as MYREALM.UTAH.EDU
                 // 
                 // ex: krbtgt/MYREALM.UTAH.EDU
                 //
                case -2146893039:
                    return new pGina.Shared.Types.BooleanResult() { Success = false, Message = "Failed to contact authenticating kerberos authority." };
                //
                 // The user name and/or password supplied at login through pGina does not match in the kerberos realm.  
                 //
                case -2146893044:
                    return new pGina.Shared.Types.BooleanResult() { Success = false, Message = "Failed due to bad password and/or user name." };
                //
                 // The SPN for your kerberos target was incorrect. Format should be <service-name>/REALM where the service is usually krbtgt (kerberos ticket granting ticket) followed by
                 // the realm you are targeting (all capitals) such as MYREALM.UTAH.EDU
                 // 
                 // ex: krbtgt/MYREALM.UTAH.EDU  
                 //
                case -2146893053:
                    return new pGina.Shared.Types.BooleanResult() { Success = false, Message = "Failed due to bad kerberos Security Principal Name." };
                //
                 // Success
                 //
                case 0:
                    return new pGina.Shared.Types.BooleanResult() { Success = true, Message = "Success" };
                default:
                    return new pGina.Shared.Types.BooleanResult() { Success = false, Message = "Failed to authenticate due to unaccounted for kerberos error." + r };             
            }           
        }
    }
}
