using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public class GameGoals
    {
        public ObservableCollection<Goal> FirstPeriodGoals { get; }
        public ObservableCollection<Goal> SecondPeriodGoals { get; }
        public ObservableCollection<Goal> ThirdPeriodGoals { get; }
        public ObservableCollection<Goal> OTGoals { get; }

        public GameGoals()
        {
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
    }
}
