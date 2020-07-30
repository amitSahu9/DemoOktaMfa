namespace OktaDemo.UserControls
{
    partial class Email
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
            this.SendEmailButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.VerifyEmailButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SendEmailButton
            // 
            this.SendEmailButton.AccessibleName = "SendEmailButton";
            this.SendEmailButton.Location = new System.Drawing.Point(49, 62);
            this.SendEmailButton.Name = "SendEmailButton";
            this.SendEmailButton.Size = new System.Drawing.Size(102, 28);
            this.SendEmailButton.TabIndex = 0;
            this.SendEmailButton.Text = "Send OTP";
            this.SendEmailButton.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(162, 64);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(179, 26);
            this.textBox1.TabIndex = 1;
            // 
            // VerifyEmailButton
            // 
            this.VerifyEmailButton.AccessibleName = "VerifyEmailButton";
            this.VerifyEmailButton.Location = new System.Drawing.Point(49, 131);
            this.VerifyEmailButton.Name = "VerifyEmailButton";
            this.VerifyEmailButton.Size = new System.Drawing.Size(292, 34);
            this.VerifyEmailButton.TabIndex = 2;
            this.VerifyEmailButton.Text = "Verify";
            this.VerifyEmailButton.UseVisualStyleBackColor = true;
            // 
            // Email
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VerifyEmailButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.SendEmailButton);
            this.Name = "Email";
            this.Size = new System.Drawing.Size(375, 225);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SendEmailButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button VerifyEmailButton;
    }
}
