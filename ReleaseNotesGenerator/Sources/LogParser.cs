using ReleaseNotesGenerator.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReleaseNotesGenerator.Sources
{
    public static class LogParser
    {
        private static List<CommitInfo> commitInfos;

        public static List<CommitInfo> GetParsedCommitInfo(string SvnLogContent, string lastVersion)
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

                    if (match.Value.Contains(lastVersion))
                        break;
                    else
                        continue;
                }

                var jiras = Regex.Matches(commit, @"(?<=^JIRA\sIssue:)((?s)\s.*?)(?=(Description|Merge))", RegexOptions.Multiline);
                var descs = Regex.Matches(commit, @"(?<=^Description:)((?s).*?)(?=Reviewer:)", RegexOptions.Multiline);

                NewCommit(
                    jiras.Cast<Match>().Select(j => j.Value.Trim()).ToList(),
                    descs.Cast<Match>().Select(d => d.Value.Trim()).ToList());
            }

            return commitInfos;
        }

        private static void NewCommit(List<string> jiras, List<string> descs)
        {
            CommitInfo commit = commitInfos.FirstOrDefault(c => c.JiraIssuesList.Intersect(jiras).Any());

            if (commit != null)
            {
                commit.JiraIssuesList = commit.JiraIssuesList.Union(jiras).Distinct().ToList();
                commit.DescriptionsList = commit.DescriptionsList.Union(descs).Distinct().ToList();
            }
            else
            {
                commit = new CommitInfo
                {
                    JiraIssuesList = jiras.Distinct().ToList(),
                    DescriptionsList = descs.Distinct().ToList()
                };

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
