using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace HockeyScoresVS
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ScoresToolWindowCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("c358ea8a-ad21-4c5e-a18d-76520bbca927");

        private readonly Package package;

        private ScoresToolWindowCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        private string favouriteTeam;
        public string FavouriteTeam
        {
            get
            {
                return this.favouriteTeam;
            }

            set
            {
                this.favouriteTeam = value;
                ScoresToolWindow window = this.package.FindToolWindow(typeof(ScoresToolWindow), 0, false) as ScoresToolWindow;
                if (window != null)
                {
                    window.SetFavouriteTeam(favouriteTeam);
                }
            }
        }

        public static ScoresToolWindowCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new ScoresToolWindowCommand(package);
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ToolWindowPane window = this.package.FindToolWindow(typeof(ScoresToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            ScoresToolWindow scoresToolWindow = this.package.FindToolWindow(typeof(ScoresToolWindow), 0, false) as ScoresToolWindow;
            if (window != null)
            {
                scoresToolWindow.SetFavouriteTeam(favouriteTeam);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
