using Decompiler.UI.ViewResources.Helpers;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Formatting;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Decompiler.UI.ViewModels
{
    public class HandledExceptionViewModel : Screen
    {
        public async Task Report()
        {
            // Disable reporting to avoid issue overload
            IsReportable = false;

            // Set loading bar value
            IsLoading = true;

            // GitHub upload
            if (ShellViewModel.UseGitHubUpload)
            {
                if (ShellViewModel != null && ShellViewModel.WindowManager != null)
                {
                    // Report issue
                    bool rtn = await ReportError.GitHub(ShellViewModel.WindowManager, ShellViewModel.GitHubRepo, Title, ReportError.Markdown(this));

                    // Allow reports if ReportError.GitHub() canceled
                    IsReportable = rtn;
                    IsLoading = false;
                }
            }

            // Discord upload
            else
            {
                string discordLog = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Temp\\discord.log.txt";

                if (ShellViewModel != null && ShellViewModel.WindowManager != null)
                {
                    bool results = await ReportError.DiscordEmbed(ShellViewModel.DiscordReportChannel, this);
                    string discordLogText = File.Exists(discordLog) ? File.ReadAllText(discordLog) : "Discord logs not found.";

                    IsLoading = false;

                    if (results)
                    {
                        IsReportable = false;
                        ShellViewModel.WindowManager.Show($"Exception successfully reported to Discord\n\n[Discord Invite Link]({ShellViewModel.DiscordInvite})", width: 200);
                    }
                    else
                    {
                        IsReportable = true;
                        ShellViewModel.WindowManager.Error(
                            $"Channel ID: {ShellViewModel.DiscordReportChannel} | [View Logs]({discordLog})",
                            discordLogText,
                            "Discord.Net Exception"
                        );
                    }
                }

                File.Copy(discordLog, ".\\discord.log.txt", true);
                File.Delete(discordLog);
            }
        }

        public async Task Copy()
        {
            await ReportError.HtmlView(this);
        }

        public void Close()
        {
            if (ShellViewModel != null)
                ShellViewModel.HandledExceptionViewVisibility = System.Windows.Visibility.Collapsed;
        }

        private string _title = "Handled Exception";
        public string Title
        {
            get => _title;
            set => SetAndNotify(ref _title, value);
        }

        private string _message = "No details were provided.";
        public string Message
        {
            get => _message;
            set => SetAndNotify(ref _message, value);
        }

        private TextBlock _stack = new();
        public TextBlock Stack
        {
            get => _stack;
            set => SetAndNotify(ref _stack, value);
        }

        private bool _isReportable;
        public bool IsReportable
        {
            get => _isReportable;
            set => SetAndNotify(ref _isReportable, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetAndNotify(ref _isLoading, value);
        }

        private string _user = "Anonymous";
        public string User
        {
            get => _user;
            set => SetAndNotify(ref _user, value);
        }

        public string StackText { get; set; } = "";
        public ShellViewModel? ShellViewModel { get; set; } = null;

        public HandledExceptionViewModel(ShellViewModel? shell, string title, string message, string stack, bool isReportable = true)
        {
            Title = title;
            Message = message;
            StackText = stack;
            Stack = stack.ToTextBlock();
            IsReportable = isReportable;
            ShellViewModel = shell;
        }
    }
}
