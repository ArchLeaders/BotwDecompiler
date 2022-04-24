using Decompiler.UI.ViewModels;
using Decompiler.UI.ViewResources.Data;
using Discord;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Operations;
using System.Threading.Tasks;

namespace Decompiler.UI.ViewResources.Helpers
{
    public class ReportError
    {
        public static async Task DiscordMessage(ulong channelId, string body)
        {
            DiscordBot bot = new();
            await bot.RunAsync(ReportError);

            async Task ReportError()
            {
                if (bot.Client == null)
                    return;

                IMessageChannel channel = (IMessageChannel)await bot.Client.GetChannelAsync(channelId);

                await channel.SendMessageAsync(body);
            }
        }

        public static async Task<bool> DiscordEmbed(ulong channelId, HandledExceptionViewModel ex) => await DiscordEmbed(channelId, ex.Title, ex.Message, ex.StackText, ex.User);
        public static async Task<bool> DiscordEmbed(ulong channelId, UnhandledExceptionViewModel ex) => await DiscordEmbed(channelId, ex.Title, ex.MessageText, ex.Stack, "Anonymous");
        public static async Task<bool> DiscordEmbed(ulong channelId, string title, string message, string stack, string user)
        {
            bool results = false;

            DiscordBot bot = new();
            await bot.RunAsync(ReportError);
            return results;

            async Task ReportError()
            {
                if (bot.Client == null)
                {
                    results = false;
                    return;
                }

                IMessageChannel channel = (IMessageChannel)bot.Client.GetChannel(channelId);

                if (channel == null)
                {
                    results = false;
                    return;
                }

                EmbedBuilder embed = new()
                {
                    Title = title,
                    Description = message,
                    Footer = new() { Text = new ShellViewModel(null).Title },
                    Timestamp = DateTime.Now,
                    Author = new()
                    {
                        Name = user,
                        IconUrl = user.ToLower() == "anonymous" ? "https://static.thenounproject.com/png/302770-200.png" :
                            "https://icons.veryicon.com/png/o/miscellaneous/two-color-icon-library/user-286.png"
                    },
                    Color = Color.Blue
                };

                if (stack.Length <= 200)
                {
                    embed.AddField("Stack Trace", stack);
                }
                else
                {
                    embed.AddField("Stack Trace", "- - - - - - - - -");

                    foreach (var trace in stack.Split("   at "))
                    {
                        if (trace.Trim().Length > 1 && embed.Fields.Count < 15)
                        {
                            embed.AddField("at", trace.Trim());
                        }
                        else if (embed.Fields.Count > 15)
                        {
                            embed.Fields.Clear();
                            break;
                        }
                    }

                    if (embed.Fields.Count == 0)
                    {
                        List<Embed> embeds = new();
                        embeds.Add(embed.Build());

                        foreach (var trace in stack.Split("   at "))
                        {
                            if (trace.Trim().Length > 1 && embed.Fields.Count < 15)
                            {
                                EmbedBuilder subEmbed = new();
                                subEmbed.AddField("at", trace);
                                embeds.Add(subEmbed.Build());
                            }
                        }

                        await channel.SendMessageAsync(embeds: embeds.ToArray());
                        results = true;
                        return;
                    }
                }

                await channel.SendMessageAsync(embed: embed.Build());
                results = true;
                return;
            }
        }

        public static async Task<bool> GitHub(Stylet.IWindowManager win, string repo, string title, string body, string owner = "archleaders")
        {
            // Get repo
            if (!win.Show($"{ToolTips.ReportError}\n\nContinue anyway?", "Privacy Warning", true, width: 500)) return false;

            // Create git client
            GitHubClient client = new(new ProductHeaderValue($"{repo}--{new Random().Next(1000, 9999)}"));
            client.Credentials = new Credentials(Data.User.GitHubToken);

            // Create new issue
            var issueNew = await client.Issue.Create(owner, repo, new NewIssue(title) { Body = body });
            win.Show($"Created issue: {issueNew.Id}");

            return true;
        }

