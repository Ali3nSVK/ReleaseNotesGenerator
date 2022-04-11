using System.Linq;
using System.Xml.Linq;

namespace ReleaseNotesGenerator.Sources
{
    public class ConfigHandler
    {
        public string ConfigFilePath { get; private set; }

        private readonly ConfigContent configContent;

        public ConfigHandler(string config)
        {
            ConfigFilePath = config;
            configContent = new ConfigContent();
        }

        public void SetDefault()
        {
            configContent.Repos.Clear();
            configContent.Repos.Add("ExREPO", "https://server.com/repo");
            configContent.LimitThreshold = 500;
            configContent.EmailContent = string.Empty;
        }

        public void Save()
        {
            var xml =
                new XElement("Config",
                    new XElement("RevisionLimit", configContent.LimitThreshold),
                    new XElement("EmailContent", configContent.EmailContent),
                    new XElement("Repositories", configContent.Repos.Select(r => 
                        new XElement("Repo", 
                            new XElement("Name", r.Key), 
                            new XElement("URL", r.Value)))));

            xml.Save(ConfigFilePath);
        }

        public ConfigContent Load()
        {
            XDocument config = XDocument.Load(ConfigFilePath);
            var doc = XElement.Parse(config.ToString());

            configContent.Repos.Clear();

            configContent.LimitThreshold = int.Parse(doc.Element("RevisionLimit").Value);
            configContent.EmailContent = doc.Element("EmailContent").Value.Trim();
            configContent.Repos = doc.Elements("Repositories").Descendants("Repo").ToDictionary(k => k.Element("Name").Value, k => k.Element("URL").Value);

            return configContent;
        }
    }
}
