using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Goal : INotifyPropertyChanged, IComparable<Goal>
    {
        private int? _goalTime;

        private string _scoredBy = "";
        public string ScoredBy
        {
            get
            {
                return _scoredBy;
            }
            set
            {
                _scoredBy = value;
                OnNotifyPropertyChanged("GoalScoredText");
            }
        }

        private string _primaryAssist = "";
        public string PrimaryAssist
        {
            get
            {
                return _primaryAssist;
            }
            set
            {
                _primaryAssist = value;
                OnNotifyPropertyChanged("AssistedByText");
            }
        }

        private string _secondaryAssist = "";
        public string SecondaryAssist
        {
            get
            {
                return _secondaryAssist;
            }
            set
            {
                _secondaryAssist = value;
                OnNotifyPropertyChanged("AssistedByText");
            }
        }

        private string _team = "";
        public string Team
        {
            get
            {
                return _team;
            }
            set
            {
                _team = value;
                OnNotifyPropertyChanged("GoalScoredText");
            }
        }

        public string GoalScoredText
        {
            get
            {
                if (Team != "" && ScoredBy != "")
                {
                    return $"{Team} {ScoredBy}";
                }
                else if (Team != "")
                {
                    return $"{Team} scored a goal";
                }

                return "Goal was scored";
                
            }
        }

        public string AssistedByText
        {
            get
            {
                if (PrimaryAssist == "")
                {
                    return "Unassisted";
                }
                else if (SecondaryAssist == "")
                {
                    return $"{PrimaryAssist}";
                }
                
                return $"{PrimaryAssist}, {SecondaryAssist}";
            }
        }

        public Goal()
        {
        }

        public Goal(string team, string goalString, int? secondsInPeriod)
        {
            this.Team = team;
            try
            {
                this.ScoredBy = goalString.Split(',')[0];
            }
            catch (Exception) { }
            try
            {
                this.PrimaryAssist = goalString.Split(',')[1];
            }
            catch (Exception) { }
            try
            {
                this.SecondaryAssist = goalString.Split(',')[2];
            }
            catch (Exception) { }
            this._goalTime = secondsInPeriod;
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

        #region IComparable Members
        public int CompareTo(Goal other)
        {
            if (this._goalTime.HasValue && !other._goalTime.HasValue) return -1;
            else if (!this._goalTime.HasValue && other._goalTime.HasValue) return 1;
            else if (this._goalTime.HasValue && other._goalTime.HasValue)
            {
                if (this._goalTime.Value < other._goalTime.Value) return -1;
                else if (this._goalTime.Value > other._goalTime.Value) return 1;
            }

            return 0;
        }

        #endregion
    }
}
