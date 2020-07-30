using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OktaDemo
{
    public interface IAppSettingsHelper
    {

        string GetBaseUrl();

        void WriteSessionTokenInRegistry(string sessionToken);

        void WriteOfflineMFA_AttemptCount(int offlineMFA_AttemptCount, string username);

        string GetContactInfoText();

        bool GetOfflineEnrollPromptValue(string username);

    }

    public class RegistryAppSettingsHelper : IAppSettingsHelper
    {
        private readonly SecurityHelper securityHelper;

        public RegistryAppSettingsHelper(SecurityHelper securityHelper)
        {
            this.securityHelper = securityHelper;
        }

        public string GetBaseUrl()
        {
            string baseUrl = "https://tecnics-dev.oktapreview.com";
            if (baseUrl != null)
            {
                return baseUrl;
            }
            else
            {
                throw new Exception("Base Url is not set in registry.");
            }
        }

        public string GetContactInfoText()
        {
            throw new NotImplementedException();
        }

        public bool GetOfflineEnrollPromptValue(string username)
        {
            throw new NotImplementedException();
        }

        public void WriteOfflineMFA_AttemptCount(int offlineMFA_AttemptCount, string username)
        {
            throw new NotImplementedException();
        }

        public void WriteSessionTokenInRegistry(string sessionToken)
        {
            throw new NotImplementedException();
        }
    }

    public class SignOnPolicyRegistryHelper
    {
        public void AssignDeviceTokenIfNotAssigned(string username)
        {
            this.AssignDeviceToken(username);
        }

        /*
        public bool IsDeviceTokenAssigned(string username)
        {
            string registryPath = string.Format(Constants.USERS_REGISTRY_PATH_IN_SIGNON_POLICY + "\\" + username);
            Tuple<bool, object> deviceToken = RegistryUtils.FindValueAtPath(Constants.DEVICE_TOKEN, registryPath);
            return deviceToken.Item1;
        }*/

        public string GetAssignedDeviceToken(string username)
        {
            string deviceToken = CreateDeviceToken();
            return deviceToken;
        }

        public string CreateDeviceToken()
        {
            return this.CreateGuidForDeviceToken();
        }

        public string CreateGuidForDeviceToken()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString("N").ToUpper().Substring(0, 31);
        }

        public void AssignDeviceToken(string username)
        {
            string deviceToken = this.CreateDeviceToken();
            //string registryPath = string.Format(Constants.USERS_REGISTRY_PATH_IN_SIGNON_POLICY + "\\" + username);
            // RegistryUtils.WriteCredToRegistry(deviceToken, registryPath, Constants.DEVICE_TOKEN);
        }
    }
}
