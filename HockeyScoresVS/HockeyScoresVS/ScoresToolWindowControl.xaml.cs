//------------------------------------------------------------------------------
// <copyright file="ScoresToolWindowControl.xaml.cs" company="HP Inc.">
//     Copyright (c) HP Inc..  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace HockeyScoresVS
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ScoresToolWindowControl.
    /// </summary>
    public partial class ScoresToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresToolWindowControl"/> class.
        /// </summary>
        public ScoresToolWindowControl()
        {
            this.InitializeComponent();
            this.MaxWidth = 265;
            this.MinWidth = 265;
        }
    }
}