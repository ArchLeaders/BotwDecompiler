#pragma warning disable CS8600
#pragma warning disable CS8601

using MaterialDesignThemes.Wpf;
using Stylet;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Formatting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Decompiler.UI.ViewModels
{
    public class MessageViewModel : Screen, INotifyPropertyChanged
    {
        #region Actions

        public void Yes() => RequestClose(false);

        public void No() => RequestClose(true);

        public void Copy() => Clipboard.SetText(MessageStr);

        private static ITheme GetTheme()
        {
            PaletteHelper helper = new();
            return helper.GetTheme();
        }

        #endregion

        #region Props

        private string _title = "Notice";
        public string Title
        {
            get => _title;
            set => SetAndNotify(ref _title, value);
        }

        private TextBlock _message = new() { Text = "No details were provided." };
        public string MessageStr { get; set; } = "";
        public TextBlock Message
        {
            get => _message;
            set => SetAndNotify(ref _message, value);
        }

        private Brush _foreground = new SolidColorBrush(GetTheme().Body);
        public Brush Foreground
        {
            get => _foreground;
            set => SetAndNotify(ref _foreground, value);
        }

        private string _buttonRight = "Ok";
        public string ButtonRight
        {
            get { return _buttonRight; }
            set => SetAndNotify(ref _buttonRight, value);
        }

        private string _buttonLeft = "Yes";
        public string ButtonLeft
        {
            get { return _buttonLeft; }
            set => SetAndNotify(ref _buttonLeft, value);
        }

        private Visibility _buttonLeftVisibility = Visibility.Collapsed;
        public Visibility ButtonLeftVisibility
        {
            get => _buttonLeftVisibility;
            set => SetAndNotify(ref _buttonLeftVisibility, value);
        }

        private double _width = 220;
        public double Width
        {
            get { return _width; }
            set => SetAndNotify(ref _width, value);
        }

        #endregion

        public MessageViewModel(string message, string title = "Notice", bool isOption = false, string? messageColor = null, double width = 220,
            string yesButtonText = "Yes", string noButtonText = "Auto")
        {
            MessageStr = $"**{title}**\n> {message}";
            Message = message.ToTextBlock();
            Title = title;
            Width = width;
            ButtonRight = noButtonText == "Auto" ? "Ok" : noButtonText;
            ButtonLeft = yesButtonText;

            if (isOption)
            {
                ButtonLeftVisibility = Visibility.Visible;
                ButtonRight = noButtonText == "Auto" ? "No" : noButtonText;
            }

            if (messageColor != null)
                Foreground = (Brush)new BrushConverter().ConvertFromString(messageColor);
        }
    }
}
