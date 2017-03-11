using DotNetLive.Framework.Diagnostics.Trace.DiagnosticsViewPage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DotNetLive.Framework.Diagnostics.Trace.Views
{
    public class LogPage : BaseView
    {
        public LogPage()
        {
        }

        public LogPage(LogPageModel model)
        {
            Model = model;
        }

        public LogPageModel Model { get; set; }

        public override async Task ExecuteAsync()
        {
            Response.ContentType = "text/html";

            WriteLiteral(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>ASP.NET Core Logs</title>
    <script src=""//ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.1.min.js""></script>
<style>
body { font-family: 'Segoe UI', Tahoma, Arial, Helvtica, sans-serif; line-height: 1.4em; }

h1 { font-family: 'Segoe UI', Helvetica, sans-serif; font-size: 2.5em; }

td { text-overflow: ellipsis; overflow: hidden; }

tr:nth-child(2n) { background-color: #F6F6F6; }

.critical { background-color: red; color: white; }

.error { color: red; }

.information { color: blue; }

.debug { color: black; }

.warning { color: orange; }
body { font-size: .813em; white-space: nowrap; margin: 20px; }

col:nth-child(2n) { background-color: #FAFAFA; }

form { display: inline-block; }

h1 { margin-left: 25px; }

table { margin: 0px auto; border-collapse: collapse; border-spacing: 0px; table-layout: fixed; width: 100%; }

td, th { padding: 4px; }

thead { font-size: 1em; font-family: Arial; }

tr { height: 23px; }

#requestHeader { border-bottom: solid 1px gray; border-top: solid 1px gray; margin-bottom: 2px; font-size: 1em; line-height: 2em; }

.collapse { color: black; float: right; font-weight: normal; width: 1em; }

.date, .time { width: 70px; }

.logHeader { border-bottom: 1px solid lightgray; color: gray; text-align: left; }

.logState { text-overflow: ellipsis; overflow: hidden; }

.logTd { border-left: 1px solid gray; padding: 0px; }

.logs { width: 80%; }

.logRow:hover { background-color: #D6F5FF; }

.requestRow > td { border-bottom: solid 1px gray; }

.severity { width: 80px; }

.summary { color: black; line-height: 1.8em; }

    .summary > th { font-weight: normal; }

.tab { margin-left: 30px; }

#viewOptions { margin: 20px; }

    #viewOptions > * { margin: 5px; }

</style>
</head>
<body>
    <h1>ASP.NET Core Trace Logs</h1>
");
            WriteLiteral("<table>");
            foreach (var activity in Model.Activities.Reverse())
            {
                WriteLiteral("<tbody><tr class=\"requestRow\">");
                var activityPath = Model.Path.Value + "/" + activity.Id;
                if (activity.HttpInfo != null)
                {
                    WriteLiteral("<td><a");
                    BeginWriteAttribute("href", " href=\"", 6313, "\"", 6333, 1);
                    WriteAttributeValue("", 6320, activityPath, 6320, 13, false);
                    EndWriteAttribute();
                    BeginWriteAttribute("title", " title=\"", 6334, "\"", 6365, 1);
                    WriteAttributeValue("", 6342, activity.HttpInfo.Path, 6342, 23, false);
                    EndWriteAttribute();
                    WriteLiteral(">");
                    Write(activity.HttpInfo.Path);
                    WriteLiteral("</a></td><td>");
                    Write(activity.HttpInfo.Method);
                    WriteLiteral("</td><td>");
                    Write(activity.HttpInfo.Host);
                    WriteLiteral("</td><td>");
                    Write(activity.HttpInfo.StatusCode);
                    WriteLiteral("</td>");
                }
                else if (activity.RepresentsScope)
                {
                    WriteLiteral("<td colspan=\"4\">NoHttpRequest <a");
                    BeginWriteAttribute("href", " href=\"", 6755, "\"", 6775, 1);
                    WriteAttributeValue("", 6762, activityPath, 6762, 13, false);
                    EndWriteAttribute();
                    BeginWriteAttribute("title", " title=\"", 6776, "\"", 6804, 1);
                    WriteAttributeValue("", 6784, activity.Root.State, 6784, 20, false);
                    EndWriteAttribute();
                    WriteLiteral(">");
                    Write(activity.Root.State);
                    WriteLiteral("</a></td>");
                }
                else
                {
                    WriteLiteral("<td colspan=\"4\"><a");
                    BeginWriteAttribute("href", " href=\"", 6967, "\"", 6987, 1);
                    WriteAttributeValue("", 6974, activityPath, 6974, 13, false);
                    EndWriteAttribute();
                    WriteLiteral(">Non-scope Log</a></td>");
                }
                WriteLiteral("</tr><tr>");
                WriteLiteral(@"<td class=""logTd"" colspan='4'>
                        <table class=""logTable"">
                            <thead class=""logHeader"">
                                <tr class=""headerRow"">
                                    <th class=""date"">Date</th>
                                    <th class=""time"">Time</th>
                                    <th class=""name"">Name</th>
                                    <th class=""severity"">Severity</th>
                                    <th class=""state"">State</th>
                                    <th>Error<span class=""collapse"">^</span></th>
                                </tr>
                            </thead>");

                var counts = new Dictionary<string, int>
                {
                    ["Critical"] = 0,
                    ["Error"] = 0,
                    ["Warning"] = 0,
                    ["Information"] = 0,
                    ["Debug"] = 0
                };
                WriteLiteral("<tbody class=\"logBody\">");
                if (!activity.RepresentsScope)
                {
                    // message not within a scope
                    var logInfo = activity.Root.Messages.FirstOrDefault();
                    Write(LogRow(logInfo, 0));
                    counts[logInfo.Severity.ToString()] = 1;
                }
                else
                {
                    Write(Traverse(activity.Root, 0, counts));
                }
                WriteLiteral("</tbody><tbody class=\"summary\"><tr class=\"logRow\"><td>");
                Write(activity.Time.ToString("MM-dd-yyyy HH:mm:ss"));
                WriteLiteral("</td>");
                foreach (var kvp in counts)
                {
                    if (string.Equals("Debug", kvp.Key))
                    {
                        WriteLiteral("<td>");
                        Write(kvp.Value);
                        WriteLiteral(" ");
                        Write(kvp.Key);
                        WriteLiteral("<span class=\"collapse\">v</span></td>");
                    }
                    else
                    {
                        WriteLiteral("<td>");
                        Write(kvp.Value);
                        WriteLiteral(" ");
                        Write(kvp.Key);
                        WriteLiteral("</td>");
                    }
                }
                WriteLiteral("</tr></tbody></table></td>");
                WriteLiteral("</tr></tbody>");
            }
              WriteLiteral("</table>");
            WriteLiteral(@"
    <script type=""text/javascript"">
        $(document).ready(function () {
            $("".logBody"").hide();
            $("".logTable > thead"").hide();
            $("".logTable > thead"").click(function () {
                $(this).closest("".logTable"").find(""tbody"").hide();
                $(this).closest("".logTable"").find("".summary"").show();
                $(this).hide();
            });
            $("".logTable > .summary"").click(function () {
                $(this).closest("".logTable"").find(""tbody"").show();
                $(this).closest("".logTable"").find(""thead"").show();
                $(this).hide();
            });
        });
    </script>
</body>
</html>");
            await Task.FromResult(0);
        }

        public HelperResult LogRow(LogInfo log, int level)
        {
            return new HelperResult((writer) =>
            {
                if (log.Severity >= Model.Options.MinLevel &&
                    (string.IsNullOrEmpty(Model.Options.NamePrefix) || log.Name.StartsWith(Model.Options.NamePrefix, StringComparison.Ordinal)))
                {

                    WriteLiteralTo(writer, "<tr class=\"logRow\"><td>");
                    WriteTo(writer, string.Format("{0:MM/dd/yy}", log.Time));

                    WriteLiteralTo(writer, "</td><td>");
                    WriteTo(writer, string.Format("{0:H:mm:ss}", log.Time));

                    WriteLiteralTo(writer, $"</td><td title=\"{log.Name}\">");
                    WriteTo(writer, log.Name);
                    var severity = log.Severity.ToString().ToLowerInvariant();
                    WriteLiteralTo(writer, $"</td><td class=\"{severity}\">");
                    WriteTo(writer, log.Severity);

                    WriteLiteralTo(writer, $"</td><td title=\"{log.Message}\"> ");

                    for (var i = 0; i < level; i++)
                    {
                        WriteLiteralTo(writer, "<span class=\"tab\"></span>");
                    }

                    WriteLiteralTo(writer, "");
                    WriteTo(writer, log.Message);

                    WriteLiteralTo(writer, $"</td><td title=\"{log.Exception}\">");

                    WriteTo(writer, log.Exception);

                    WriteLiteralTo(writer, "</td></tr>");

                }
            });
        }

        public HelperResult Traverse(ScopeNode node, int level, Dictionary<string, int> counts)
        {
            return new HelperResult((writer) =>
            {
                // print start of scope
                WriteTo(writer, LogRow(new LogInfo()
                {
                    Name = node.Name,
                    Time = node.StartTime,
                    Severity = LogLevel.Debug,
                    Message = "Beginning " + node.State,
                }, level));

                var messageIndex = 0;
                var childIndex = 0;
                while (messageIndex < node.Messages.Count && childIndex < node.Children.Count)
                {
                    if (node.Messages[messageIndex].Time < node.Children[childIndex].StartTime)
                    {
                        WriteTo(writer, LogRow(node.Messages[messageIndex], level));

                        counts[node.Messages[messageIndex].Severity.ToString()]++;
                        messageIndex++;
                    }
                    else
                    {
                        WriteTo(writer, Traverse(node.Children[childIndex], level + 1, counts));
                        childIndex++;
                    }
                }
                if (messageIndex < node.Messages.Count)
                {
                    for (var i = messageIndex; i < node.Messages.Count; i++)
                    {
                        WriteTo(writer, LogRow(node.Messages[i], level));
                        counts[node.Messages[i].Severity.ToString()]++;
                    }
                }
                else
                {
                    for (var i = childIndex; i < node.Children.Count; i++)
                    {
                        WriteTo(writer, Traverse(node.Children[i], level + 1, counts));
                    }
                }
                // print end of scope
                WriteTo(writer, LogRow(new LogInfo()
                {
                    Name = node.Name,
                    Time = node.EndTime,
                    Severity = LogLevel.Debug,
                    Message = string.Format("Completed {0} in {1}ms", node.State, node.EndTime - node.StartTime)
                }, level));
            });
        }
    }
}
