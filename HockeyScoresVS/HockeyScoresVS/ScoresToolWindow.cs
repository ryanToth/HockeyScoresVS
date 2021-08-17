namespace HockeyScoresVS
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [Guid("7a570ff0-5a8a-41bf-929d-19a83737b7c4")]
    public class ScoresToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresToolWindow"/> class.
        /// </summary>
        public ScoresToolWindow() : base(null)
        {
            this.Caption = "NHL Scores";
            this.Content = new ScoresToolWindowControl();
        }

        public void SetFavouriteTeam(string favouriteTeam)
        {
            (this.Content as ScoresToolWindowControl).SetFavouriteTeam(favouriteTeam);
        }
    }
}
