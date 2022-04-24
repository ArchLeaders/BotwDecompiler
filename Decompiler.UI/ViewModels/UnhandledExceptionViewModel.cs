#pragma warning disable CS8600
#pragma warning disable CS8601

using MaterialDesignThemes.Wpf;
using Stylet;
using System.Text.Formatting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Decompiler.UI.ViewModels
{
    public class UnhandledExceptionViewModel : Screen
    {
        #region Actions

        public void Yes()
        {
            RequestClose(false);
        }

        public void No()
        {
            RequestClose(true);
        }

        public void Copy()
        {
            Clipboard.SetText(MessageText);
        }

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
        public TextBlock Message
        {
            get => _message;
            set => SetAndNotify(ref _message, value);
        }

        private string _stack = "";
        public string Stack
        {
            get => _stack;
            set => SetAndNotify(ref _stack, value);
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

        private Visibility _buttonLeftVisibility = Visibility.Hidden;
        public Visibility ButtonLeftVisibility
        {
            get => _buttonLeftVisibility;
            set => SetAndNotify(ref _buttonLeftVisibility, value);
        }

        private Visibility _detailVisibility = Visibility.Hidden;
        public Visibility DetailVisibility
        {
            get => _detailVisibility;
            set => SetAndNotify(ref _detailVisibility, value);
        }

        public string MessageText { get; set; } = "";

        #endregion

        public UnhandledExceptionViewModel(string message, string title = "Notice", bool isOption = false, string? stack = null,
            string? extendedMessageColor = null, string yesButtonText = "Yes", string noButtonText = "Auto")
        {
            MessageText = $"**{title}**\n> {message}\n\n```\n{stack}\n```";
            Message = message.ToTextBlock();
            Title = title;
            ButtonRight = noButtonText == "Auto" ? "Ok" : noButtonText;
            ButtonLeft = yesButtonText;

            if (isOption)
            {
                ButtonLeftVisibility = Visibility.Visible;
                ButtonRight = noButtonText == "Auto" ? "No" : noButtonText;
            }

            if (stack != null)
            {
                DetailVisibility = Visibility.Visible;
                Stack = stack;

                if (extendedMessageColor != null)
                    Foreground = (Brush)new BrushConverter().ConvertFromString(extendedMessageColor);
            }
        }
    }
}
