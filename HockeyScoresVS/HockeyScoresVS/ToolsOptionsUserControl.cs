using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HockeyScoresVS
{
    public partial class ToolsOptionsUserControl : UserControl
    {
        public ToolsOptionsUserControl()
        {
            InitializeComponent();
        }

        internal OptionsPageGrid optionsPage;

        public void Initialize()
        {
            this.teamsComboBox.SelectedItem = optionsPage.FavouriteTeam;
        }

        private void teamsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            optionsPage.FavouriteTeam = this.teamsComboBox.SelectedItem as String;
        }
    }
}
