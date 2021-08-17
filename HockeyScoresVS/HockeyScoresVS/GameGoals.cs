using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HockeyScoresVS
{
    public class GameGoals : INotifyPropertyChanged
    {
        public ObservableCollection<Goal> FirstPeriodGoals { get; }
        public ObservableCollection<Goal> SecondPeriodGoals { get; }
        public ObservableCollection<Goal> ThirdPeriodGoals { get; }
        public ObservableCollection<Goal> OTGoals { get; }

        private readonly string seasonCode;
        private readonly string gameCode;

        public GameGoals(string seasonCode, string gameCode)
        {
            this.seasonCode = seasonCode;
            this.gameCode = gameCode;

            this.FirstPeriodGoals = new ObservableCollection<Goal>();
            this.SecondPeriodGoals = new ObservableCollection<Goal>();
            this.ThirdPeriodGoals = new ObservableCollection<Goal>();
            this.OTGoals = new ObservableCollection<Goal>();
        }

        public bool AnyGoalsScored
        {
            get
            {
                return this.FirstPeriodGoals.Any() || this.SecondPeriodGoals.Any() || this.ThirdPeriodGoals.Any() || this.OTGoals.Any();
            }
        }

        public void Clear()
        {
            this.FirstPeriodGoals.Clear();
            this.SecondPeriodGoals.Clear();
            this.ThirdPeriodGoals.Clear();
            this.OTGoals.Clear();
        }

        public void RefreshGoalSummary(List<IEnumerable<Goal>> updatedScoringSummary)
        {
            this.Clear();

            foreach (var goal in updatedScoringSummary[0])
            {
                this.FirstPeriodGoals.Add(goal);
            }

            foreach (var goal in updatedScoringSummary[1])
            {
                this.SecondPeriodGoals.Add(goal);
            }

            foreach (var goal in updatedScoringSummary[2])
            {
                this.ThirdPeriodGoals.Add(goal);
            }

            foreach (var goal in updatedScoringSummary[3])
            {
                this.OTGoals.Add(goal);
            }

            OnNotifyPropertyChanged("AnyGoalsScored");
        }

        public async Task GetUpdateScoringSummaryAsync()
        {
            IEnumerable<Goal> tempFirstPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempSecondPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempThirdPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempOTGoals = Enumerable.Empty<Goal>();

            JObject gameData = await NetworkCalls.ApiCallAsync($"http://live.nhl.com/GameData/{seasonCode}/{gameCode}/gc/gcbx.jsonp");

            var goals = gameData["goalSummary"].Values();

            List<Goal> tempGoalsList = new List<Goal>();

            foreach (var goalsList in goals)
            {
                try
                {
                    int period = goalsList.First().Value<int>();
                    tempGoalsList.Sort();

                    switch (period)
                    {
                        case 1:
                            tempFirstPeriodGoals = new List<Goal>(tempGoalsList);
                            break;
                        case 2:
                            tempSecondPeriodGoals = new List<Goal>(tempGoalsList);
                            break;
                        case 3:
                            tempThirdPeriodGoals = new List<Goal>(tempGoalsList);
                            break;
                        // Any overtime goals go here (including double, triple, etc.)
                        default:
                            tempOTGoals = new List<Goal>(tempGoalsList);
                            break;
                    }

                    tempGoalsList = new List<Goal>();
                    continue;
                }
                catch (Exception) { }

                foreach (var goal in goalsList.First())
                {
                    string goalString = "";
                    string team = "";
                    int? secondsInPeriod = null;

                    try
                    {
                        goalString = goal["desc"].Value<string>();
                        team = goal["t1"].Value<string>();
                        secondsInPeriod = goal["sip"].Value<int?>();
                    }
                    catch (Exception)
                    {
                        
                    }

                    tempGoalsList.Add(new Goal(team, goalString, secondsInPeriod));
                }
            }

            // Don't reverse OT, there can only ever be one goal there
            var list = new List<IEnumerable<Goal>>() { tempFirstPeriodGoals, tempSecondPeriodGoals, tempThirdPeriodGoals, tempOTGoals };
            this.RefreshGoalSummary(list);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
