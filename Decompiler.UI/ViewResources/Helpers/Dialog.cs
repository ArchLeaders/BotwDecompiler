#pragma warning disable CS8629

using Decompiler.UI.ViewModels;
using Stylet;

namespace Decompiler.UI.ViewResources.Helpers
{
    public static class Dialog
    {
        public static bool Show(this IWindowManager winManager, string message, string title = "Notice", bool isOption = false,
            string? exColor = null, double width = 220, string yesButtonText = "Yes", string noButtonText = "Auto")
        {
            MessageViewModel promptViewModel = new(message, title, isOption, exColor, width, yesButtonText, noButtonText);
            return !(bool)winManager.ShowDialog(promptViewModel);
        }

        public static bool Error(this IWindowManager winManager, string message, string? stack = null, string title = "Unhandled Exception", bool isOption = false,
            string? exColor = null, string yesButtonText = "Yes", string noButtonText = "Auto")
        {
            UnhandledExceptionViewModel promptViewModel = new(message, title, isOption, stack, exColor, yesButtonText, noButtonText);
            return !(bool)winManager.ShowDialog(promptViewModel);
        }
    }
}
