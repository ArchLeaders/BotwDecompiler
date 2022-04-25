using Acheron.Web;
using Decompiler.UI.ViewResources.Helpers;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Decompiler.UI.ViewModels
{
    public class ShellViewModel : Screen
    {
        /// 
        /// App parameters
        /// 
        public static int MinHeight { get; set; } = 400;
        public static int MinWidth { get; set; } = 600;
        public static bool CanResize { get; set; } = false;
        public string HelpLink { get; set; } = "https://github.com/ArchLeaders/botwdecompiler";
        public string Title { get; set; } = "BOTW Asset Decompiler";

        // Error report settings
        public static bool UseGitHubUpload { get; set; } = false;
        public string GitHubRepo { get; set; } = "botwdecompiler";
        public string DiscordInvite { get; set; } = "https://discord.gg/cbA3AWwfJj";
        public ulong DiscordReportChannel { get; set; } = 825165473661714492;

        ///
        /// Actions
        ///
        #region Actions

        public async void Decompile()
        {
            string temp = $"{Environment.GetEnvironmentVariable("temp")}\\botw_decomp";
            string repo = "https://raw.githubusercontent.com/ArchLeaders/BotwDecompiler/master/src";

            try
            {
                await Task.Run(async() =>
                {
                    IsEnabled = false;
                    Message = "Downloading decompiler . . .";

                    List<Task> tasks = new();

                    tasks.Add(new Uri($"{repo}/main.py").DownloadFile($"{temp}\\main.py", true));
                    tasks.Add(new Uri($"{repo}/decomp.py").DownloadFile($"{temp}\\decomp.py", true));
                    tasks.Add(new Uri($"{repo}/exts.py").DownloadFile($"{temp}\\exts.py", true));
                    tasks.Add(new Uri($"{repo}/utils.py").DownloadFile($"{temp}\\utils.py", true));
                    tasks.Add(new Uri($"{repo}/lib.zip").DownloadFile($"{temp}\\lib.zip", true));
                    tasks.Add(new Uri($"{repo}/imported/bars_extractor.py").DownloadFile($"{temp}\\imported\\bars_extractor.py", true));

                    await Task.WhenAll(tasks);

                    Message = "Writing configuration . . .";

                    await File.WriteAllTextAsync($"{temp}\\config.yml",
                        $"aamp: {AAMP}\n" +
                        $"bars: {BARS}\n" +
                        $"evfl: {EVFL}\n" +
                        $"fres: {FRES}\n" +
                        $"byml: {BYML}\n" +
                        $"havk: {HAVK}\n" +
                        $"msbt: {MSBT}\n" +
                        $"sarc: {SARC}\n" +
                        $"copy: {COPY}\n" +
                        $"out_folder: {ExportDir}"
                    );

                    Message = "Extracting libs . . .";

                    ZipFile.ExtractToDirectory($"{temp}\\lib.zip", $"{temp}\\lib", true);

                    Message = "Decompiling, please wait (a while) . . .";

                    await System.Operations.Execute.App("python.exe", $"main.py", hidden: Silent, workingDirectory: temp);

                    Message = "Done! You can close this window now.";

                    var _notifyIcon = new System.Windows.Forms.NotifyIcon();
                    _notifyIcon.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                    _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
                    _notifyIcon.BalloonTipClicked += (s, e) =>
                    {
                        if (WindowManager.Show("Close BOTW Asset Decompiler?", isOption: true))
                            Environment.Exit(0);
                    };
                    _notifyIcon.Visible = true;
                    _notifyIcon.ShowBalloonTip(5000, "BOTW Asset Decompiler", "BOTW has finished decompiling.", System.Windows.Forms.ToolTipIcon.Info);
                });

            }
            catch (Exception ex)
            {
                IsEnabled = true;
                ThrowException(new(this, "Unhandled Exception when Decompiling", ex.Message, ex.StackTrace));
            }
            finally
            {
                if (Directory.Exists(temp))
                    Directory.Delete(temp, true);

                if (Silent)
                    Environment.Exit(0);
            }
            
        }

        public void Browse()
        {
            System.Windows.Forms.FolderBrowserDialog browse = new();
            if (browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ExportDir = browse.SelectedPath;
        }

        #endregion

        ///
        /// Properties
        ///
        #region Properties

        private string _exportDir = $"{Environment.GetEnvironmentVariable("userprofile")}\\BOTW-Decompiled";
        public string ExportDir
        {
            get => _exportDir;
            set => SetAndNotify(ref _exportDir, value);
        }

        private bool _aamp = true;
        public bool AAMP
        {
            get => _aamp;
            set => SetAndNotify(ref _aamp, value);
        }

        private bool _bars = true;
        public bool BARS
        {
            get => _bars;
            set => SetAndNotify(ref _bars, value);
        }

        private bool _evfl = true;
        public bool EVFL
        {
            get => _evfl;
            set => SetAndNotify(ref _evfl, value);
        }

        private bool _fres = true;
        public bool FRES
        {
            get => _fres;
            set => SetAndNotify(ref _fres, value);
        }

        private bool _byml = true;
        public bool BYML
        {
            get => _byml;
            set => SetAndNotify(ref _byml, value);
        }

        private bool _havk = false;
        public bool HAVK
        {
            get => _havk;
            set => SetAndNotify(ref _havk, value);
        }

        private bool _msbt = true;
        public bool MSBT
        {
            get => _msbt;
            set => SetAndNotify(ref _msbt, value);
        }

        private bool _sarc = true;
        public bool SARC
        {
            get => _sarc;
            set => SetAndNotify(ref _sarc, value);
        }

        private bool _copy = true;
        public bool COPY
        {
            get => _copy;
            set => SetAndNotify(ref _copy, value);
        }

        #endregion

        ///
        /// Bindings
        ///
        #region Bindings


        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetAndNotify(ref _isEnabled, value);
        }

        private bool _silent = false;
        public bool Silent
        {
            get => _silent;
            set => SetAndNotify(ref _silent, value);
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetAndNotify(ref _message, value);
        }

        private Visibility _handledExceptionViewVisibility = Visibility.Collapsed;
        public Visibility HandledExceptionViewVisibility
        {
            get => _handledExceptionViewVisibility;
            set => SetAndNotify(ref _handledExceptionViewVisibility, value);
        }

        #endregion

        ///
        /// DataContext
        ///
        #region DataContext

        // Views
        public SettingsViewModel? SettingsViewModel { get; set; } = null;

        private HandledExceptionViewModel? _handledExceptionViewModel = null;
        public HandledExceptionViewModel? HandledExceptionViewModel
        {
            get => _handledExceptionViewModel;
            set => SetAndNotify(ref _handledExceptionViewModel, value);
        }

        // App
        public bool CanFullscreen { get; set; } = CanResize;
        public ResizeMode ResizeMode { get; set; } = CanResize ? ResizeMode.CanResize : ResizeMode.CanMinimize;
        public WindowStyle WindowStyle { get; set; } = CanResize ? WindowStyle.None : WindowStyle.SingleBorderWindow;

        public void ThrowException(HandledExceptionViewModel ex)
        {
            WindowManager.Error(ex.Message, ex.StackText, ex.Title);
            HandledExceptionViewModel = ex;
            HandledExceptionViewVisibility = Visibility.Visible;
        }

        public void Help()
        {
            Process proc = new();

            proc.StartInfo.FileName = "explorer.exe";
            proc.StartInfo.Arguments = HelpLink;

            proc.Start();
        }

        public IWindowManager? WindowManager { get; set; }
        public ShellViewModel(IWindowManager? windowManager)
        {
            WindowManager = windowManager;
            SettingsViewModel = new(this);
        }

        ///
        /// Root Error handling
        /// 
        public void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception as Exception ?? new();
            File.WriteAllText("error.log.txt", $"[{DateTime.Now}]\n- {ex.Message}\n[Stack Trace]\n{ex.StackTrace}\n- - - - - - - - - - - - - - -\n\n");
            ThrowException(new(this, "Unhandled Exception", ex.Message, ex.StackTrace ?? "", true));
            Environment.Exit(0);
        }

        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception ?? new();
            File.WriteAllText("error.log.txt", $"[{DateTime.Now}]\n- {ex.Message}\n[Stack Trace]\n{ex.StackTrace}\n- - - - - - - - - - - - - - -\n\n");
            ThrowException(new(this, "Unhandled Exception", ex.Message, ex.StackTrace ?? "", true));
            Environment.Exit(0);
        }

        #endregion
    }
}
