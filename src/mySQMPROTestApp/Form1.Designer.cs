namespace ASCOM.mySQMPRO
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonChoose = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.StatusMsg = new System.Windows.Forms.Label();
            this.MessageTxtBox = new System.Windows.Forms.TextBox();
            this.DewPointBtn = new System.Windows.Forms.Button();
            this.HumidityBtn = new System.Windows.Forms.Button();
            this.TemperatureBtn = new System.Windows.Forms.Button();
            this.labelDriverId = new System.Windows.Forms.Label();
            this.SkyTemperatureBtn = new System.Windows.Forms.Button();
            this.LuxBtn = new System.Windows.Forms.Button();
            this.SQMBtn = new System.Windows.Forms.Button();
            this.PressureBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonChoose
            // 
            this.buttonChoose.Location = new System.Drawing.Point(412, 12);
            this.buttonChoose.Margin = new System.Windows.Forms.Padding(4);
            this.buttonChoose.Name = "buttonChoose";
            this.buttonChoose.Size = new System.Drawing.Size(96, 28);
            this.buttonChoose.TabIndex = 0;
            this.buttonChoose.Text = "Choose";
            this.buttonChoose.UseVisualStyleBackColor = true;
            this.buttonChoose.Click += new System.EventHandler(this.buttonChoose_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(412, 48);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(96, 28);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(378, 281);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(131, 42);
            this.ExitBtn.TabIndex = 14;
            this.ExitBtn.Text = "Exit";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // StatusMsg
            // 
            this.StatusMsg.AutoSize = true;
            this.StatusMsg.Location = new System.Drawing.Point(207, 222);
            this.StatusMsg.Name = "StatusMsg";
            this.StatusMsg.Size = new System.Drawing.Size(116, 17);
            this.StatusMsg.TabIndex = 13;
            this.StatusMsg.Text = "Status Messages";
            // 
            // MessageTxtBox
            // 
            this.MessageTxtBox.Location = new System.Drawing.Point(210, 242);
            this.MessageTxtBox.Name = "MessageTxtBox";
            this.MessageTxtBox.Size = new System.Drawing.Size(298, 22);
            this.MessageTxtBox.TabIndex = 12;
            // 
            // DewPointBtn
            // 
            this.DewPointBtn.Location = new System.Drawing.Point(377, 92);
            this.DewPointBtn.Name = "DewPointBtn";
            this.DewPointBtn.Size = new System.Drawing.Size(131, 42);
            this.DewPointBtn.TabIndex = 11;
            this.DewPointBtn.Text = "Dew-Point";
            this.DewPointBtn.UseVisualStyleBackColor = true;
            this.DewPointBtn.Click += new System.EventHandler(this.DewPointBtn_Click);
            // 
            // HumidityBtn
            // 
            this.HumidityBtn.Location = new System.Drawing.Point(201, 92);
            this.HumidityBtn.Name = "HumidityBtn";
            this.HumidityBtn.Size = new System.Drawing.Size(131, 42);
            this.HumidityBtn.TabIndex = 10;
            this.HumidityBtn.Text = "Humidity";
            this.HumidityBtn.UseVisualStyleBackColor = true;
            this.HumidityBtn.Click += new System.EventHandler(this.HumidityBtn_Click);
            // 
            // TemperatureBtn
            // 
            this.TemperatureBtn.Location = new System.Drawing.Point(18, 92);
            this.TemperatureBtn.Name = "TemperatureBtn";
            this.TemperatureBtn.Size = new System.Drawing.Size(131, 42);
            this.TemperatureBtn.TabIndex = 9;
            this.TemperatureBtn.Text = "Temperature";
            this.TemperatureBtn.UseVisualStyleBackColor = true;
            this.TemperatureBtn.Click += new System.EventHandler(this.TemperatureBtn_Click);
            // 
            // labelDriverId
            // 
            this.labelDriverId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDriverId.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASCOM.mySQMPRO.Properties.Settings.Default, "DriverId", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelDriverId.Location = new System.Drawing.Point(17, 15);
            this.labelDriverId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDriverId.Name = "labelDriverId";
            this.labelDriverId.Size = new System.Drawing.Size(387, 25);
            this.labelDriverId.TabIndex = 2;
            this.labelDriverId.Text = global::ASCOM.mySQMPRO.Properties.Settings.Default.DriverId;
            this.labelDriverId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SkyTemperatureBtn
            // 
            this.SkyTemperatureBtn.Location = new System.Drawing.Point(18, 156);
            this.SkyTemperatureBtn.Name = "SkyTemperatureBtn";
            this.SkyTemperatureBtn.Size = new System.Drawing.Size(131, 42);
            this.SkyTemperatureBtn.TabIndex = 15;
            this.SkyTemperatureBtn.Text = "Sky-Temperature";
            this.SkyTemperatureBtn.UseVisualStyleBackColor = true;
            this.SkyTemperatureBtn.Click += new System.EventHandler(this.SkyTemperatureBtn_Click);
            // 
            // LuxBtn
            // 
            this.LuxBtn.Location = new System.Drawing.Point(201, 156);
            this.LuxBtn.Name = "LuxBtn";
            this.LuxBtn.Size = new System.Drawing.Size(131, 42);
            this.LuxBtn.TabIndex = 16;
            this.LuxBtn.Text = "Sky-Brightness";
            this.LuxBtn.UseVisualStyleBackColor = true;
            this.LuxBtn.Click += new System.EventHandler(this.LuxBtn_Click);
            // 
            // SQMBtn
            // 
            this.SQMBtn.Location = new System.Drawing.Point(377, 156);
            this.SQMBtn.Name = "SQMBtn";
            this.SQMBtn.Size = new System.Drawing.Size(131, 42);
            this.SQMBtn.TabIndex = 17;
            this.SQMBtn.Text = "Sky-Quality";
            this.SQMBtn.UseVisualStyleBackColor = true;
            this.SQMBtn.Click += new System.EventHandler(this.SQMBtn_Click);
            // 
            // PressureBtn
            // 
            this.PressureBtn.Location = new System.Drawing.Point(18, 222);
            this.PressureBtn.Name = "PressureBtn";
            this.PressureBtn.Size = new System.Drawing.Size(131, 42);
            this.PressureBtn.TabIndex = 18;
            this.PressureBtn.Text = "Atmospheric Pressure";
            this.PressureBtn.UseVisualStyleBackColor = true;
            this.PressureBtn.Click += new System.EventHandler(this.PressureBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 306);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 17);
            this.label1.TabIndex = 110;
            this.label1.Text = "(c) R Brown 2017-2018. All rights reserved";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 339);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PressureBtn);
            this.Controls.Add(this.SQMBtn);
            this.Controls.Add(this.LuxBtn);
            this.Controls.Add(this.SkyTemperatureBtn);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.StatusMsg);
            this.Controls.Add(this.MessageTxtBox);
            this.Controls.Add(this.DewPointBtn);
            this.Controls.Add(this.HumidityBtn);
            this.Controls.Add(this.TemperatureBtn);
            this.Controls.Add(this.labelDriverId);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonChoose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "mySQMPRO ASCOM Tester App v104";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChoose;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label labelDriverId;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Label StatusMsg;
        private System.Windows.Forms.TextBox MessageTxtBox;
        private System.Windows.Forms.Button DewPointBtn;
        private System.Windows.Forms.Button HumidityBtn;
        private System.Windows.Forms.Button TemperatureBtn;
        private System.Windows.Forms.Button SkyTemperatureBtn;
        private System.Windows.Forms.Button LuxBtn;
        private System.Windows.Forms.Button SQMBtn;
        private System.Windows.Forms.Button PressureBtn;
        private System.Windows.Forms.Label label1;
    }
}

