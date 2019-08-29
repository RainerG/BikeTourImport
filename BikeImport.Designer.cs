namespace BikeTourImport
{
    partial class BikeImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BikeImport));
            this.btnImpBrwse = new System.Windows.Forms.Button();
            this.btnTourEdit = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExpBrwse = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCalc = new System.Windows.Forms.Button();
            this.userRichTextBox = new NS_Utilities.UserRichTextBox();
            this.fileCmbExport = new NS_UserCombo.FileComboBox();
            this.fileCmbTour = new NS_UserCombo.FileComboBox();
            this.userCmbTitle = new NS_UserCombo.UserComboBox();
            this.btnEditExport = new System.Windows.Forms.Button();
            this.userCmbOdo = new NS_UserCombo.UserComboBox();
            this.btnRenFile = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImpBrwse
            // 
            resources.ApplyResources(this.btnImpBrwse, "btnImpBrwse");
            this.btnImpBrwse.Name = "btnImpBrwse";
            this.btnImpBrwse.UseVisualStyleBackColor = true;
            this.btnImpBrwse.Click += new System.EventHandler(this.btnTourBrwse_Click);
            // 
            // btnTourEdit
            // 
            resources.ApplyResources(this.btnTourEdit, "btnTourEdit");
            this.btnTourEdit.Name = "btnTourEdit";
            this.btnTourEdit.UseVisualStyleBackColor = true;
            this.btnTourEdit.Click += new System.EventHandler(this.btnTourEdit_Click);
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExpBrwse
            // 
            resources.ApplyResources(this.btnExpBrwse, "btnExpBrwse");
            this.btnExpBrwse.Name = "btnExpBrwse";
            this.btnExpBrwse.UseVisualStyleBackColor = true;
            this.btnExpBrwse.Click += new System.EventHandler(this.btnExpBrwse_Click);
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCalc
            // 
            resources.ApplyResources(this.btnCalc, "btnCalc");
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // userRichTextBox
            // 
            resources.ApplyResources(this.userRichTextBox, "userRichTextBox");
            this.userRichTextBox.Name = "userRichTextBox";
            // 
            // fileCmbExport
            // 
            this.fileCmbExport.AllowDrop = true;
            resources.ApplyResources(this.fileCmbExport, "fileCmbExport");
            this.fileCmbExport.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.fileCmbExport.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.fileCmbExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fileCmbExport.FormattingEnabled = true;
            this.fileCmbExport.Name = "fileCmbExport";
            this.fileCmbExport.ReadOnly = false;
            // 
            // fileCmbTour
            // 
            this.fileCmbTour.AllowDrop = true;
            resources.ApplyResources(this.fileCmbTour, "fileCmbTour");
            this.fileCmbTour.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.fileCmbTour.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.fileCmbTour.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fileCmbTour.FormattingEnabled = true;
            this.fileCmbTour.Name = "fileCmbTour";
            this.fileCmbTour.ReadOnly = false;
            // 
            // userCmbTitle
            // 
            resources.ApplyResources(this.userCmbTitle, "userCmbTitle");
            this.userCmbTitle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.userCmbTitle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.userCmbTitle.FormattingEnabled = true;
            this.userCmbTitle.Name = "userCmbTitle";
            this.userCmbTitle.ReadOnly = false;
            // 
            // btnEditExport
            // 
            resources.ApplyResources(this.btnEditExport, "btnEditExport");
            this.btnEditExport.Name = "btnEditExport";
            this.btnEditExport.UseVisualStyleBackColor = true;
            this.btnEditExport.Click += new System.EventHandler(this.btnEditExport_Click);
            // 
            // userCmbOdo
            // 
            resources.ApplyResources(this.userCmbOdo, "userCmbOdo");
            this.userCmbOdo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.userCmbOdo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.userCmbOdo.FormattingEnabled = true;
            this.userCmbOdo.Name = "userCmbOdo";
            this.userCmbOdo.ReadOnly = false;
            // 
            // btnRenFile
            // 
            resources.ApplyResources(this.btnRenFile, "btnRenFile");
            this.btnRenFile.Name = "btnRenFile";
            this.btnRenFile.UseVisualStyleBackColor = true;
            this.btnRenFile.Click += new System.EventHandler(this.btnRenFile_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // BikeImport
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRenFile);
            this.Controls.Add(this.userCmbOdo);
            this.Controls.Add(this.btnEditExport);
            this.Controls.Add(this.userCmbTitle);
            this.Controls.Add(this.userRichTextBox);
            this.Controls.Add(this.btnCalc);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnExpBrwse);
            this.Controls.Add(this.fileCmbExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnTourEdit);
            this.Controls.Add(this.btnImpBrwse);
            this.Controls.Add(this.fileCmbTour);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "BikeImport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BikeImport_FormClosing);
            this.Load += new System.EventHandler(this.BikeImport_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NS_UserCombo.FileComboBox fileCmbTour;
        private System.Windows.Forms.Button btnImpBrwse;
        private System.Windows.Forms.Button btnTourEdit;
        private System.Windows.Forms.Button btnImport;
        private NS_UserCombo.FileComboBox fileCmbExport;
        private System.Windows.Forms.Button btnExpBrwse;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCalc;
        private NS_Utilities.UserRichTextBox userRichTextBox;
        private NS_UserCombo.UserComboBox userCmbTitle;
        private System.Windows.Forms.Button btnEditExport;
        private NS_UserCombo.UserComboBox userCmbOdo;
        private System.Windows.Forms.Button btnRenFile;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

