namespace VideoDownloader
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            urlLabel = new Label();
            urlTextBox = new TextBox();
            pasteButton = new RoundedButton();
            videoFormatButton = new RoundedButton();
            audioFormatButton = new RoundedButton();
            previewPanel = new Panel();
            thumbnailPictureBox = new PictureBox();
            videoTitleLabel = new Label();
            videoChannelLabel = new Label();
            videoDurationLabel = new Label();
            previewLoadingLabel = new Label();
            qualityLabel = new Label();
            qualityComboBox = new ComboBox();
            subtitleCheckBox = new CheckBox();
            pathLabel = new Label();
            pathTextBox = new TextBox();
            browseButton = new RoundedButton();
            downloadButton = new RoundedButton();
            progressPanel = new Panel();
            progressBar = new ProgressBar();
            progressLabel = new Label();
            speedLabel = new Label();
            pauseButton = new RoundedButton();
            cancelButton = new RoundedButton();
            statusLabel = new Label();
            signatureLabel = new Label();
            previewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)thumbnailPictureBox).BeginInit();
            progressPanel.SuspendLayout();
            SuspendLayout();
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            urlLabel.ForeColor = Color.FromArgb(250, 250, 250);
            urlLabel.Location = new Point(30, 30);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(100, 25);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "Video URL";
            // 
            // urlTextBox
            // 
            urlTextBox.BackColor = Color.FromArgb(39, 39, 42);
            urlTextBox.BorderStyle = BorderStyle.FixedSingle;
            urlTextBox.Font = new Font("Segoe UI", 11F);
            urlTextBox.ForeColor = Color.FromArgb(244, 244, 245);
            urlTextBox.Location = new Point(30, 60);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.PlaceholderText = "📋 Paste here";
            urlTextBox.Size = new Size(380, 32);
            urlTextBox.TabIndex = 0;
            urlTextBox.TextChanged += UrlTextBox_TextChanged;
            // 
            // pasteButton
            // 
            pasteButton.BackColor = Color.FromArgb(99, 102, 241);
            pasteButton.BorderRadius = 6;
            pasteButton.FlatStyle = FlatStyle.Flat;
            pasteButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            pasteButton.ForeColor = Color.White;
            pasteButton.Location = new Point(420, 58);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new Size(105, 36);
            pasteButton.TabIndex = 1;
            pasteButton.Text = "📋 Paste";
            pasteButton.UseVisualStyleBackColor = false;
            pasteButton.Click += PasteButton_Click;
            // 
            // videoFormatButton
            // 
            videoFormatButton.BackColor = Color.FromArgb(99, 102, 241);
            videoFormatButton.BorderRadius = 6;
            videoFormatButton.FlatStyle = FlatStyle.Flat;
            videoFormatButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            videoFormatButton.ForeColor = Color.White;
            videoFormatButton.Location = new Point(535, 58);
            videoFormatButton.Name = "videoFormatButton";
            videoFormatButton.Size = new Size(100, 36);
            videoFormatButton.TabIndex = 2;
            videoFormatButton.Text = "🎬 Video";
            videoFormatButton.UseVisualStyleBackColor = false;
            videoFormatButton.Click += VideoFormatButton_Click;
            // 
            // audioFormatButton
            // 
            audioFormatButton.BackColor = Color.FromArgb(39, 39, 42);
            audioFormatButton.BorderRadius = 6;
            audioFormatButton.FlatStyle = FlatStyle.Flat;
            audioFormatButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            audioFormatButton.ForeColor = Color.FromArgb(212, 212, 216);
            audioFormatButton.Location = new Point(645, 58);
            audioFormatButton.Name = "audioFormatButton";
            audioFormatButton.Size = new Size(95, 36);
            audioFormatButton.TabIndex = 3;
            audioFormatButton.Text = "🎵 Ses";
            audioFormatButton.UseVisualStyleBackColor = false;
            audioFormatButton.Click += AudioFormatButton_Click;
            // 
            // previewPanel
            // 
            previewPanel.BackColor = Color.FromArgb(39, 39, 42);
            previewPanel.Controls.Add(thumbnailPictureBox);
            previewPanel.Controls.Add(videoTitleLabel);
            previewPanel.Controls.Add(videoChannelLabel);
            previewPanel.Controls.Add(videoDurationLabel);
            previewPanel.Controls.Add(previewLoadingLabel);
            previewPanel.Location = new Point(30, 110);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(710, 105);
            previewPanel.TabIndex = 4;
            previewPanel.Visible = false;
            // 
            // thumbnailPictureBox
            // 
            thumbnailPictureBox.BackColor = Color.FromArgb(24, 24, 27);
            thumbnailPictureBox.BorderStyle = BorderStyle.FixedSingle;
            thumbnailPictureBox.Location = new Point(10, 10);
            thumbnailPictureBox.Name = "thumbnailPictureBox";
            thumbnailPictureBox.Size = new Size(140, 85);
            thumbnailPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            thumbnailPictureBox.TabIndex = 0;
            thumbnailPictureBox.TabStop = false;
            // 
            // videoTitleLabel
            // 
            videoTitleLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            videoTitleLabel.ForeColor = Color.FromArgb(250, 250, 250);
            videoTitleLabel.Location = new Point(160, 8);
            videoTitleLabel.Name = "videoTitleLabel";
            videoTitleLabel.Size = new Size(540, 50);
            videoTitleLabel.TabIndex = 1;
            videoTitleLabel.MaximumSize = new Size(540, 50);
            videoTitleLabel.AutoEllipsis = true;
            // 
            // videoChannelLabel
            // 
            videoChannelLabel.Font = new Font("Segoe UI", 9F);
            videoChannelLabel.ForeColor = Color.FromArgb(212, 212, 216);
            videoChannelLabel.Location = new Point(160, 60);
            videoChannelLabel.Name = "videoChannelLabel";
            videoChannelLabel.Size = new Size(350, 20);
            videoChannelLabel.TabIndex = 2;
            videoChannelLabel.AutoEllipsis = true;
            videoChannelLabel.Click += videoChannelLabel_Click;
            // 
            // videoDurationLabel
            // 
            videoDurationLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            videoDurationLabel.ForeColor = Color.FromArgb(99, 102, 241);
            videoDurationLabel.Location = new Point(160, 82);
            videoDurationLabel.Name = "videoDurationLabel";
            videoDurationLabel.Size = new Size(150, 20);
            videoDurationLabel.TabIndex = 3;
            // 
            // previewLoadingLabel
            // 
            previewLoadingLabel.Font = new Font("Segoe UI", 10F);
            previewLoadingLabel.ForeColor = Color.FromArgb(212, 212, 216);
            previewLoadingLabel.Location = new Point(10, 40);
            previewLoadingLabel.Name = "previewLoadingLabel";
            previewLoadingLabel.Size = new Size(690, 25);
            previewLoadingLabel.TabIndex = 4;
            previewLoadingLabel.Text = "Loading video info...";
            previewLoadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            previewLoadingLabel.Visible = false;
            // 
            // qualityLabel
            // 
            qualityLabel.AutoSize = true;
            qualityLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            qualityLabel.ForeColor = Color.FromArgb(250, 250, 250);
            qualityLabel.Location = new Point(30, 145);
            qualityLabel.Name = "qualityLabel";
            qualityLabel.Size = new Size(61, 25);
            qualityLabel.TabIndex = 5;
            qualityLabel.Text = "Kalite";
            // 
            // qualityComboBox
            // 
            qualityComboBox.BackColor = Color.FromArgb(39, 39, 42);
            qualityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            qualityComboBox.FlatStyle = FlatStyle.Flat;
            qualityComboBox.Font = new Font("Segoe UI", 11F);
            qualityComboBox.ForeColor = Color.FromArgb(34, 197, 94);
            qualityComboBox.Items.AddRange(new object[] { "En İyi Kalite", "4K (2160p)", "2K (1440p)", "1080p", "720p", "480p", "360p" });
            qualityComboBox.Location = new Point(30, 175);
            qualityComboBox.Name = "qualityComboBox";
            qualityComboBox.Size = new Size(250, 33);
            qualityComboBox.TabIndex = 5;
            // 
            // subtitleCheckBox
            // 
            subtitleCheckBox.AutoSize = true;
            subtitleCheckBox.Font = new Font("Segoe UI", 10F);
            subtitleCheckBox.ForeColor = Color.FromArgb(250, 250, 250);
            subtitleCheckBox.Location = new Point(300, 182);
            subtitleCheckBox.Name = "subtitleCheckBox";
            subtitleCheckBox.Size = new Size(122, 27);
            subtitleCheckBox.TabIndex = 6;
            subtitleCheckBox.Text = "Download Subtitles";
            subtitleCheckBox.UseVisualStyleBackColor = true;
            // 
            // pathLabel
            // 
            pathLabel.AutoSize = true;
            pathLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            pathLabel.ForeColor = Color.FromArgb(250, 250, 250);
            pathLabel.Location = new Point(30, 230);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new Size(148, 25);
            pathLabel.TabIndex = 7;
            pathLabel.Text = "Download Folder";
            // 
            // pathTextBox
            // 
            pathTextBox.BackColor = Color.FromArgb(39, 39, 42);
            pathTextBox.BorderStyle = BorderStyle.FixedSingle;
            pathTextBox.Font = new Font("Segoe UI", 10F);
            pathTextBox.ForeColor = Color.FromArgb(244, 244, 245);
            pathTextBox.Location = new Point(30, 260);
            pathTextBox.Name = "pathTextBox";
            pathTextBox.ReadOnly = true;
            pathTextBox.Size = new Size(585, 30);
            pathTextBox.TabIndex = 7;
            // 
            // browseButton
            // 
            browseButton.BackColor = Color.FromArgb(39, 39, 42);
            browseButton.BorderRadius = 6;
            browseButton.FlatStyle = FlatStyle.Flat;
            browseButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            browseButton.ForeColor = Color.FromArgb(250, 250, 250);
            browseButton.Location = new Point(625, 258);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(115, 34);
            browseButton.TabIndex = 8;
            browseButton.Text = "📁 Browse";
            browseButton.UseVisualStyleBackColor = false;
            browseButton.Click += BrowseButton_Click;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = Color.FromArgb(34, 197, 94);
            downloadButton.BorderRadius = 12;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButton.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            downloadButton.ForeColor = Color.White;
            downloadButton.Location = new Point(30, 320);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(710, 55);
            downloadButton.TabIndex = 9;
            downloadButton.Text = "⬇️ Download";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += DownloadButton_Click;
            // 
            // progressPanel
            // 
            progressPanel.BackColor = Color.FromArgb(39, 39, 42);
            progressPanel.Controls.Add(progressBar);
            progressPanel.Controls.Add(progressLabel);
            progressPanel.Controls.Add(speedLabel);
            progressPanel.Controls.Add(pauseButton);
            progressPanel.Controls.Add(cancelButton);
            progressPanel.Location = new Point(30, 390);
            progressPanel.Name = "progressPanel";
            progressPanel.Size = new Size(710, 70);
            progressPanel.TabIndex = 10;
            progressPanel.Visible = false;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 15);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(600, 20);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 0;
            // 
            // progressLabel
            // 
            progressLabel.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            progressLabel.ForeColor = Color.FromArgb(99, 102, 241);
            progressLabel.Location = new Point(625, 12);
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new Size(70, 25);
            progressLabel.TabIndex = 1;
            progressLabel.Text = "0%";
            progressLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // speedLabel
            // 
            speedLabel.Font = new Font("Segoe UI", 9F);
            speedLabel.ForeColor = Color.FromArgb(212, 212, 216);
            speedLabel.Location = new Point(15, 42);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new Size(500, 20);
            speedLabel.TabIndex = 2;
            speedLabel.Text = "Downloading...";
            // 
            // cancelButton
            // 
            cancelButton.BackColor = Color.FromArgb(239, 68, 68);
            cancelButton.BorderRadius = 8;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(620, 38);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 28);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += CancelButton_Click;
            // 
            // pauseButton
            // 
            pauseButton.BackColor = Color.FromArgb(234, 179, 8);
            pauseButton.BorderRadius = 8;
            pauseButton.FlatStyle = FlatStyle.Flat;
            pauseButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            pauseButton.ForeColor = Color.White;
            pauseButton.Location = new Point(530, 38);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(85, 28);
            pauseButton.TabIndex = 3;
            pauseButton.Text = "⏸ Pause";
            pauseButton.UseVisualStyleBackColor = false;
            pauseButton.Click += PauseButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusLabel.Font = new Font("Segoe UI", 9F);
            statusLabel.ForeColor = Color.FromArgb(212, 212, 216);
            statusLabel.Location = new Point(30, 405);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(300, 25);
            statusLabel.TabIndex = 11;
            statusLabel.Text = "Ready";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // signatureLabel
            // 
            signatureLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            signatureLabel.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            signatureLabel.ForeColor = Color.FromArgb(113, 113, 122);
            signatureLabel.Location = new Point(630, 405);
            signatureLabel.Name = "signatureLabel";
            signatureLabel.Size = new Size(110, 25);
            signatureLabel.TabIndex = 12;
            signatureLabel.Text = "kayapater";
            signatureLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            ClientSize = new Size(770, 458);
            Controls.Add(urlLabel);
            Controls.Add(urlTextBox);
            Controls.Add(pasteButton);
            Controls.Add(videoFormatButton);
            Controls.Add(audioFormatButton);
            Controls.Add(previewPanel);
            Controls.Add(qualityLabel);
            Controls.Add(qualityComboBox);
            Controls.Add(subtitleCheckBox);
            Controls.Add(pathLabel);
            Controls.Add(pathTextBox);
            Controls.Add(browseButton);
            Controls.Add(downloadButton);
            Controls.Add(progressPanel);
            Controls.Add(signatureLabel);
            Controls.Add(statusLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            MaximumSize = new Size(788, 505);
            MinimumSize = new Size(788, 505);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Video Downloader";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            previewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)thumbnailPictureBox).EndInit();
            progressPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label urlLabel;
        private TextBox urlTextBox;
        private RoundedButton pasteButton;
        private RoundedButton videoFormatButton;
        private RoundedButton audioFormatButton;
        private Panel previewPanel;
        private PictureBox thumbnailPictureBox;
        private Label videoTitleLabel;
        private Label videoDurationLabel;
        private Label videoChannelLabel;
        private Label previewLoadingLabel;
        private Label qualityLabel;
        private ComboBox qualityComboBox;
        private CheckBox subtitleCheckBox;
        private Label pathLabel;
        private TextBox pathTextBox;
        private RoundedButton browseButton;
        private RoundedButton downloadButton;
        private Panel progressPanel;
        private ProgressBar progressBar;
        private Label progressLabel;
        private Label speedLabel;
        private RoundedButton cancelButton;
        private RoundedButton pauseButton;
        private Label statusLabel;
        private Label signatureLabel;
    }
}
