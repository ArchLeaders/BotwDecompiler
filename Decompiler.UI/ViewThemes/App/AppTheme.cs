#pragma warning disable CS8604

using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using System.Windows.Media;

namespace Decompiler.UI.ViewResources.Helpers
{
    public static class AppTheme
    {
        private static Color Color(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }

        public static string ThemeFile { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Apps\\Decompiler.UI.theme";
        public static string ThemeStr { get; set; } = $"Dark";
        public static void Change(bool toLight = false)
        {
            PaletteHelper helper = new();
            ITheme theme = helper.GetTheme();

            if (toLight)
            {
                ThemeStr = "Light";
                Directory.CreateDirectory(new FileInfo(ThemeFile).DirectoryName);
                File.WriteAllText($"{ThemeFile}", string.Empty);

                theme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Light);

                theme.PrimaryDark = "#fff".Color();
                theme.PrimaryMid = "#f7f7f7".Color();
                theme.PrimaryLight = "#25000000".Color();

                theme.SecondaryDark = "#657160E8".Color();
                theme.SecondaryMid = "#7160E8".Color();
                theme.SecondaryLight = "#8A80FA".Color();

                theme.Selection = "#7160E8".Color();
                theme.Paper = "#D9D9D9".Color();
                theme.Body = "#1f1f1f".Color();
                theme.FlatButtonRipple = "#A32215".Color();
                theme.Background = "#fff".Color();
            }
            else
            {
                ThemeStr = "Dark";

                if (File.Exists(ThemeFile)) File.Delete($"{ThemeFile}");

                theme.SetBaseTheme(MaterialDesignThemes.Wpf.Theme.Dark);

                theme.PrimaryDark = "#1F1F1F".Color();
                theme.PrimaryMid = "#414141".Color();
                theme.PrimaryLight = "#15ffffff".Color();

                theme.SecondaryDark = "#201B42".Color();
                theme.SecondaryMid = "#423887".Color();
                theme.SecondaryLight = "#7160E8".Color();

                theme.Selection = "#7160E8".Color();
                theme.Paper = "#121212".Color();
                theme.Body = "#B0B0B0".Color();
                theme.FlatButtonRipple = "#E04343".Color();
                theme.Background = "#0A0A0A".Color();
            }

            helper.SetTheme(theme);
        }
    }
}
