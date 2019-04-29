namespace ASCOM.mySQMPRO
{
    partial class SetupDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupDialogForm));
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comportspeed = new System.Windows.Forms.ListBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.mscthresholdLbl = new System.Windows.Forms.Label();
            this.ResetControllerChkBox = new System.Windows.Forms.CheckBox();
            this.RefreshComPortBtn = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Checked = global::ASCOM.mySQMPRO.Properties.Settings.Default.TraceEnabled;
            this.chkTrace.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASCOM.mySQMPRO.Properties.Settings.Default, "TraceEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkTrace.Location = new System.Drawing.Point(15, 125);
            this.chkTrace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(97, 24);
            this.chkTrace.TabIndex = 6;
            this.chkTrace.Text = "Trace on";
            this.chkTrace.UseVisualStyleBackColor = true;
            this.chkTrace.CheckedChanged += new System.EventHandler(this.chkTrace_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 30);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 20);
            this.label11.TabIndex = 90;
            this.label11.Text = "Port Speed";
            // 
            // comportspeed
            // 
            this.comportspeed.FormattingEnabled = true;
            this.comportspeed.ItemHeight = 20;
            this.comportspeed.Items.AddRange(new object[] {
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600",
            "115200"});
            this.comportspeed.Location = new System.Drawing.Point(18, 56);
            this.comportspeed.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comportspeed.Name = "comportspeed";
            this.comportspeed.Size = new System.Drawing.Size(87, 44);
            this.comportspeed.TabIndex = 89;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(124, 56);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(87, 28);
            this.comboBox1.TabIndex = 88;
            // 
            // mscthresholdLbl
            // 
            this.mscthresholdLbl.AutoSize = true;
            this.mscthresholdLbl.Location = new System.Drawing.Point(120, 30);
            this.mscthresholdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mscthresholdLbl.Name = "mscthresholdLbl";
            this.mscthresholdLbl.Size = new System.Drawing.Size(38, 20);
            this.mscthresholdLbl.TabIndex = 87;
            this.mscthresholdLbl.Text = "Port";
            // 
            // ResetControllerChkBox
            // 
            this.ResetControllerChkBox.AutoSize = true;
            this.ResetControllerChkBox.Checked = global::ASCOM.mySQMPRO.Properties.Settings.Default.ResetControllerOnConnect;
            this.ResetControllerChkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ResetControllerChkBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASCOM.mySQMPRO.Properties.Settings.Default, "ResetControllerOnConnect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ResetControllerChkBox.Location = new System.Drawing.Point(15, 164);
            this.ResetControllerChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ResetControllerChkBox.Name = "ResetControllerChkBox";
            this.ResetControllerChkBox.Size = new System.Drawing.Size(239, 24);
            this.ResetControllerChkBox.TabIndex = 105;
            this.ResetControllerChkBox.Text = "Reset Controller On Connect";
            this.ResetControllerChkBox.UseVisualStyleBackColor = true;
            // 
            // RefreshComPortBtn
            // 
            this.RefreshComPortBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshComPortBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.RefreshComPortBtn.Location = new System.Drawing.Point(381, 114);
            this.RefreshComPortBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.RefreshComPortBtn.Name = "RefreshComPortBtn";
            this.RefreshComPortBtn.Size = new System.Drawing.Size(90, 38);
            this.RefreshComPortBtn.TabIndex = 108;
            this.RefreshComPortBtn.Text = "Refresh";
            this.RefreshComPortBtn.UseVisualStyleBackColor = true;
            this.RefreshComPortBtn.Click += new System.EventHandler(this.RefreshComPortBtn_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(381, 175);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(89, 39);
            this.cmdCancel.TabIndex = 107;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(381, 49);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(89, 38);
            this.cmdOK.TabIndex = 106;
            this.cmdOK.Text = "Connect";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 231);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 20);
            this.label1.TabIndex = 109;
            this.label1.Text = "(c) R Brown 2017-2018. All rights reserved";
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 269);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RefreshComPortBtn);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.ResetControllerChkBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comportspeed);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.mscthresholdLbl);
            this.Controls.Add(this.chkTrace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mySQMPRO ASCOM OC Driver v104";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupDialogForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupDialogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox comportspeed;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label mscthresholdLbl;
        private System.Windows.Forms.CheckBox ResetControllerChkBox;
        private System.Windows.Forms.Button RefreshComPortBtn;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label1;
    }
}