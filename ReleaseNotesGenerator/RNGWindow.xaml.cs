using ReleaseNotesGenerator.Sources;
using ReleaseNotesGenerator.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ReleaseNotesGenerator
{
    /// <summary>
    /// Interaction logic for RNGWindow.xaml
    /// </summary>
    public partial class RNGWindow : Window
    {
        private SubversionHandler svn;
        private ConfigHandler cfg;
        
        private string AppPath;
        private string LastVersion;

        public ConfigContent Config { get; set; }
        public string SelectedRepo { get; set; }

        public RNGWindow()
        {
            InitializeComponent();
            RNGInit();
        }

        private void RNGInit()
        {
            AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            LastVersion = string.Empty;

            svn = new SubversionHandler();
            cfg = new ConfigHandler(AppPath + "\\" + Constants.ConfigName);

            try
            {
                InitChecks();
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }
            
        }

        private async void GenerateReleaseNotes()
        {
            string svnPath = Config.Repos[SelectedRepo];

            try
            {
                ProgBar.IsIndeterminate = true;

                CheckSvnInfo(svnPath);

                UpdateStatus(Constants.StatusLog);
                svn.LimitThreshold = Config.LimitThreshold;
                var SVNLog = await svn.GetSvnLog(svnPath, LastVersion);

                UpdateStatus(Constants.StatusParse);
                var parsedCommits = LogParser.GetParsedCommitInfo(SVNLog, LastVersion);
                ReportWriter.WriteHtml(parsedCommits, Config.EmailContent);

                ProgBar.IsIndeterminate = false;
                UpdateStatus(Constants.StatusIdle);
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }

            EnableDisableUI(true);
        }

        #region Checks

        private void CheckSvnInfo(string path)
        {
            if (!svn.CheckSvnInfo(path))
            {
                Fail(Constants.NoREP);
            }
        }

        private void InitChecks()
        {
            if (!svn.CheckSvnInstalled())
            {
                Fail(Constants.NoSVN);
                return;
            }

            if (!File.Exists(cfg.ConfigFilePath))
            {
                cfg.CreateDefaultConfig();

                Fail(Constants.NoCFG);
                return;
            }

            Config = cfg.ReadConfig();

            if (Config.Repos.ContainsKey("ExREPO"))
            {
                Fail(Constants.NoDEF);
                return;
            }

            Repositories.IsEnabled = true;
            SelectedRepo = Config.Repos.First().Key;
        }

        #endregion

        #region Utils

        private void Fail(string msg)
        {
            var dialog = MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            if (dialog.Equals(MessageBoxResult.OK))
            {
                Application.Current.Shutdown();
            }
        }

        private void UpdateStatus(string stat)
        {
            InfoLabel.Content = stat;
        }

        private void EnableDisableUI(bool enable)
        {
            Repositories.IsEnabled = enable;
            LastVersionTextbox.IsEnabled = enable;
            GenerateButton.IsEnabled = enable;
        }

        #endregion

        #region Event Handlers

        private void LastVersionTextbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastVersionTextbox.Text))
                GenerateButton.IsEnabled = false;

            LastVersion = LastVersionTextbox.Text.Trim();
            GenerateButton.IsEnabled = true;
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            EnableDisableUI(false);
            GenerateReleaseNotes();
        }

        #endregion
    }
}
