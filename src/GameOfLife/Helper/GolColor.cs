using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace GameOfLife
{
    public static class GolColor
    {

        public static Color Dead = Color.FromArgb(255, 210, 210, 210);

        public static Color Next = Color.FromArgb(255, 190, 190, 190);

        public static Color Stable = Color.FromArgb(175, 255, 0, 0);

        public static Color Life = Color.FromArgb(255, 255, 0, 0);

    }

    public static class GolBrush
    {
        public static bool RandomColors = false;
        public static readonly Brush Dead = new SolidColorBrush(GolColor.Dead);

        public static readonly Brush Next = new SolidColorBrush(GolColor.Next);

        public static readonly Brush Stable = new SolidColorBrush(GolColor.Stable);

        private static Windows.UI.Color[] colors = new Windows.UI.Color[] { Windows.UI.Colors.DodgerBlue, Windows.UI.Colors.Orange, Windows.UI.Colors.Gray, Windows.UI.Colors.Tomato, Windows.UI.Colors.Violet, Windows.UI.Colors.DarkGreen, Windows.UI.Colors.CornflowerBlue, Windows.UI.Colors.RoyalBlue, Windows.UI.Colors.DodgerBlue, Windows.UI.Colors.Orange, Windows.UI.Colors.Gray, Windows.UI.Colors.Tomato, Windows.UI.Colors.Violet, Windows.UI.Colors.DarkGreen, Windows.UI.Colors.CornflowerBlue, Windows.UI.Colors.RoyalBlue, Windows.UI.Colors.DodgerBlue, Windows.UI.Colors.Orange, Windows.UI.Colors.Gray, Windows.UI.Colors.Tomato, Windows.UI.Colors.Violet, Windows.UI.Colors.DarkGreen, Windows.UI.Colors.CornflowerBlue, Windows.UI.Colors.RoyalBlue, Windows.UI.Colors.DodgerBlue, Windows.UI.Colors.Orange, Windows.UI.Colors.Gray, Windows.UI.Colors.Tomato, Windows.UI.Colors.Violet, Windows.UI.Colors.DarkGreen, Windows.UI.Colors.CornflowerBlue, Windows.UI.Colors.RoyalBlue };

        private static Random random = new Random();
        public static Brush Life
        {
            get
            {
                if (RandomColors)
                {
                    try
                    {
                        var c = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                        return new SolidColorBrush(c);
                    }
                    catch (Exception ex)
                    {
                        return new SolidColorBrush(GolColor.Life);
                    }
                }
                else
                {
                    return new SolidColorBrush(GolColor.Life);
                }
            }
        }
        public static Brush LifeCheck
        {
            get
            {
                return new SolidColorBrush(GolColor.Life);
            }
        }
    }
}