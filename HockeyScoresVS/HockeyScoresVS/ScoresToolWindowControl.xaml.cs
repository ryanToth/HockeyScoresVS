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
        public TodayGames CurrentGames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoresToolWindowControl"/> class.
        /// </summary>
        public ScoresToolWindowControl()
        {
            this.InitializeComponent();
            InitializeContent();
        }

        public void InitializeContent()
        {
            this.DataContext = CurrentGames = new TodayGames();
        }

        public void Dispose()
        {
            this.CurrentGames.Dispose();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                // Nothing
            }
            else
            {
                if (CurrentGames != null)
                {
                    CurrentGames.ChangeGameDay(date.Value);
                }
            }
        }
    }
}