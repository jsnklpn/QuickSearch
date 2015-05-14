using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickSearch.Forms
{
    public partial class FilePreviewForm : Form
    {
        public FilePreviewForm()
        {
            InitializeComponent();
        }

        public string LabelText
        {
            get
            {
                return this.lblText.Text;
            }
            set
            {
                this.lblText.Text = value;
            }
        }

        public void SetLabelIcon(Icon icon)
        {
            var currentImage = this.pictureBox1.Image;
            if (currentImage != null)
            {
                currentImage.Dispose();
            }

            this.pictureBox1.Image = icon.ToBitmap();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
 	         base.OnPaintBackground(e);

             var pen = new Pen(Color.Black) { Width = 2 };
             e.Graphics.DrawRectangle(pen, this.ClientRectangle);
             pen.Dispose();
        }
    }
}
