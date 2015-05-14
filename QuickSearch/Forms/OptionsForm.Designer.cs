using QuickSearch.Controls;

namespace QuickSearch.Forms
{
    partial class OptionsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAddDir = new QuickSearch.Controls.MenuButton();
            this.cbShowIndexErrors = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numMaxResults = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.cbAllowFolderSearch = new System.Windows.Forms.CheckBox();
            this.cbSmallIcons = new System.Windows.Forms.CheckBox();
            this.cbLaunchWithWindows = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAudio = new System.Windows.Forms.CheckBox();
            this.cbImages = new System.Windows.Forms.CheckBox();
            this.cbVideo = new System.Windows.Forms.CheckBox();
            this.btnRemoveDir = new System.Windows.Forms.Button();
            this.lbDirectories = new System.Windows.Forms.ListBox();
            this.tbHotkey = new QuickSearch.Controls.ShortcutKeyTextBox();
            this.cbSysTray = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.cbShowFilePreview = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxResults)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbShowFilePreview);
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnAddDir);
            this.groupBox1.Controls.Add(this.cbShowIndexErrors);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numMaxResults);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbAllowFolderSearch);
            this.groupBox1.Controls.Add(this.cbSmallIcons);
            this.groupBox1.Controls.Add(this.cbLaunchWithWindows);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbAudio);
            this.groupBox1.Controls.Add(this.cbImages);
            this.groupBox1.Controls.Add(this.cbVideo);
            this.groupBox1.Controls.Add(this.btnRemoveDir);
            this.groupBox1.Controls.Add(this.lbDirectories);
            this.groupBox1.Controls.Add(this.tbHotkey);
            this.groupBox1.Controls.Add(this.cbSysTray);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbFilter);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(414, 416);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search options";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(244, 23);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 21;
            this.btnClear.Text = "Clear All";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAddDir
            // 
            this.btnAddDir.Location = new System.Drawing.Point(80, 23);
            this.btnAddDir.Name = "btnAddDir";
            this.btnAddDir.Size = new System.Drawing.Size(76, 23);
            this.btnAddDir.TabIndex = 20;
            this.btnAddDir.Text = "Add";
            this.btnAddDir.UseVisualStyleBackColor = true;
            // 
            // cbShowIndexErrors
            // 
            this.cbShowIndexErrors.AutoSize = true;
            this.cbShowIndexErrors.Location = new System.Drawing.Point(31, 352);
            this.cbShowIndexErrors.Name = "cbShowIndexErrors";
            this.cbShowIndexErrors.Size = new System.Drawing.Size(294, 17);
            this.cbShowIndexErrors.TabIndex = 19;
            this.cbShowIndexErrors.Text = "Display errors which occurred while building search index";
            this.cbShowIndexErrors.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(248, 380);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 32);
            this.label6.TabIndex = 18;
            this.label6.Text = "(Decrease this if the interface is unresponsive.)";
            // 
            // numMaxResults
            // 
            this.numMaxResults.Location = new System.Drawing.Point(145, 384);
            this.numMaxResults.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numMaxResults.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMaxResults.Name = "numMaxResults";
            this.numMaxResults.Size = new System.Drawing.Size(97, 20);
            this.numMaxResults.TabIndex = 17;
            this.numMaxResults.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 386);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Maximum results to show:";
            // 
            // cbAllowFolderSearch
            // 
            this.cbAllowFolderSearch.AutoSize = true;
            this.cbAllowFolderSearch.Location = new System.Drawing.Point(79, 124);
            this.cbAllowFolderSearch.Name = "cbAllowFolderSearch";
            this.cbAllowFolderSearch.Size = new System.Drawing.Size(149, 17);
            this.cbAllowFolderSearch.TabIndex = 15;
            this.cbAllowFolderSearch.Text = "Allow searching for folders";
            this.cbAllowFolderSearch.UseVisualStyleBackColor = true;
            // 
            // cbSmallIcons
            // 
            this.cbSmallIcons.AutoSize = true;
            this.cbSmallIcons.Location = new System.Drawing.Point(31, 306);
            this.cbSmallIcons.Name = "cbSmallIcons";
            this.cbSmallIcons.Size = new System.Drawing.Size(141, 17);
            this.cbSmallIcons.TabIndex = 14;
            this.cbSmallIcons.Text = "Use small icons in file list";
            this.cbSmallIcons.UseVisualStyleBackColor = true;
            this.cbSmallIcons.CheckedChanged += new System.EventHandler(this.cbSmallIcons_CheckedChanged);
            // 
            // cbLaunchWithWindows
            // 
            this.cbLaunchWithWindows.AutoSize = true;
            this.cbLaunchWithWindows.Location = new System.Drawing.Point(31, 283);
            this.cbLaunchWithWindows.Name = "cbLaunchWithWindows";
            this.cbLaunchWithWindows.Size = new System.Drawing.Size(166, 17);
            this.cbLaunchWithWindows.TabIndex = 13;
            this.cbLaunchWithWindows.Text = "Launch when Windows starts";
            this.cbLaunchWithWindows.UseVisualStyleBackColor = true;
            this.cbLaunchWithWindows.CheckedChanged += new System.EventHandler(this.cbLaunchWithWindows_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(6, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(402, 2);
            this.label4.TabIndex = 12;
            // 
            // cbAudio
            // 
            this.cbAudio.AutoSize = true;
            this.cbAudio.Location = new System.Drawing.Point(205, 184);
            this.cbAudio.Name = "cbAudio";
            this.cbAudio.Size = new System.Drawing.Size(53, 17);
            this.cbAudio.TabIndex = 11;
            this.cbAudio.Text = "Audio";
            this.cbAudio.UseVisualStyleBackColor = true;
            this.cbAudio.CheckedChanged += new System.EventHandler(this.cbAudio_CheckedChanged);
            // 
            // cbImages
            // 
            this.cbImages.AutoSize = true;
            this.cbImages.Location = new System.Drawing.Point(139, 184);
            this.cbImages.Name = "cbImages";
            this.cbImages.Size = new System.Drawing.Size(60, 17);
            this.cbImages.TabIndex = 10;
            this.cbImages.Text = "Images";
            this.cbImages.UseVisualStyleBackColor = true;
            this.cbImages.CheckedChanged += new System.EventHandler(this.cbImages_CheckedChanged);
            // 
            // cbVideo
            // 
            this.cbVideo.AutoSize = true;
            this.cbVideo.Location = new System.Drawing.Point(80, 184);
            this.cbVideo.Name = "cbVideo";
            this.cbVideo.Size = new System.Drawing.Size(53, 17);
            this.cbVideo.TabIndex = 9;
            this.cbVideo.Text = "Video";
            this.cbVideo.UseVisualStyleBackColor = true;
            this.cbVideo.CheckedChanged += new System.EventHandler(this.cbVideo_CheckedChanged);
            // 
            // btnRemoveDir
            // 
            this.btnRemoveDir.Location = new System.Drawing.Point(162, 23);
            this.btnRemoveDir.Name = "btnRemoveDir";
            this.btnRemoveDir.Size = new System.Drawing.Size(76, 23);
            this.btnRemoveDir.TabIndex = 7;
            this.btnRemoveDir.Text = "Remove";
            this.btnRemoveDir.UseVisualStyleBackColor = true;
            this.btnRemoveDir.Click += new System.EventHandler(this.btnRemoveDir_Click);
            // 
            // lbDirectories
            // 
            this.lbDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDirectories.FormattingEnabled = true;
            this.lbDirectories.Location = new System.Drawing.Point(80, 49);
            this.lbDirectories.Name = "lbDirectories";
            this.lbDirectories.Size = new System.Drawing.Size(317, 69);
            this.lbDirectories.TabIndex = 6;
            this.lbDirectories.SelectedIndexChanged += new System.EventHandler(this.lbDirectories_SelectedIndexChanged);
            // 
            // tbHotkey
            // 
            this.tbHotkey.BackColor = System.Drawing.SystemColors.Window;
            this.tbHotkey.Location = new System.Drawing.Point(98, 254);
            this.tbHotkey.Name = "tbHotkey";
            this.tbHotkey.ReadOnly = true;
            this.tbHotkey.Size = new System.Drawing.Size(205, 20);
            this.tbHotkey.TabIndex = 4;
            this.tbHotkey.Text = "None";
            // 
            // cbSysTray
            // 
            this.cbSysTray.AutoSize = true;
            this.cbSysTray.Location = new System.Drawing.Point(31, 231);
            this.cbSysTray.Name = "cbSysTray";
            this.cbSysTray.Size = new System.Drawing.Size(119, 17);
            this.cbSysTray.TabIndex = 5;
            this.cbSysTray.Text = "Close to system tray";
            this.cbSysTray.UseVisualStyleBackColor = true;
            this.cbSysTray.CheckedChanged += new System.EventHandler(this.cbSysTray_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Hotkey:";
            // 
            // tbFilter
            // 
            this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilter.Location = new System.Drawing.Point(80, 158);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(317, 20);
            this.tbFilter.TabIndex = 3;
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Filter:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Directories:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(352, 437);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(271, 437);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAbout.Location = new System.Drawing.Point(12, 435);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(109, 29);
            this.btnAbout.TabIndex = 8;
            this.btnAbout.Text = "About QuickSearch";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // cbShowFilePreview
            // 
            this.cbShowFilePreview.AutoSize = true;
            this.cbShowFilePreview.Location = new System.Drawing.Point(31, 329);
            this.cbShowFilePreview.Name = "cbShowFilePreview";
            this.cbShowFilePreview.Size = new System.Drawing.Size(161, 17);
            this.cbShowFilePreview.TabIndex = 22;
            this.cbShowFilePreview.Text = "Show large filename preview";
            this.cbShowFilePreview.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(439, 474);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox cbSysTray;
        private ShortcutKeyTextBox tbHotkey;
        private System.Windows.Forms.ListBox lbDirectories;
        private System.Windows.Forms.Button btnRemoveDir;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbAudio;
        private System.Windows.Forms.CheckBox cbImages;
        private System.Windows.Forms.CheckBox cbVideo;
        private System.Windows.Forms.CheckBox cbSmallIcons;
        private System.Windows.Forms.CheckBox cbLaunchWithWindows;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.CheckBox cbAllowFolderSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numMaxResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbShowIndexErrors;
        private MenuButton btnAddDir;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox cbShowFilePreview;
    }
}