using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReleaseNotesGenerator.Sources
{
    public static class LogParser
    {
        private static List<CommitInfo> commitInfos;

        public static List<CommitInfo> GetParsedCommitInfo(string SvnLogContent)
        {
            commitInfos = new List<CommitInfo>();

            foreach (var commit in Regex.Split(SvnLogContent, @"-+(\r\n|\r|\n)"))
            {
                if (string.IsNullOrWhiteSpace(commit))
                    continue;

                // Build commit
                var match = Regex.Match(commit, @"(?<=Version\s)[0-9\.]+(?=\sset\sby)");
                if (match.Success)
                {
                    NewVersionBump(match.Value);
                }
                else
                {
                    string jira = string.Empty;
                    string desc = string.Empty;

                    // Regular commit
                    if ((match = Regex.Match(commit, @"(?<=^JIRA\sIssue:)((?s)\s.*)(?=Description:)", RegexOptions.Multiline)).Success)
                    {
                        jira = match.Value.Trim();
                    }

                    if ((match = Regex.Match(commit, @"(?<=^Description:)((?s).*)(?=Reviewer:)", RegexOptions.Multiline)).Success)
                    {
                        desc = match.Value.Trim();
                    }

                    NewCommit(jira, desc);
                }
            }

            return commitInfos;
        }

        private static void NewCommit(string jira, string desc)
        {
            CommitInfo commit = commitInfos.FirstOrDefault(c => c.JiraIssue == jira);

            if (commit != null)
            {
                if (commit.Descriptions.FirstOrDefault(c => c.Equals(desc)) == null)
                {
                    commit.Descriptions.Add(desc);
                }
            }
            else
            {
                commit = new CommitInfo
                {
                    JiraIssue = jira
                };
                commit.Descriptions.Add(desc);
                
                commitInfos.Add(commit);
            }
        }

        private static void NewVersionBump(string build)
        {
            commitInfos.Add(new CommitInfo
            {
                VersionBump = true,
                BuildVersion = build
            });
        }
    }
}
