using System.Collections.Generic;

namespace ReleaseNotesGenerator.Sources
{
    public static class Constants
    {
        public static string NoSVN = "It seems SVN is not installed or it is not properly setup in the PATH environment variable.";
        public static string NoCFG = "No config file found. A default has been created. Update it with SVN repos and run RNG again.";
        public static string NoDEF = "Update your config file with proper SVN repos and run RNG again.";
        public static string NoDIR = "Invalid SVN working directory.";
        public static string NoREP = "Specified URL is not a valid SVN repository.";

        public static string StatusIdle = "Idle...";
        public static string StatusLog = "Retrieving SVN repository log...";
        public static string StatusParse = "Parsing SVN log...";

        public static string ConfigName = "RNGConfig.xml";
    }

    public static class SvnConstants
    {
        public static string SvnVersion = "svn --version";
        public static string[] SvnInstalledCheck = { "svn", "version" };
        public static string[] SvnInfoCheck = { "Path", "URL" };

        public static string SvnLog(string url, int limit)
        {
            return string.Format("svn log {0} -l {1}", url, limit);
        }

        public static string SvnInfo(string url)
        {
            return string.Format("svn info {0}", url);
        }
    }

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
