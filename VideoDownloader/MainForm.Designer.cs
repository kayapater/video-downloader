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
            mediaTypeLabel = new Label();
            videoRadioButton = new RadioButton();
            audioRadioButton = new RadioButton();
            qualityLabel = new Label();
            qualityComboBox = new ComboBox();
            subtitleCheckBox = new CheckBox();
            pathLabel = new Label();
            pathTextBox = new TextBox();
            browseButton = new Button();
            downloadButton = new Button();
            updateButton = new Button();
            progressBar = new ProgressBar();
            statusLabel = new Label();
            progressGroupBox = new GroupBox();
            timeRemainingLabel = new Label();
            speedLabel = new Label();
            currentFileLabel = new Label();
            progressPercentageLabel = new Label();
            progressStatusLabel = new Label();
            pauseResumeButton = new Button();
            cancelButton = new Button();
            logTextBox = new RichTextBox();
            progressGroupBox.SuspendLayout();
            SuspendLayout();

            urlLabel.AutoSize = true;
            urlLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            urlLabel.ForeColor = Color.FromArgb(45, 45, 45);
            urlLabel.Location = new Point(25, 33);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(98, 23);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "Video URL:";

            urlTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            urlTextBox.BorderStyle = BorderStyle.FixedSingle;
            urlTextBox.Font = new Font("Segoe UI", 10F);
            urlTextBox.Location = new Point(25, 60);
            urlTextBox.Margin = new Padding(3, 4, 3, 4);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.PlaceholderText = "YouTube, Twitter, Instagram veya diğer platform linklerini buraya yapıştırın...";
            urlTextBox.Size = new Size(845, 30);
            urlTextBox.TabIndex = 1;

            mediaTypeLabel.AutoSize = true;
            mediaTypeLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            mediaTypeLabel.ForeColor = Color.FromArgb(45, 45, 45);
            mediaTypeLabel.Location = new Point(25, 105);
            mediaTypeLabel.Name = "mediaTypeLabel";
            mediaTypeLabel.Size = new Size(110, 23);
            mediaTypeLabel.TabIndex = 2;
            mediaTypeLabel.Text = "Medya Türü:";

            videoRadioButton.AutoSize = true;
            videoRadioButton.Checked = true;
            videoRadioButton.Font = new Font("Segoe UI", 10F);
            videoRadioButton.ForeColor = Color.FromArgb(45, 45, 45);
            videoRadioButton.Location = new Point(25, 135);
            videoRadioButton.Name = "videoRadioButton";
            videoRadioButton.Size = new Size(103, 27);
            videoRadioButton.TabIndex = 3;
            videoRadioButton.TabStop = true;
            videoRadioButton.Text = "🎬 Video";
            videoRadioButton.UseVisualStyleBackColor = true;
            videoRadioButton.CheckedChanged += VideoRadioButton_CheckedChanged;

            audioRadioButton.AutoSize = true;
            audioRadioButton.Font = new Font("Segoe UI", 10F);
            audioRadioButton.ForeColor = Color.FromArgb(45, 45, 45);
            audioRadioButton.Location = new Point(140, 135);
            audioRadioButton.Name = "audioRadioButton";
            audioRadioButton.Size = new Size(84, 27);
            audioRadioButton.TabIndex = 4;
            audioRadioButton.Text = "🎵 Ses";
            audioRadioButton.UseVisualStyleBackColor = true;
            audioRadioButton.CheckedChanged += AudioRadioButton_CheckedChanged;

            qualityLabel.AutoSize = true;
            qualityLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            qualityLabel.ForeColor = Color.FromArgb(45, 45, 45);
            qualityLabel.Location = new Point(250, 105);
            qualityLabel.Name = "qualityLabel";
            qualityLabel.Size = new Size(61, 23);
            qualityLabel.TabIndex = 5;
            qualityLabel.Text = "Kalite:";

            qualityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            qualityComboBox.FlatStyle = FlatStyle.Flat;
            qualityComboBox.Font = new Font("Segoe UI", 10F);
            qualityComboBox.FormattingEnabled = true;
            qualityComboBox.Items.AddRange(new object[] { "En İyi", "2160p", "1440p", "1080p", "720p", "480p", "360p" });
            qualityComboBox.Location = new Point(250, 135);
            qualityComboBox.Margin = new Padding(3, 4, 3, 4);
            qualityComboBox.Name = "qualityComboBox";
            qualityComboBox.Size = new Size(180, 31);
            qualityComboBox.TabIndex = 6;
            qualityComboBox.SelectedIndex = 0; 

            subtitleCheckBox.AutoSize = true;
            subtitleCheckBox.Font = new Font("Segoe UI", 10F);
            subtitleCheckBox.ForeColor = Color.FromArgb(45, 45, 45);
            subtitleCheckBox.Location = new Point(450, 140);
            subtitleCheckBox.Margin = new Padding(3, 4, 3, 4);
            subtitleCheckBox.Name = "subtitleCheckBox";
            subtitleCheckBox.Size = new Size(122, 27);
            subtitleCheckBox.TabIndex = 7;
            subtitleCheckBox.Text = "Altyazı İndir";
            subtitleCheckBox.UseVisualStyleBackColor = true;

            pathLabel.AutoSize = true;
            pathLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            pathLabel.ForeColor = Color.FromArgb(45, 45, 45);
            pathLabel.Location = new Point(25, 185);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new Size(116, 23);
            pathLabel.TabIndex = 10;
            pathLabel.Text = "İndirme Yolu:";

            pathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathTextBox.BorderStyle = BorderStyle.FixedSingle;
            pathTextBox.Font = new Font("Segoe UI", 10F);
            pathTextBox.Location = new Point(25, 215);
            pathTextBox.Margin = new Padding(3, 4, 3, 4);
            pathTextBox.Name = "pathTextBox";
            pathTextBox.Size = new Size(685, 30);
            pathTextBox.TabIndex = 11;

            browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseButton.BackColor = Color.FromArgb(108, 117, 125);
            browseButton.FlatAppearance.BorderSize = 0;
            browseButton.FlatStyle = FlatStyle.Flat;
            browseButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            browseButton.ForeColor = Color.White;
            browseButton.Location = new Point(730, 215);
            browseButton.Margin = new Padding(3, 4, 3, 4);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(140, 35);
            browseButton.TabIndex = 12;
            browseButton.Text = "📁 Gözat";
            browseButton.UseVisualStyleBackColor = false;
            browseButton.Click += BrowseButton_Click;

            downloadButton.BackColor = Color.FromArgb(40, 167, 69);
            downloadButton.FlatAppearance.BorderSize = 0;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            downloadButton.ForeColor = Color.White;
            downloadButton.Location = new Point(25, 275);
            downloadButton.Margin = new Padding(3, 4, 3, 4);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(190, 55);
            downloadButton.TabIndex = 13;
            downloadButton.Text = "🎬 İndir";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += DownloadButton_Click;

            updateButton.BackColor = Color.FromArgb(0, 123, 255);
            updateButton.FlatAppearance.BorderSize = 0;
            updateButton.FlatStyle = FlatStyle.Flat;
            updateButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            updateButton.ForeColor = Color.White;
            updateButton.Location = new Point(230, 275);
            updateButton.Margin = new Padding(3, 4, 3, 4);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(170, 55);
            updateButton.TabIndex = 14;
            updateButton.Text = "🔧 Sistem Kontrolü";
            updateButton.UseVisualStyleBackColor = false;
            updateButton.Click += UpdateButton_Click;

            progressBar.Location = new Point(20, 35);
            progressBar.Margin = new Padding(3, 4, 3, 4);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(804, 28);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 11;

            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            statusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            statusLabel.ForeColor = Color.FromArgb(108, 117, 125);
            statusLabel.Location = new Point(25, 350);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(846, 25);
            statusLabel.TabIndex = 12;
            statusLabel.Text = "Hazır";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;

            progressGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressGroupBox.Controls.Add(cancelButton);
            progressGroupBox.Controls.Add(pauseResumeButton);
            progressGroupBox.Controls.Add(timeRemainingLabel);
            progressGroupBox.Controls.Add(speedLabel);
            progressGroupBox.Controls.Add(currentFileLabel);
            progressGroupBox.Controls.Add(progressPercentageLabel);
            progressGroupBox.Controls.Add(progressStatusLabel);
            progressGroupBox.Controls.Add(progressBar);
            progressGroupBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            progressGroupBox.ForeColor = Color.FromArgb(45, 45, 45);
            progressGroupBox.Location = new Point(25, 385);
            progressGroupBox.Name = "progressGroupBox";
            progressGroupBox.Size = new Size(845, 145);
            progressGroupBox.TabIndex = 13;
            progressGroupBox.TabStop = false;
            progressGroupBox.Text = "📊 İndirme Durumu";
            progressGroupBox.Visible = false;

            timeRemainingLabel.Font = new Font("Segoe UI", 9F);
            timeRemainingLabel.ForeColor = Color.FromArgb(0, 123, 255);
            timeRemainingLabel.Location = new Point(240, 115);
            timeRemainingLabel.Name = "timeRemainingLabel";
            timeRemainingLabel.Size = new Size(200, 20);
            timeRemainingLabel.TabIndex = 18;

            speedLabel.Font = new Font("Segoe UI", 9F);
            speedLabel.ForeColor = Color.FromArgb(40, 167, 69);
            speedLabel.Location = new Point(20, 115);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new Size(200, 20);
            speedLabel.TabIndex = 17;

            currentFileLabel.Font = new Font("Segoe UI", 9F);
            currentFileLabel.ForeColor = Color.FromArgb(108, 117, 125);
            currentFileLabel.Location = new Point(20, 90);
            currentFileLabel.Name = "currentFileLabel";
            currentFileLabel.Size = new Size(804, 20);
            currentFileLabel.TabIndex = 16;

            progressPercentageLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            progressPercentageLabel.ForeColor = Color.FromArgb(45, 45, 45);
            progressPercentageLabel.Location = new Point(640, 70);
            progressPercentageLabel.Name = "progressPercentageLabel";
            progressPercentageLabel.Size = new Size(80, 20);
            progressPercentageLabel.TabIndex = 15;
            progressPercentageLabel.Text = "0%";
            progressPercentageLabel.TextAlign = ContentAlignment.MiddleRight;

            progressStatusLabel.Font = new Font("Segoe UI", 10F);
            progressStatusLabel.ForeColor = Color.FromArgb(45, 45, 45);
            progressStatusLabel.Location = new Point(20, 70);
            progressStatusLabel.Name = "progressStatusLabel";
            progressStatusLabel.Size = new Size(600, 20);
            progressStatusLabel.TabIndex = 14;
            progressStatusLabel.Text = "Hazırlanıyor...";

            pauseResumeButton.BackColor = Color.FromArgb(255, 193, 7);
            pauseResumeButton.FlatAppearance.BorderSize = 0;
            pauseResumeButton.FlatStyle = FlatStyle.Flat;
            pauseResumeButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            pauseResumeButton.ForeColor = Color.White;
            pauseResumeButton.Location = new Point(650, 115);
            pauseResumeButton.Name = "pauseResumeButton";
            pauseResumeButton.Size = new Size(80, 25);
            pauseResumeButton.TabIndex = 19;
            pauseResumeButton.Text = "⏸️ Duraklat";
            pauseResumeButton.UseVisualStyleBackColor = false;
            pauseResumeButton.Click += PauseResumeButton_Click;

            cancelButton.BackColor = Color.FromArgb(220, 53, 69);
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(740, 115);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(80, 25);
            cancelButton.TabIndex = 20;
            cancelButton.Text = "❌ İptal";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += CancelButton_Click;

            logTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextBox.BackColor = Color.FromArgb(33, 37, 41);
            logTextBox.BorderStyle = BorderStyle.FixedSingle;
            logTextBox.Font = new Font("Microsoft Sans Serif", 9F);
            logTextBox.ForeColor = Color.FromArgb(40, 167, 69);
            logTextBox.Location = new Point(25, 545);
            logTextBox.Margin = new Padding(3, 4, 3, 4);
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.Size = new Size(845, 155);
            logTextBox.TabIndex = 14;
            logTextBox.Text = "";

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 249, 250);
            ClientSize = new Size(894, 720);
            Controls.Add(logTextBox);
            Controls.Add(progressGroupBox);
            Controls.Add(statusLabel);
            Controls.Add(updateButton);
            Controls.Add(downloadButton);
            Controls.Add(browseButton);
            Controls.Add(pathTextBox);
            Controls.Add(pathLabel);
            Controls.Add(subtitleCheckBox);
            Controls.Add(qualityComboBox);
            Controls.Add(qualityLabel);

            Controls.Add(audioRadioButton);
            Controls.Add(videoRadioButton);
            Controls.Add(mediaTypeLabel);
            Controls.Add(urlTextBox);
            Controls.Add(urlLabel);
            Margin = new Padding(3, 4, 3, 4);
            MaximumSize = new Size(1200, 800);
            MinimumSize = new Size(683, 720);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Video İndirici - YouTube, Twitter, Instagram";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            progressGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label mediaTypeLabel;
        private System.Windows.Forms.RadioButton videoRadioButton;
        private System.Windows.Forms.RadioButton audioRadioButton;

        private System.Windows.Forms.Label qualityLabel;
        private System.Windows.Forms.ComboBox qualityComboBox;
        private System.Windows.Forms.CheckBox subtitleCheckBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.GroupBox progressGroupBox;
        private System.Windows.Forms.Label progressStatusLabel;
        private System.Windows.Forms.Label progressPercentageLabel;
        private System.Windows.Forms.Label currentFileLabel;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Label timeRemainingLabel;
        private System.Windows.Forms.Button pauseResumeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RichTextBox logTextBox;
    }
}