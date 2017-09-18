using Microsoft.VisualStudio.PlatformUI;
using System.Windows.Media;

namespace HockeyScoresVS
{
    public static class DefaultColors
    {
        public static string ToolWindowBackground = Converters.VSColorThemeConverter(EnvironmentColors.LightColorKey);
        public static string DefaultBackgroundColor = Converters.VSColorThemeConverter(EnvironmentColors.DarkColorKey);

        public static Brush DefaultTextColor
        {
            get
            {
                string hex = Converters.VSColorThemeConverter(EnvironmentColors.BrandedUITextColorKey);
                return (Brush)new BrushConverter().ConvertFromString(hex);
            }
        }
    }
}
