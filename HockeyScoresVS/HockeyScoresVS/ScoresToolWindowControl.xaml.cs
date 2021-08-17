using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace HockeyScoresVS
{
    /// <summary>
    /// Interaction logic for ScoresToolWindowControl.
    /// </summary>
    public partial class ScoresToolWindowControl : UserControl
    {
        public TodayGames CurrentGames;

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
            this.CurrentGames.FavouriteTeam = favouriteTeam;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date != null)
            {
                if (CurrentGames != null && CurrentGames.CurrentGamesDate != date.Value)
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

        private void Button_Click_Tomorrow(object sender, RoutedEventArgs e)
        {
            if (this.DatePicker != null && this.DatePicker.SelectedDate.HasValue)
            {
                var date = this.DatePicker.SelectedDate.Value;
                this.DatePicker.SelectedDate = date.AddDays(1);
            }
        }

        private void Button_Click_Yesterday(object sender, RoutedEventArgs e)
        {
            if (this.DatePicker != null && this.DatePicker.SelectedDate.HasValue)
            {
                var date = this.DatePicker.SelectedDate.Value;
                this.DatePicker.SelectedDate = date.AddDays(-1);
            }
        }
    }
}