
using System.Windows.Forms;

namespace HockeyScoresVS
{
    partial class ToolsOptionsUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.teamsComboBox = new System.Windows.Forms.ComboBox();
            this.favouriteTeamLabel = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // teamsComboBox
            // 
            this.teamsComboBox.Items.AddRange(new object[] {
            "",
            "TOR",
            "PIT",
            "ANA",
            "ARI",
            "BOS",
            "BUF",
            "CAR",
            "CBJ",
            "COL",
            "CGY",
            "CHI",
            "DAL",
            "DET",
            "EDM",
            "FLA",
            "LAK",
            "MIN",
            "MTL",
            "OTT",
            "NJD",
            "NSH",
            "NYR",
            "NYI",
            "PHI",
            "SJS",
            "STL",
            "TBL",
            "VAN",
            "WPG",
            "WSH"});
            this.teamsComboBox.Location = new System.Drawing.Point(112, 24);
            this.teamsComboBox.Name = "teamsComboBox";
            this.teamsComboBox.Size = new System.Drawing.Size(150, 21);
            this.teamsComboBox.TabIndex = 0;
            this.teamsComboBox.SelectedIndexChanged += new System.EventHandler(this.teamsComboBox_SelectedIndexChanged);
            // 
            // favouriteTeamLabel
            // 
            this.favouriteTeamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.favouriteTeamLabel.Location = new System.Drawing.Point(6, 27);
            this.favouriteTeamLabel.Name = "favouriteTeamLabel";
            this.favouriteTeamLabel.Size = new System.Drawing.Size(100, 23);
            this.favouriteTeamLabel.TabIndex = 1;
            this.favouriteTeamLabel.Text = "Favourite Team: ";
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.favouriteTeamLabel);
            this.groupBox.Controls.Add(this.teamsComboBox);
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(391, 68);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "NHL Scores Settings";
            // 
            // ToolsOptionsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "ToolsOptionsUserControl";
            this.Size = new System.Drawing.Size(401, 77);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox teamsComboBox;
        private Label favouriteTeamLabel;
        private GroupBox groupBox;
    }
}
