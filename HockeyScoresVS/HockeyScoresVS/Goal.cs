﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Goal : INotifyPropertyChanged
    {
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

        public Goal(string team, string goalString)
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
        }

        public Goal(string team, string scoredBy, string primaryAssist, string secondaryAssist)
        {
            this.Team = team;
            this.ScoredBy = scoredBy;
            this.PrimaryAssist = primaryAssist;
            this.SecondaryAssist = secondaryAssist;
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
