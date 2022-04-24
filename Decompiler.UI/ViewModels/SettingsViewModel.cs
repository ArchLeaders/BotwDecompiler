using Decompiler.UI.ViewResources.Helpers;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.UI.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private ShellViewModel ShellViewModel { get; set; }
        public void Throw()
        {
            try
            {
                File.ReadAllBytes("D:\\void.ini");
            }
            catch (Exception ex)
            {
                if (ShellViewModel.WindowManager != null)
                {
                    ShellViewModel.WindowManager.Show($"{ex.Message}", "Error", isOption: true);
                    ShellViewModel.WindowManager.Error($"{ex.Message}", $"{ex.Source}.{ex.TargetSite}\n\n[Stack Trace]\n{ex.StackTrace}\n\n[Inner Exception]\n{ex.InnerException}");
                    ShellViewModel.ThrowException(new(ShellViewModel, "Handled Exception", ex.Message, ex.StackTrace ?? "", true));
                }
            }
        }

        public SettingsViewModel(ShellViewModel shell)
        {
            ShellViewModel = shell;
        }
    }
}
