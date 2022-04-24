using Decompiler.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Decompiler.UI.Views
{
    /// <summary>
    /// Interaction logic for PromptView.xaml
    /// </summary>
    public partial class UnhandledExceptionView : Window
    {
        #region Fix Window Size in fullscreen.

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    ShellView.WmGetMinMaxInfo(hwnd, lParam, 300, 300);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        #endregion

        public UnhandledExceptionView()
        {
            InitializeComponent();
            DataContext = new UnhandledExceptionViewModel("No details were provided.");
            SourceInitialized += (s, a) =>
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
                Dispatcher.Invoke(InvalidateVisual, DispatcherPriority.Input);
            };

            btnOk.Focus();
        }

        private void Window_OnContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
    }
}
