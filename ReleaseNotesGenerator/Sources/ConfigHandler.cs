using Newtonsoft.Json;
using ReleaseNotesGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReleaseNotesGenerator.Sources
{
    public class ConfigHandler
    {
        private readonly string _configFilePath;

        public string ConfigFilePath => _configFilePath;

        public ConfigHandler(string configFilePath = null)
        {
            _configFilePath = configFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        }

        public ConfigContent ReadConfig()
        {
            if (!File.Exists(_configFilePath))
            {
                throw new FileNotFoundException($"Configuration file not found at: {_configFilePath}");
            }

            try
            {
                string json = File.ReadAllText(_configFilePath, Encoding.UTF8);
                var config = JsonConvert.DeserializeObject<ConfigContent>(json);

                if (config.Repos == null)
                {
                    config.Repos = new Dictionary<string, string>();
                }

                if (config.EmailRecipients == null)
                {
                    config.EmailRecipients = new List<string>();
                }

                if (config.CcRecipients == null)
                {
                    config.CcRecipients = new List<string>();
                }

                return config;
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error deserializing settings.json: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading configuration: {ex.Message}", ex);
            }
        }

        public void WriteConfig(ConfigContent config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configFilePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing configuration: {ex.Message}", ex);
            }
        }

        public ConfigContent CreateDefaultConfig(int limitThreshold = 500)
        {
            if (File.Exists(_configFilePath))
            {
                return ReadConfig();
            }

            var defaultConfig = new ConfigContent
            {
                LimitThreshold = limitThreshold,
                EmailContent = "Default email content about SVN repository changes.",
                EmailSubject = "SVN Changes",
                EmailRecipients = new List<string> { "team@example.com", "manager@example.com" },
                CcRecipients = new List<string> { "records@example.com" },
                Repos = new Dictionary<string, string>
                {
                    { "MainRepo", "http://svn.example.com/main" },
                    { "DevRepo", "http://svn.example.com/dev" }
                }
            };

            WriteConfig(defaultConfig);
            return defaultConfig;
        }
    }
}