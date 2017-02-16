//------------------------------------------------------------------------------
// <copyright file="ScoresToolWindowControl.xaml.cs" company="HP Inc.">
//     Copyright (c) HP Inc..  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace HockeyScoresVS
{
    using System.Collections;
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
            this.DataContext = CurrentGames = new TodayGames(ScoresToolWindowCommand.Instance.FavouriteTeam);
        }

        public void Dispose()
        {
            this.CurrentGames.Dispose();
        }

        public void SetFavouriteTeam(string favouriteTeam)
        {
            CurrentGames.FavouriteTeam = favouriteTeam;
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                HockeyGame deselectedGame = e.RemovedItems[0] as HockeyGame;
                deselectedGame.IsSelected = false;
            }

            if (e.AddedItems.Count == 1)
            {
                HockeyGame selectedGame = e.AddedItems[0] as HockeyGame;
                selectedGame.IsSelected = true;

                ListBox listBox = e.OriginalSource as ListBox;
                if (listBox.SelectedItems != null)
                {
                    var valid = e.AddedItems[0];
                    foreach (HockeyGame item in new ArrayList(listBox.SelectedItems))
                    {
                        if (item != valid)
                        {
                            item.IsSelected = false;
                            listBox.SelectedItems.Remove(item);
                        }
                    }
                }
            }
        }
    }
}