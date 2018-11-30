using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeyScoresVS
{
    public static class Converters
    {
        public static string VSColorThemeConverter(ThemeResourceKey color)
        {
            return $"#{VSColorTheme.GetThemedColor(color).ToArgb().ToString("X")}";
        }

        public static string TeamNameConverter(string abrv)
        {
            if (abrv == null)
            {
                return "";
            }

            switch (abrv)
            {
                case "TOR": return "Toronto";
                case "PIT": return "Pittsburgh";
                case "ANA": return "Anaheim";
                case "ARI": return "Arizona";
                case "BOS": return "Boston";
                case "BUF": return "Buffalo";
                case "CAR": return "Carolina";
                case "CBJ": return "Colombus";
                case "COL": return "Colorado";
                case "CGY": return "Calgary";
                case "CHI": return "Chicago";
                case "DAL": return "Dallas";
                case "DET": return "Detroit";
                case "EDM": return "Edmonton";
                case "FLA": return "Florida";
                case "LAK": return "Los Angeles";
                case "MIN": return "Minnesota";
                case "MTL": return "Montreal";
                case "OTT": return "Ottawa";
                case "NJD": return "New Jersey";
                case "NSH": return "Nashville";
                case "NYR": return "NY Rangers";
                case "NYI": return "NY Islanders";
                case "PHI": return "Philedalphia";
                case "SJS": return "San Jose";
                case "STL": return "St Louis";
                case "TBL": return "Tampa Bay";
                case "VAN": return "Vancouver";
                case "VGK": return "Las Vegas";
                case "WPG": return "Winnipeg";
                case "WSH": return "Washington";
            }


            return abrv;
        }
    }
}
