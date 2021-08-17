using System;
using System.ComponentModel;

namespace HockeyScoresVS
{
    public class Goal : INotifyPropertyChanged, IComparable<Goal>
    {
        private int? goalTime;

        private string scoredBy = string.Empty;
        public string ScoredBy
        {
            get
            {
                return scoredBy;
            }
            set
            {
                scoredBy = value;
                OnNotifyPropertyChanged("GoalScoredText");
            }
        }

        private string primaryAssist = string.Empty;
        public string PrimaryAssist
        {
            get
            {
                return this.primaryAssist;
            }
            set
            {
                this.primaryAssist = value;
                OnNotifyPropertyChanged("AssistedByText");
            }
        }

        private string secondaryAssist = string.Empty;
        public string SecondaryAssist
        {
            get
            {
                return this.secondaryAssist;
            }
            set
            {
                this.secondaryAssist = value;
                OnNotifyPropertyChanged("AssistedByText");
            }
        }

        private string team = string.Empty;
        public string Team
        {
            get
            {
                return this.team;
            }
            set
            {
                this.team = value;
                OnNotifyPropertyChanged("GoalScoredText");
            }
        }

        public string GoalScoredText
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Team) && !string.IsNullOrEmpty(this.ScoredBy))
                {
                    return $"{this.Team} {this.ScoredBy}";
                }
                else if (!string.IsNullOrEmpty(this.Team))
                {
                    return $"{this.Team} scored a goal";
                }

                return "Goal was scored";
                
            }
        }

        public string AssistedByText
        {
            get
            {
                if (string.IsNullOrEmpty(this.PrimaryAssist))
                {
                    return "Unassisted";
                }
                else if (string.IsNullOrEmpty(this.SecondaryAssist))
                {
                    return $"{this.PrimaryAssist}";
                }
                
                return $"{this.PrimaryAssist}, {this.SecondaryAssist}";
            }
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

            this.goalTime = secondsInPeriod;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IComparable Members
        public int CompareTo(Goal other)
        {
            if (this.goalTime.HasValue && !other.goalTime.HasValue) return -1;
            else if (!this.goalTime.HasValue && other.goalTime.HasValue) return 1;
            else if (this.goalTime.HasValue && other.goalTime.HasValue)
            {
                if (this.goalTime.Value < other.goalTime.Value) return -1;
                else if (this.goalTime.Value > other.goalTime.Value) return 1;
            }

            return 0;
        }

        #endregion
    }
}
