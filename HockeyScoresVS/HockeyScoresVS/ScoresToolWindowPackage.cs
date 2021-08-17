using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace HockeyScoresVS
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ScoresToolWindow), Width = 225)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(OptionsPageGrid), "NHL Scores", "Favourite Team", 0, 0, true)]
    public sealed class ScoresToolWindowPackage : AsyncPackage
    {
        public const string PackageGuidString = "345d4dad-3337-412f-a55a-edf2babbbda6";

        public ScoresToolWindowPackage()
        {
        }

        #region Package Members

        protected async override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            ScoresToolWindowCommand.Initialize(this);
            await base.InitializeAsync(cancellationToken, progress);

            ScoresToolWindowCommand.Instance.FavouriteTeam = this.FavouriteTeam;

            OptionsPageGrid page = (OptionsPageGrid)GetDialogPage(typeof(OptionsPageGrid));
            page.PropertyChanged += this.FavouriteTeam_Changed;
        }

        public string FavouriteTeam
        {
            get
            {
                OptionsPageGrid page = (OptionsPageGrid)GetDialogPage(typeof(OptionsPageGrid));
                return page.FavouriteTeam;
            }
        }

        private void FavouriteTeam_Changed(object sender, PropertyChangedEventArgs e)
        {
            ScoresToolWindowCommand.Instance.FavouriteTeam = this.FavouriteTeam;
        }
        #endregion
    }

    [Guid("6186abe0-05e7-4361-bca1-bc48bcab6771")]
    public class OptionsPageGrid : DialogPage, INotifyPropertyChanged
    {
        private string favouriteTeam = string.Empty;

        [Category("NHL Scores")]
        [DisplayName("Favourite Team")]
        [Description("Your favourite NHL team will always be at the top of the games list")]
        public string FavouriteTeam
        {
            get
            {
                return this.favouriteTeam;
            }
            set
            {
                if (this.favouriteTeam != value)
                {
                    this.favouriteTeam = value;
                    this.OnNotifyPropertyChanged("FavouriteTeam");
                }
            }
        }

        protected override IWin32Window Window
        {
            get
            {
                ToolsOptionsUserControl page = new ToolsOptionsUserControl();
                page.optionsPage = this;
                page.Initialize();
                return page;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
