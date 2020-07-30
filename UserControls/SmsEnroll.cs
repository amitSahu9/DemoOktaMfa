using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OktaDemo.UserControls;

namespace OktaDemo.UserControls
{
    public partial class SmsEnroll : UserControl
    {
        private const string SmsEnrollErrorLabel = "smsEnrollErrorLabel";
        private Timer timer = new Timer();
        private AuthnStateController authnStateController;
        public SmsEnroll()
        {
            InitializeComponent();
            this.authnStateController = stateController;
            this.passCodeTextBox.KeyDown += new KeyEventHandler(async (sender, e) => await this.EnterKeyEvent(sender, e));

            // this.EnablePlaceholder();
            SmsFactorEnroll smsFactorEnrollObject = (SmsFactorEnroll)factorObject;
            this.sendSmsButton.Click += async (object sender, EventArgs e) =>
            {
                await this.PerformActionsOnSendClick(smsFactorEnrollObject);
            };

            this.verifyButton.Click += async (object sender, EventArgs e) =>
            {
                await this.PerformActionsOnVerifyClick(smsFactorEnrollObject);
            };
        }

        private async Task PerformActionsOnVerifyClick(SmsFactorEnroll smsFactorEnrollObject)
        {
            this.formUtils.RemoveErrorMessageLabelAndAdjustPanelLocation();
            if (this.passCodeTextBox.Text == Constants.ENTER_CODE || string.IsNullOrEmpty(this.passCodeTextBox.Text))
            {
                this.CreateLabelAndDisplayErrorMessageForSms("This field cannot be left blank.", Constants.ALERT);
            }
            else
            {
                await smsFactorEnrollObject.VerifyAsync(this.passCodeTextBox.Text);
                ExitApplication.ExitAppWithSuccessCode();
            }
        }

        public void CreateLabelAndDisplayErrorMessageForSms(string errorMessage, string errorToolTip)
        {
            Label smsErrorMessageLabel = new Label()
            {
                ForeColor = Color.Red,
                Name = SmsEnrollErrorLabel,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(this.passCodeTextBox.Location.X + 15, this.passCodeTextBox.Location.Y + this.passCodeTextBox.Size.Height + 10),
            };
            if (this.Contains(smsErrorMessageLabel) == false)
            {
                ErrorProvider errorProvider = new ErrorProvider()
                {
                    BlinkStyle = ErrorBlinkStyle.NeverBlink,
                    RightToLeft = true,
                };
                errorProvider.SetError(smsErrorMessageLabel, errorToolTip);
                this.Controls.Add(smsErrorMessageLabel);
                this.verifyButton.Location = new Point(this.verifyButton.Location.X, smsErrorMessageLabel.Location.Y + smsErrorMessageLabel.Size.Height + 10);
            }

            smsErrorMessageLabel.Text = errorMessage;
        }

        private async Task PerformActionsOnSendClick(SmsFactorEnroll smsFactorEnrollObject)
        {
            // this.formUtils.RemoveErrorMessageLabelAndAdjustPanelLocation();
            // this.EnablePlaceholder();
            if (this.sendSmsButton.Text == Constants.SEND_CODE)
            {
                await smsFactorEnrollObject.SendAsync(this.phoneExtensionTextBox.Text, this.phoneNumberTextBox.Text);
            }
            else
            {
                await smsFactorEnrollObject.Resend();
            }
        }


        private void EnrollButton_Click(object sender, EventArgs e)
        {

        }
    }
}
