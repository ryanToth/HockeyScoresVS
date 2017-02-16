//------------------------------------------------------------------------------
// <copyright file="ScoresToolWindowPackage.cs" company="HP Inc.">
//     Copyright (c) HP Inc..  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Windows.Forms;
using System.ComponentModel;

namespace HockeyScoresVS
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ScoresToolWindow), Width = 225)]
    [Guid(ScoresToolWindowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionsPageGrid),
        "NHL Scores", "Favourite Team", 0, 0, true)]
    public sealed class ScoresToolWindowPackage : Package
    {
        /// <summary>
        /// ScoresToolWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "345d4dad-3337-412f-a55a-edf2babbbda6";

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresToolWindow"/> class.
        /// </summary>
        public ScoresToolWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ScoresToolWindowCommand.Initialize(this);
            base.Initialize();

            ScoresToolWindowCommand.Instance.FavouriteTeam = this.FavouriteTeam;

            OptionsPageGrid page = (OptionsPageGrid)GetDialogPage(typeof(OptionsPageGrid));
            page.PropertyChanged += FavouriteTeam_Changed;
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
        private string _favouriteTeam = "";

        [Category("NHL Scores")]
        [DisplayName("Favourite Team")]
        [Description("Your favourite NHL team will always be at the top of the games list")]
        public string FavouriteTeam
        {
            get
            {
                return _favouriteTeam;
            }
            set
            {
                if (_favouriteTeam != value)
                {
                    _favouriteTeam = value;
                    OnNotifyPropertyChanged("FavouriteTeam");
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
