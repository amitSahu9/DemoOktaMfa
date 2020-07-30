namespace OktaDemo.UserControls
{
    partial class Sms
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
            this.VerifyButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SendSmsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // VerifyButton
            // 
            this.VerifyButton.AccessibleName = "VerifyButton";
            this.VerifyButton.Location = new System.Drawing.Point(86, 138);
            this.VerifyButton.Name = "VerifyButton";
            this.VerifyButton.Size = new System.Drawing.Size(292, 34);
            this.VerifyButton.TabIndex = 5;
            this.VerifyButton.Text = "Verify";
            this.VerifyButton.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(199, 71);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(179, 26);
            this.textBox1.TabIndex = 4;
            // 
            // SendSmsButton
            // 
            this.SendSmsButton.AccessibleName = "SendSmsButton";
            this.SendSmsButton.Location = new System.Drawing.Point(86, 69);
            this.SendSmsButton.Name = "SendSmsButton";
            this.SendSmsButton.Size = new System.Drawing.Size(102, 28);
            this.SendSmsButton.TabIndex = 3;
            this.SendSmsButton.Text = "Send OTP";
            this.SendSmsButton.UseVisualStyleBackColor = true;
            // 
            // Sms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VerifyButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.SendSmsButton);
            this.Name = "Sms";
            this.Size = new System.Drawing.Size(448, 240);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button VerifyButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SendSmsButton;
    }
}
