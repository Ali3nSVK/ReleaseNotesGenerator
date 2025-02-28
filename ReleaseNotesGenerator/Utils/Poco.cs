using System.Collections.Generic;

namespace ReleaseNotesGenerator.Utils
{
    public class ConfigContent
    {
        public int LimitThreshold { get; set; }
        public Dictionary<string, string> Repos { get; set; }
        public string EmailContent { get; set; }
        public string EmailSubject { get; set; }
        public List<string> EmailRecipients { get; set; }
        public List<string> CcRecipients { get; set; }

        public ConfigContent()
        {
            Repos = new Dictionary<string, string>();
            EmailRecipients = new List<string>();
            CcRecipients = new List<string>();
        }

        public EmailSettings GetEmailSettings()
        {
            var settings = new EmailSettings();
            settings.CC = CcRecipients;
            settings.Recipients = EmailRecipients;
            settings.Subject = EmailSubject;
            return settings;
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

    public class EmailSettings
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Recipients { get; set; } = new List<string>();
        public List<string> CC { get; set; } = new List<string>();
    }
}
