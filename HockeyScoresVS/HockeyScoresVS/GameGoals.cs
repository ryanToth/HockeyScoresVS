using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class GameGoals : INotifyPropertyChanged
    {
        public ObservableCollection<Goal> FirstPeriodGoals { get; }
        public ObservableCollection<Goal> SecondPeriodGoals { get; }
        public ObservableCollection<Goal> ThirdPeriodGoals { get; }
        public ObservableCollection<Goal> OTGoals { get; }

        private string _seasonCode;
        private string _gameCode;

        public GameGoals(string seasonCode, string gameCode)
        {
            this._seasonCode = seasonCode;
            this._gameCode = gameCode;

            FirstPeriodGoals = new ObservableCollection<Goal>();
            SecondPeriodGoals = new ObservableCollection<Goal>();
            ThirdPeriodGoals = new ObservableCollection<Goal>();
            OTGoals = new ObservableCollection<Goal>();
        }

        public bool AnyGoalsScored
        {
            get
            {
                return FirstPeriodGoals.Any() || SecondPeriodGoals.Any() || ThirdPeriodGoals.Any() || OTGoals.Any();
            }
        }

        public void Clear()
        {
            FirstPeriodGoals.Clear();
            SecondPeriodGoals.Clear();
            ThirdPeriodGoals.Clear();
            OTGoals.Clear();
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

        public async Task GetUpdateScoringSummary()
        {
            IEnumerable<Goal> tempFirstPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempSecondPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempThirdPeriodGoals = Enumerable.Empty<Goal>();
            IEnumerable<Goal> tempOTGoals = Enumerable.Empty<Goal>();

            JObject gameData = await NetworkCalls.ApiCallAsync($"http://live.nhl.com/GameData/{_seasonCode}/{_gameCode}/gc/gcbx.jsonp");

            var goals = gameData["goalSummary"].Values();

            List<Goal> tempGoalsList = new List<Goal>();

            foreach (var goalsList in goals)
            {
                try
                {
                    int period = goalsList.First().Value<int>();

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
                        case 4:
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

                    try
                    {
                        goalString = goal["desc"].Value<string>();
                        team = goal["t1"].Value<string>();
                    }
                    catch (Exception)
                    {
                        
                    }

                    tempGoalsList.Add(new Goal(team, goalString));
                }
            }

            // Don't reverse OT, there can only ever be one goal there
            var list = new List<IEnumerable<Goal>>() { tempFirstPeriodGoals.Reverse(), tempSecondPeriodGoals.Reverse(), tempThirdPeriodGoals.Reverse(), tempOTGoals };
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