        public static string Markdown(HandledExceptionViewModel ex) => Markdown(ex.Title, ex.Message, ex.StackText, ex.User);
        public static string Markdown(UnhandledExceptionViewModel ex) => Markdown(ex.Title, ex.MessageText, ex.Stack, "Anonymous");
        public static string Markdown(string title, string message, string stack, string user)
        {
            return $"# {title}\n> {user}\n\n{message}\n\n```\n{stack}\n```";
        }

        public static async Task HtmlView(UnhandledExceptionViewModel ex) => await HtmlView(new HandledExceptionViewModel(null, ex.Title, ex.MessageText, ex.Stack));
        public static async Task HtmlView(HandledExceptionViewModel ex)
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string htmlFile = $"{appdata}\\Temp\\{new Random().Next(1000, 9999)}-{new Random().Next(1000, 9999)}.htm";
            string fullReport = Html(ex).Replace(user, "C:\\Users\\admin");

            await File.WriteAllTextAsync(htmlFile, fullReport);
            await Execute.Explorer($"\"{htmlFile}\"");
        }

        public static string Html(HandledExceptionViewModel ex) => Html(ex.Title, ex.Message, ex.StackText, ex.User);
        public static string Html(UnhandledExceptionViewModel ex) => Html(ex.Title, ex.MessageText, ex.Stack, "Anonymous");
        public static string Html(string title, string message, string stack, string user)
        {
            // - - Make an actuall class to create this in future - -

            #region HTML Meta-data/CSS

            string css = new(
                "<style>" +
                "html {" +
                "    background: #121212;" +
                "    color: #e1e1e1;" +
                "    font-family: 'Ubuntu';" +
                "    padding: 15px;" +
                "}" +
                "hr {" +
                "    border-color: #7160E8;" +
                "    margin-left: 30px;" +
                "    margin-right: 30px;" +
                "}" +
                "h1 {" +
                "    padding-top: 15px;" +
                "    padding-bottom: 0px;" +
                "    font-size: 40px;" +
                "    padding-left: 30px;" +
                "    margin-bottom: 0px;" +
                "}" +
                "#contact {" +
                "    padding-top: 15px;" +
                "    padding-bottom: 0px;" +
                "    font-size: 20px;" +
                "    padding-left: 30px;" +
                "}" +
                "h3 {" +
                "    padding-left: 50px;" +
                "}" +
                "p {" +
                "    padding-left: 60px;" +
                "    font-size: 26px;" +
                "    font-weight: lighter;" +
                "}" +
                "span {" +
                "    font-style: italic;" +
                "    color: #797979;" +
                "}" +
                "code {" +
                "display: inline-block;" +
                "    background: #353535;" +
                "    margin-left: 60px;" +
                "    padding: 5px;" +
                "    padding-left: 10px;" +
                "    padding-right: 10px;" +
                "    border-radius: 5px;" +
                "    line-height: 25px;" +
                "    font-family: 'Ubuntu Mono', monospace;;" +
                "    font-size: 14px;" +
                "    font-weight: bold;" +
                "}" +
                "</style>"
            );

            string htmlHeader = new(
                $"<!DOCTYPE html>" +
                $"<html lang=\"en\">" +
                $"<head>" +
                $"\t<meta charset=\"UTF-8\">" +
                $"\t<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                $"\t<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                $"\t<link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">\n" +
                $"\t<link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>\n" +
                $"\t<link href=\"https://fonts.googleapis.com/css2?family=Ubuntu+Mono&family=Ubuntu:wght@300&display=swap\" rel=\"stylesheet\">\n" +
                $"\t<title>{title}</title>{css}" +
                $"</head>" +
                $"<body>"
            );

            string htmlFooter = new(
                $"</body>" +
                $"</html>"
            );

            #endregion

            string body = new(
                $"<span>*The contents of this page will be uploaded to a public or private GitHub repository</span><br>" +
                $"<h1>{title}</h1>" +
                $"<label id=\"contact\">{user}<label><hr>" +
                $"<p>{message}</p>" +
                $"<code>{stack}</code>"
            );

            return new(
                $"{htmlHeader}{body}{htmlFooter}"
            );
        }
    }
}
