using ReleaseNotesGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ReleaseNotesGenerator.Sources
{
    public static class ReportWriter
    {
        public static string WriteHtml(List<CommitInfo> commitInfos, string emailContent = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html><body>")
                .AppendLine("<div style=\"font-family: Calibri; font-size: 11pt\">")
                .AppendLine(emailContent)
                .AppendLine("</div>")
                .AppendLine("<div style=\"font-family: Monospace; font-size: 11pt\">")
                .AppendLine("<br><b>Release notes:</b><br><br>");

            FormatCommits(commitInfos);

            foreach (var commitInfo in commitInfos)
            {
                sb.AppendFormat("JIRA Issue: {0}<br>", commitInfo.JiraIssuesFormatted).AppendLine();

                foreach(var desc in commitInfo.DescriptionsList)
                {
                    sb.AppendFormat("Description: {0}<br>", desc.Trim()).AppendLine();
                }

                sb.AppendLine("<br>");
            }

            sb.AppendLine("</div>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        public static void OpenReport(string content)
        {
            string fpath = Path.GetTempPath() + "release_notes_" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".htm";
            File.WriteAllText(fpath, content);

            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = fpath;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void FormatCommits(List<CommitInfo> commitInfos)
        {
            foreach (var info in commitInfos)
            {
                // Remove redundant mentions
                info.DescriptionsList = info.DescriptionsList.Select(desc =>
                    info.JiraIssuesList.Aggregate(desc, (current, strToRemove) =>
                        current.Replace(strToRemove, string.Empty).Trim())).ToList();

                // Remove leading and trailing characters
                info.DescriptionsList = info.DescriptionsList
                    .Select(lead => lead.StartsWith("-") ? lead.Substring(1) : lead)
                    .Select(trail => trail.EndsWith(".") ? trail : trail + ".")
                    .ToList();
            }
        }
    }
}
