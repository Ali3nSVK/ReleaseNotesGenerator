using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ReleaseNotesGenerator.Sources
{
    public static class ReportWriter
    {
        public static void WriteHtml(List<CommitInfo> commitInfos, string version, string emailContent = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html><body style=\"font-family: Monospace\">")
                .AppendLine(emailContent)
                .AppendLine("<br><b>Release notes:</b><br><br>");

            foreach (var commitInfo in commitInfos)
            {
                if (commitInfo.VersionBump)
                {
                    if (commitInfo.BuildVersion == version)
                        break;
                }
                else
                {
                    sb.AppendFormat("JIRA Issue: {0}<br>", commitInfo.JiraIssue).AppendLine();

                    foreach(var desc in commitInfo.Descriptions)
                    {
                        sb.AppendFormat("Description: {0}<br>", desc).AppendLine();
                    }

                    sb.AppendLine("<br>");
                }
            }

            sb.AppendLine("</body></html>");

            string fpath = Path.GetTempPath() + "release_notes_" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".htm";
            File.WriteAllText(fpath, sb.ToString());

            OpenReport(fpath);
        }

        private static void OpenReport(string path)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = path;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
