using System.Collections.Generic;

namespace ReleaseNotesGenerator.Utils
{
    public class ConfigContent
    {
        public int LimitThreshold { get; set; }
        public Dictionary<string, string> Repos { get; set; }
        public string EmailContent { get; set; }

        public ConfigContent()
        {
            Repos = new Dictionary<string, string>();
        }
    }

    public class CommitInfo
    {
        public bool VersionBump;
        public string BuildVersion;

        public List<string> JiraIssuesList;
        public List<string> DescriptionsList;

        public string JiraIssuesFormatted => string.Join(", ", JiraIssuesList);

        public CommitInfo()
        {
            DescriptionsList = new List<string>();
            JiraIssuesList = new List<string>();
        }
    }
}
