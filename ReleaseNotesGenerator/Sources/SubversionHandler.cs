using ReleaseNotesGenerator.Utils;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ReleaseNotesGenerator.Sources
{
    public class SubversionHandler
    {
        private int SvnLogLimit = 100;
        public int LimitThreshold { get; set; } = 500;

        public bool CheckSvnInstalled()
        {
            string output = RunProcess(SvnConstants.SvnVersion);
            
            if (output.ContainsAll(SvnConstants.SvnInstalledCheck))
                return true;

            return false;
        }

        public bool CheckSvnInfo(string path)
        {
            string output = RunProcess(SvnConstants.SvnInfo(path));

            if (output.ContainsAll(SvnConstants.SvnInfoCheck))
                return true;

            return false;
        }

        public async Task<string> GetSvnLog(string path, string version)
        {
            string output = string.Empty;

            await Task.Run(() =>
            {
                while (!(output = RunProcess(SvnConstants.SvnLog(path, SvnLogLimit))).Contains(version))
                {
                    SvnLogLimit *= 2;

                    if (SvnLogLimit > LimitThreshold)
                        throw new InvalidDataException(string.Format("Last deployed version was not found in the last {0} revisions.", SvnLogLimit));
                }
            });

            return output;
        }

        private Process GetNewProcess(string command)
        {
            Process ret = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = "/C " + command
                }
            };

            return ret;
        }

        private string RunProcess(string command)
        {
            Process p = GetNewProcess(command);
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
    }
}
