using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickSearch.Models;

namespace QuickSearch.Forms
{
    public partial class ErrorListForm : Form
    {
        /// <summary>
        /// The user selected to not show this window again.
        /// </summary>
        public bool DontShowAgain { get; private set; }

        public ErrorListForm(string caption, IEnumerable<FileSystemItemError> errors)
        {
            InitializeComponent();

            this.DontShowAgain = false;

            //this.Icon = Properties.Resources.icon2;
            this.pictureBox1.Image = SystemIcons.Error.ToBitmap();

            this.lblDescription.Text = caption;
            this.lbErrorList.SelectionMode = SelectionMode.One;
            this.lbErrorList.DisplayMember = "Path";
            this.lbErrorList.DataSource = errors.ToList();

            if (errors.Any())
            {
                this.lbErrorList.SelectedIndex = 0;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.pictureBox1.Image.Dispose();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DontShowAgain = cbDontShowAgain.Checked;

            this.Close();
        }

        private void lbErrorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateError();
        }

        private void PopulateError()
        {
            var selectedError = lbErrorList.SelectedItem as FileSystemItemError;
            if (selectedError != null)
            {
                this.tbError.Text = string.Format("{0}{1}{1}{2}", selectedError.ErrorMessage, Environment.NewLine,
                    selectedError.ErrorDetails);
            }
        }
    }
}
