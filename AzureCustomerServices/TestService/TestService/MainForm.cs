using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestService
{
    public partial class MainForm : Form
    {

        public DataSet ds_GpsData;
        public BindingSource bs_GpsData;


        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (ValidateFields())
            {
                this.Cursor = Cursors.WaitCursor;
				errorMessage = DAC.AddGpsData(txtUserId.Text, txtDeviceId.Text, txtTime.Text, txtLongitude.Text, txtLatitude.Text, txtAltitude.Text);
                this.Cursor = Cursors.Default;
            }
            if (errorMessage.Length > 0)
                MessageBox.Show(errorMessage);
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (ValidateFields())
            {
                this.Cursor = Cursors.WaitCursor;
				errorMessage = DAC.GetGpsData(txtUserId.Text, txtDeviceId.Text, txtTime.Text, out ds_GpsData);
                this.Cursor = Cursors.Default;
            }
            if (errorMessage.Length > 0)
                MessageBox.Show(errorMessage);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string favoriteMovie = string.Empty;
            string favoriteLanguage = string.Empty;
            string errorMessage = string.Empty;
            if (ValidateFields())
            {
                this.Cursor = Cursors.WaitCursor;
				errorMessage = DAC.DeleteGpsData(txtUserId.Text, txtDeviceId.Text, txtTime.Text);
                this.Cursor = Cursors.Default;
            }

        }

        private bool ValidateFields()
        {
            bool success = true;
            txtDeviceId.Text = txtDeviceId.Text.Trim();
            txtUserId.Text = txtUserId.Text.Trim();
            txtTime.Text = txtTime.Text.Trim();
            txtLongitude.Text = txtLongitude.Text.Trim();
			txtLatitude.Text = txtLatitude.Text.Trim();
			txtAltitude.Text = txtAltitude.Text.Trim();

            if (txtDeviceId.Text.Length == 0)
            {
                success = false;
                MessageBox.Show("Must specify Device Id.");
            }

			if (txtUserId.Text.Length == 0)
			{
				success = false;
				MessageBox.Show("Must specify User Id.");
			}

			if (txtTime.Text.Length == 0)
			{
				success = false;
				MessageBox.Show("Must specify Time.");
			}

			if (txtLongitude.Text.Length == 0)
			{
				success = false;
				MessageBox.Show("Must specify Longitude.");
			}

			if (txtLatitude.Text.Length == 0)
			{
				success = false;
				MessageBox.Show("Must specify Latitude.");
			}

			if (txtAltitude.Text.Length == 0)
			{
				success = false;
				MessageBox.Show("Must specify Altitude.");
			}

            return success;

        }
    }
}
