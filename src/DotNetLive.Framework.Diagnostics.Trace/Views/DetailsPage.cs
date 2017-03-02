using DotNetLive.Framework.Diagnostics.Trace.DiagnosticsViewPage;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetLive.Framework.Diagnostics.Trace.Views
{
    public class DetailsPage : BaseView
    {
        public DetailsPage(DetailsPageModel model)
        {
            Model = model;
        }

        public DetailsPageModel Model { get; set; }

        public HelperResult LogRow(LogInfo log)
        {
            return new HelperResult((writer) =>
            {
                if (log.Severity >= Model.Options.MinLevel &&
                   (string.IsNullOrEmpty(Model.Options.NamePrefix) || log.Name.StartsWith(Model.Options.NamePrefix, StringComparison.Ordinal)))
                {
                    WriteLiteralTo(writer, "        <tr>\r\n            <td>");
                    WriteTo(writer, string.Format("{0:MM/dd/yy}", log.Time));
                    WriteLiteralTo(writer, "</td>\r\n            <td>");
                    WriteTo(writer, string.Format("{0:H:mm:ss}", log.Time));
                    var severity = log.Severity.ToString().ToLowerInvariant();
                    WriteLiteralTo(writer, $"</td>\r\n            <td class=\"{severity}\">");
                    WriteTo(writer, log.Severity);

                    WriteLiteralTo(writer, $"</td>\r\n            <td title=\"{log.Name}\">");
                    WriteTo(writer, log.Name);

                    WriteLiteralTo(writer, $"</td>\r\n            <td title=\"{log.Message}\"" +
                        "class=\"logState\" width=\"100px\">");
                    WriteTo(writer, log.Message);

                    WriteLiteralTo(writer, $"</td>\r\n            <td title=\"{log.Exception}\">");
                    WriteTo(writer, log.Exception);

                    WriteLiteralTo(writer, "</td>\r\n        </tr>\r\n");
                }
            });
        }

        public HelperResult Traverse(ScopeNode node)
        {
            return new HelperResult((writer) =>
            {
                var messageIndex = 0;
                var childIndex = 0;
                while (messageIndex < node.Messages.Count && childIndex < node.Children.Count)
                {
                    if (node.Messages[messageIndex].Time < node.Children[childIndex].StartTime)
                    {
                        LogRow(node.Messages[messageIndex]);
                        messageIndex++;
                    }
                    else
                    {
                        Traverse(node.Children[childIndex]);
                        childIndex++;
                    }
                }
                if (messageIndex < node.Messages.Count)
                {
                    for (var i = messageIndex; i < node.Messages.Count; i++)
                    {
                        LogRow(node.Messages[i]);
                    }
                }
                else
                {
                    for (var i = childIndex; i < node.Children.Count; i++)
                    {
                        Traverse(node.Children[i]);
                    }
                }
            });
        }

        public DetailsPage()
        {
        }

#pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            Response.ContentType = "text/html";

            WriteLiteral(@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>ASP.NET Core Logs</title>
    <script src=""http://ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.1.min.js""></script>
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

body { font-size: 0.9em; width: 90%; margin: 0px auto; }

h1 { padding-bottom: 10px; }

h2 { font-weight: normal; }

table { border-spacing: 0px; width: 100%; border-collapse: collapse; border: 1px solid black; white-space: pre-wrap; }

th { font-family: Arial; }

td, th { padding: 8px; }

#headerTable, #cookieTable { border: none; height: 100%; }

#headerTd { white-space: normal; }

#label { width: 20%; border-right: 1px solid black; }

#logs { margin-top: 10px; margin-bottom: 20px; }

    #logs > tbody > tr > td { border-right: 1px dashed lightgray; }

    #logs > thead > tr > th { border: 1px solid black; }

    </style>
</head>
<body>
    <h1>ASP.NET Core Logs</h1>
");

            var context = Model.Activity?.HttpInfo;
            WriteLiteral("    ");
            if (context != null)
            {
                WriteLiteral("        <h2 id=\"requestHeader\">Request Details</h2>\r\n        <table id=\"requestDetails\">\r\n            <colgroup><col id=\"label\" /><col /></colgroup>\r\n\r\n            <tr>\r\n                <th>Path</th>\r\n                <td>");
                Write(context.Path);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Host</th>\r\n                <td>");
                Write(context.Host);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Content Type</th>\r\n                <td>");
                Write(context.ContentType);

                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Method</th>\r\n                <td>");
                Write(context.Method);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Protocol</th>\r\n                <td>");
                Write(context.Protocol);
                WriteLiteral(@"</td>
            </tr>
            <tr>
                <th>Headers</th>
                <td id=""headerTd"">
                    <table id=""headerTable"">
                        <thead>
                            <tr>
                                <th>Variable</th>
                                <th>Value</th>
                            </tr>
                        </thead>
                        <tbody>
");
                foreach (var header in context.Headers)
                {
                    WriteLiteral("                            <tr>\r\n                                <td>");
                    Write(header.Key);
                    WriteLiteral("</td>\r\n                                <td>");
                    Write(string.Join(";", header.Value));
                    WriteLiteral("</td>\r\n                            </tr>\r\n");
                }
                WriteLiteral("                        </tbody>\r\n                    </table>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n                <th>Status Code</th>\r\n                <td>");
                Write(context.StatusCode);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>User</th>\r\n                <td>");
                Write(context.User.Identity.Name);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Claims</th>\r\n                <td>\r\n");
                if (context.User.Claims.Any())
                {
                    WriteLiteral(@"                    <table id=""claimsTable"">
                        <thead>
                            <tr>
                                <th>Issuer</th>
                                <th>Value</th>
                            </tr>
                        </thead>
                        <tbody>
");

                    foreach (var claim in context.User.Claims)
                    {




                        WriteLiteral("                                <tr>\r\n                                    <td>");
                        Write(claim.Issuer);
                        WriteLiteral("</td>\r\n                                    <td>");
                        Write(claim.Value);
                        WriteLiteral("</td>\r\n                                </tr>\r\n");
                    }
                    WriteLiteral("                        </tbody>\r\n                    </table>\r\n");
                }
                WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <th>Scheme</th>\r\n                <td>");
                Write(context.Scheme);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Query</th>\r\n                <td>");
                Write(context.Query.Value);
                WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Cookies</th>\r\n                <td>\r\n");
                if (context.Cookies.Any())
                {
                    WriteLiteral(@"                    <table id=""cookieTable"">
                        <thead>
                            <tr>
                                <th>Variable</th>
                                <th>Value</th>
                            </tr>
                        </thead>
                        <tbody>
");
                    foreach (var cookie in context.Cookies)
                    {
                        WriteLiteral("                                <tr>\r\n                                    <td>");
                        Write(cookie.Key);
                        WriteLiteral("</td>\r\n                                    <td>");
                        Write(string.Join(";", cookie.Value));
                        WriteLiteral("</td>\r\n                                </tr>\r\n");
                    }
                    WriteLiteral("                        </tbody>\r\n                    </table>\r\n");
                }
                WriteLiteral("                </td>\r\n            </tr>\r\n        </table>\r\n");
            }
            WriteLiteral("    <h2>Logs</h2>\r\n    <form method=\"get\">\r\n        <select name=\"level\">\r\n");
            foreach (var severity in Enum.GetValues(typeof(LogLevel)))
            {
                var severityInt = (int)severity;
                if ((int)Model.Options.MinLevel == severityInt)
                {
                    WriteLiteral("                    <option");
                    BeginWriteAttribute("value", " value=\"", 7723, "\"", 7743, 1);
                    WriteAttributeValue("", 7731, severityInt, 7731, 12, false);
                    EndWriteAttribute();
                    WriteLiteral(" selected=\"selected\">");
                    Write(severity);
                    WriteLiteral("</option>\r\n");
                }
                else
                {
                    WriteLiteral("                    <option");
                    BeginWriteAttribute("value", " value=\"", 7872, "\"", 7892, 1);
                    WriteAttributeValue("", 7880, severityInt, 7880, 12, false);
                    EndWriteAttribute();
                    WriteLiteral(">");
                    Write(severity);
                    WriteLiteral("</option>\r\n");
                }
            }
            WriteLiteral("        </select>\r\n        <input type=\"text\" name=\"name\"");
            BeginWriteAttribute("value", " value=\"", 8005, "\"", 8038, 1);
            WriteAttributeValue("", 8013, Model.Options.NamePrefix, 8013, 25, false);
            EndWriteAttribute();
            WriteLiteral(@" />
        <input type=""submit"" value=""filter"" />
    </form>
    <table id=""logs"">
        <thead>
            <tr>
                <th>Date</th>
                <th>Time</th>
                <th>Severity</th>
                <th>Name</th>
                <th>State</th>
                <th>Error</th>
            </tr>
        </thead>
        ");
            Write(Traverse(Model.Activity.Root));
            WriteLiteral(@"
    </table>
    <script type=""text/javascript"">
        $(document).ready(function () {
            $(""#requestHeader"").click(function () {
                $(""#requestDetails"").toggle();
            });
        });
    </script>
</body>
</html>");
        }
    }
}
