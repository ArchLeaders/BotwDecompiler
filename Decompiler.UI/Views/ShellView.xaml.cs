#pragma warning disable CS8604
#pragma warning disable CS8605

using Decompiler.UI.ViewModels;
using Decompiler.UI.ViewResources.Helpers;
using Stylet;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Decompiler.UI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private IWindowManager WindowManager { get; }

        #region Fix Window Size in fullscreen.

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam, ShellViewModel.MinWidth, ShellViewModel.MinHeight);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        internal static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam, int minWidth, int minHeight)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                mmi.ptMinTrackSize.x = minWidth;
                mmi.ptMinTrackSize.y = minHeight;
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new();
            public RECT rcWork = new();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object? obj)
            {
                if (obj is not Rect) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

        public ShellView()
        {
            InitializeComponent();
            DataContext = new ShellViewModel(WindowManager);
            SourceInitialized += (s, e) =>
            {
                System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(((ShellViewModel)DataContext).Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(((ShellViewModel)DataContext).CurrentDomain_UnhandledException);

                IntPtr handle = new WindowInteropHelper(this).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            // Load theme file
            if (File.Exists(AppTheme.ThemeFile))
            {
                AppTheme.ThemeStr = "Light";
                AppTheme.Change(true);
            }
            else
            {
                footerChangeAppThemeIconScale.ScaleY = 1;
                AppTheme.ThemeStr = "Dark";
                AppTheme.Change();
            }

            // Load button close/minimize events
            btnExit.Click += (s, e) => { Hide(); Environment.Exit(1); };
            btnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            btnReSize.Click += (s, e) => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;


            // Assign state changed events
            shellView.StateChanged += (s, e) =>
            {
                if (WindowState == WindowState.Normal)
                {
                    rectCascade.Opacity = 0;
                    rectMaximize.Opacity = 1;
                }
                else
                {
                    rectCascade.Opacity = 1;
                    rectMaximize.Opacity = 0;
                }
            };

            // Animation Triggers
            btnShowSettings.Click += (s, e) =>
            {
                if (settings.Visibility == Visibility.Visible)
                {
                    Animation.DoubleAnim(footerShowSettingsIcon, nameof(footerShowSettingsIconRotate), RotateTransform.AngleProperty, 0, 200);
                    settings.Visibility = Visibility.Hidden;
                }
                else if (settings.Visibility == Visibility.Hidden)
                {
                    Animation.DoubleAnim(footerShowSettingsIcon, nameof(footerShowSettingsIconRotate), RotateTransform.AngleProperty, -45, 200);
                    settings.Visibility = Visibility.Visible;
                }
            };

            // Change app theme event
            footerChangeAppTheme.Click += async (s, e) =>
            {
                if (AppTheme.ThemeStr == "Dark")
                {
                    AppTheme.ThemeStr = "Light";
                    AppTheme.Change(true);
                    Animation.DoubleAnim(footerChangeAppThemeIcon, nameof(footerChangeAppThemeIconScale), ScaleTransform.ScaleYProperty, -1, 250);
                    await Task.Run(() => Thread.Sleep(250));
                }
                else
                {
                    AppTheme.ThemeStr = "Dark";
                    AppTheme.Change();
                    Animation.DoubleAnim(footerChangeAppThemeIcon, nameof(footerChangeAppThemeIconScale), ScaleTransform.ScaleYProperty, 1, 250);
                    await Task.Run(() => Thread.Sleep(250));
                }
            };
        }
    }
}
