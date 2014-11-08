namespace TestService
{
    partial class MainForm
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
            this.lblUserId = new System.Windows.Forms.Label();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.txtDeviceId = new System.Windows.Forms.TextBox();
            this.lblDeviceId = new System.Windows.Forms.Label();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.txtEndTime = new System.Windows.Forms.TextBox();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.txtLongitude = new System.Windows.Forms.TextBox();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.txtLatitude = new System.Windows.Forms.TextBox();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.txtAltitude = new System.Windows.Forms.TextBox();
            this.lblAltitude = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvGpsData = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGpsData)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUserId
            // 
            this.lblUserId.AutoSize = true;
            this.lblUserId.Location = new System.Drawing.Point(72, 29);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(56, 19);
            this.lblUserId.TabIndex = 0;
            this.lblUserId.Text = "UserId";
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(169, 26);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(317, 27);
            this.txtUserId.TabIndex = 1;
            this.txtUserId.Text = "75c022a7-757c-4fc9-9809-27092d4f4aef";
            // 
            // txtDeviceId
            // 
            this.txtDeviceId.Location = new System.Drawing.Point(169, 56);
            this.txtDeviceId.Name = "txtDeviceId";
            this.txtDeviceId.Size = new System.Drawing.Size(317, 27);
            this.txtDeviceId.TabIndex = 3;
            this.txtDeviceId.Text = "32e4a2b6-ec8a-4bf7-adee-d40759e818b5";
            // 
            // lblDeviceId
            // 
            this.lblDeviceId.AutoSize = true;
            this.lblDeviceId.Location = new System.Drawing.Point(74, 62);
            this.lblDeviceId.Name = "lblDeviceId";
            this.lblDeviceId.Size = new System.Drawing.Size(70, 19);
            this.lblDeviceId.TabIndex = 2;
            this.lblDeviceId.Text = "DeviceId";
            // 
            // txtStartTime
            // 
            this.txtStartTime.Location = new System.Drawing.Point(170, 116);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(317, 27);
            this.txtStartTime.TabIndex = 5;
            this.txtStartTime.Text = "230394133519";
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(74, 116);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(77, 19);
            this.lblStartTime.TabIndex = 4;
            this.lblStartTime.Text = "End Time";
            // 
            // txtEndTime
            // 
            this.txtEndTime.Location = new System.Drawing.Point(169, 86);
            this.txtEndTime.Name = "txtEndTime";
            this.txtEndTime.Size = new System.Drawing.Size(317, 27);
            this.txtEndTime.TabIndex = 5;
            this.txtEndTime.Text = "230394133519";
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(74, 91);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(83, 19);
            this.lblEndTime.TabIndex = 4;
            this.lblEndTime.Text = "Start Time";
            // 
            // txtLongitude
            // 
            this.txtLongitude.Location = new System.Drawing.Point(169, 148);
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.Size = new System.Drawing.Size(317, 27);
            this.txtLongitude.TabIndex = 7;
            this.txtLongitude.Text = "4807.038,N";
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(74, 153);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(79, 19);
            this.lblLongitude.TabIndex = 14;
            this.lblLongitude.Text = "Longitude";
            // 
            // txtLatitude
            // 
            this.txtLatitude.Location = new System.Drawing.Point(169, 181);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.Size = new System.Drawing.Size(317, 27);
            this.txtLatitude.TabIndex = 15;
            this.txtLatitude.Text = "01131.000,E";
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(77, 185);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(65, 19);
            this.lblLatitude.TabIndex = 16;
            this.lblLatitude.Text = "Latitude";
            // 
            // txtAltitude
            // 
            this.txtAltitude.Location = new System.Drawing.Point(169, 213);
            this.txtAltitude.Name = "txtAltitude";
            this.txtAltitude.Size = new System.Drawing.Size(317, 27);
            this.txtAltitude.TabIndex = 17;
            this.txtAltitude.Text = "003.1";
            // 
            // lblAltitude
            // 
            this.lblAltitude.Location = new System.Drawing.Point(74, 215);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(65, 19);
            this.lblAltitude.TabIndex = 18;
            this.lblAltitude.Text = "Altitude";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(26, 253);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(53, 27);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(93, 252);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(146, 27);
            this.btnGet.TabIndex = 9;
            this.btnGet.Text = "Get";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(253, 252);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(146, 27);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvGpsData
            // 
            this.dgvGpsData.AllowUserToAddRows = false;
            this.dgvGpsData.AllowUserToDeleteRows = false;
            this.dgvGpsData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGpsData.Location = new System.Drawing.Point(26, 296);
            this.dgvGpsData.Name = "dgvGpsData";
            this.dgvGpsData.ReadOnly = true;
            this.dgvGpsData.Size = new System.Drawing.Size(675, 227);
            this.dgvGpsData.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 542);
            this.Controls.Add(this.dgvGpsData);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtLongitude);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.txtLatitude);
            this.Controls.Add(this.lblLatitude);
            this.Controls.Add(this.txtAltitude);
            this.Controls.Add(this.lblAltitude);
            this.Controls.Add(this.txtStartTime);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.txtEndTime);
            this.Controls.Add(this.lblEndTime);
            this.Controls.Add(this.txtDeviceId);
            this.Controls.Add(this.lblDeviceId);
            this.Controls.Add(this.txtUserId);
            this.Controls.Add(this.lblUserId);
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Test the Service";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGpsData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserId;
        private System.Windows.Forms.TextBox txtUserId;

        private System.Windows.Forms.TextBox txtDeviceId;
        private System.Windows.Forms.Label lblDeviceId;

        private System.Windows.Forms.TextBox txtStartTime;
        private System.Windows.Forms.Label lblStartTime;

        private System.Windows.Forms.TextBox txtEndTime;
        private System.Windows.Forms.Label lblEndTime;

		private System.Windows.Forms.TextBox txtAltitude;
		private System.Windows.Forms.Label lblAltitude;

        private System.Windows.Forms.TextBox txtLongitude;
		private System.Windows.Forms.Label lblLongitude;

		private System.Windows.Forms.TextBox txtLatitude;
		private System.Windows.Forms.Label lblLatitude;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnGet;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.DataGridView dgvGpsData;
    }
}

