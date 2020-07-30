using Okta.Auth.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OktaDemo
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Defines the StateController
        /// </summary>
        private AuthnStateController stateController;

        /// <summary>
        /// Defines the client
        /// </summary>
        private CustomAuthnClient client;

        /// <summary>
        /// Defines the appSettingsHelper
        /// </summary>
        private IAppSettingsHelper appSettingsHelper;

        /// <summary>
        /// Defines the SecurityHelper
        /// </summary>
        private SecurityHelper securityHelper;
        public string userName;
        public string password;
        public Form1()
        {
            InitializeComponent();
            userName = UserNameTextBox.Text;
            password = PasswordTextBox.Text;

            this.securityHelper = new SecurityHelper(userName, password);
            this.appSettingsHelper = new RegistryAppSettingsHelper(this.securityHelper);
            this.client = AuthenticationClientUtils.CreateAuthenticationClient(this.appSettingsHelper);
            this.stateController = new AuthnStateController(this.client, this.appSettingsHelper, userName);
        }

        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Primary Authentication
            var authnOptions = new AuthenticateOptions()
            {
                Username = userName,
                Password = password,
                // DeviceToken = deviceToken,
                WarnBeforePasswordExpired = true,
            };
            var authenticationResponse = this.client.AuthenticateAsync(authnOptions);
           // this.stateController.ProcessAuthnResponse(authenticationResponse);
        }
    }
}
