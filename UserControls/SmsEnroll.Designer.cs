namespace OktaDemo.UserControls
{
    partial class SmsEnroll
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EnrollButton = new System.Windows.Forms.Button();
            this.PasscodeTextBox = new System.Windows.Forms.TextBox();
            this.SendSmsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EnrollButton
            // 
            this.EnrollButton.AccessibleName = "EnrollButton";
            this.EnrollButton.Location = new System.Drawing.Point(57, 138);
            this.EnrollButton.Name = "EnrollButton";
            this.EnrollButton.Size = new System.Drawing.Size(292, 34);
            this.EnrollButton.TabIndex = 5;
            this.EnrollButton.Text = "Enroll";
            this.EnrollButton.UseVisualStyleBackColor = true;
            this.EnrollButton.Click += new System.EventHandler(this.EnrollButton_Click);
            // 
            // PasscodeTextBox
            // 
            this.PasscodeTextBox.AccessibleName = "PasscodeTextBox";
            this.PasscodeTextBox.Location = new System.Drawing.Point(170, 71);
            this.PasscodeTextBox.Name = "PasscodeTextBox";
            this.PasscodeTextBox.Size = new System.Drawing.Size(179, 26);
            this.PasscodeTextBox.TabIndex = 4;
            // 
            // SendSmsButton
            // 
            this.SendSmsButton.AccessibleName = "SendSmsButton";
            this.SendSmsButton.Location = new System.Drawing.Point(57, 69);
            this.SendSmsButton.Name = "SendSmsButton";
            this.SendSmsButton.Size = new System.Drawing.Size(102, 28);
            this.SendSmsButton.TabIndex = 3;
            this.SendSmsButton.Text = "Send OTP";
            this.SendSmsButton.UseVisualStyleBackColor = true;
            // 
            // SmsEnroll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EnrollButton);
            this.Controls.Add(this.PasscodeTextBox);
            this.Controls.Add(this.SendSmsButton);
            this.Name = "SmsEnroll";
            this.Size = new System.Drawing.Size(406, 241);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button EnrollButton;
        private System.Windows.Forms.TextBox PasscodeTextBox;
        private System.Windows.Forms.Button SendSmsButton;
    }
}
