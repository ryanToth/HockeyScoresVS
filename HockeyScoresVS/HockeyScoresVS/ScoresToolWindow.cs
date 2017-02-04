//------------------------------------------------------------------------------
// <copyright file="ScoresToolWindow.cs" company="HP Inc.">
//     Copyright (c) HP Inc..  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace HockeyScoresVS
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("7a570ff0-5a8a-41bf-929d-19a83737b7c4")]
    public class ScoresToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresToolWindow"/> class.
        /// </summary>
        public ScoresToolWindow() : base(null)
        {
            this.Caption = "NHL Scores";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ScoresToolWindowControl();
        }
    }
}
