using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class Goal : INotifyPropertyChanged
    {
        private Player _scoredBy;
        public Player ScoredBy
        {
            get
            {
                return _scoredBy;
            }
            set
            {
                _scoredBy = value;
                OnNotifyPropertyChanged("ScoredBy");
            }
        }

        private Player _primaryAssist;
        public Player PrimaryAssist
        {
            get
            {
                return _primaryAssist;
            }
            set
            {
                _primaryAssist = value;
                OnNotifyPropertyChanged("PrimaryAssist");
            }
        }

        private Player _secondaryAssist;
        public Player SecondaryAssist
        {
            get
            {
                return _secondaryAssist;
            }
            set
            {
                _secondaryAssist = value;
                OnNotifyPropertyChanged("SecondaryAssist");
            }
        }

        public string GoalScoredText
        {
            get
            {
                return "Goal Scored";
            }
        }

        public string AssistedByText
        {
            get
            {
                return "Assisted by people and stuff";
            }
        }

        public Goal()
        {
        }

        public Goal(Player scoredBy, Player primaryAssist, Player secondaryAssist)
        {
            this._scoredBy = scoredBy;
            this._primaryAssist = primaryAssist;
            this._secondaryAssist = secondaryAssist;
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
