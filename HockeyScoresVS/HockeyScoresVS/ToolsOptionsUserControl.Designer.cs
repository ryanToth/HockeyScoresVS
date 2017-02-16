
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
            
            this.teamsComboBox.Location = new System.Drawing.Point(108, 3);
            this.teamsComboBox.Name = "teamsComboBox";
            this.teamsComboBox.Size = new System.Drawing.Size(150, 21);
            this.teamsComboBox.TabIndex = 0;
            this.teamsComboBox.SelectedIndexChanged += new System.EventHandler(this.teamsComboBox_SelectedIndexChanged);
            // 
            // favouriteTeamLabel
            // 
            this.favouriteTeamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.favouriteTeamLabel.Location = new System.Drawing.Point(3, 6);
            this.favouriteTeamLabel.Name = "favouriteTeamLabel";
            this.favouriteTeamLabel.Size = new System.Drawing.Size(100, 23);
            this.favouriteTeamLabel.TabIndex = 1;
            this.favouriteTeamLabel.Text = "Favourite Team: ";
            // 
            // ToolsOptionsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.teamsComboBox);
            this.Controls.Add(this.favouriteTeamLabel);
            this.Name = "ToolsOptionsUserControl";
            this.Size = new System.Drawing.Size(266, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox teamsComboBox;
        private Label favouriteTeamLabel;
    }
}
