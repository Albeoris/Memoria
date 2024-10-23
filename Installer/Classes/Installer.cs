using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;
using System.Threading;
using System.Text.RegularExpressions;

namespace Installer.Classes
{
    public class Installer
    {
        private static async Task<bool> DownloadLatestReleaseAsync(string downloadPath, IProgress<double> progress, TextBlock progressText)
        {
            try
            {
                progressText.Text = "Downloading Latest Memoria";
                using (HttpClient client = new HttpClient())
                {
                    string downloadUrl = "https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe";

                    using (var downloadResponse = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!downloadResponse.IsSuccessStatusCode)
                        {
                            return false;
                        }

                        var totalBytes = downloadResponse.Content.Headers.ContentLength ?? -1L;
                        var canReportProgress = totalBytes != -1;

                        using (var contentStream = await downloadResponse.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var totalRead = 0L;
                            var buffer = new byte[8192];
                            var isMoreToRead = true;

                            do
                            {
                                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                    progress.Report(100);
                                    continue;
                                }

                                await fileStream.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                if (canReportProgress)
                                {
                                    progress.Report((totalRead * 1d) / (totalBytes * 1d) * 100);
                                }
                            }
                            while (isMoreToRead);
                        }
                    }
                }

                progressText.Text = "Downloading of Memoria complete";
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async static Task<bool> ExtractEmbeddedResource(string gameDirectory, IProgress<double> progress, TextBlock progressText)
        {
            string resourceName = "Installer.Memoria.Patcher.exe";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                long totalBytes = resourceStream.Length;
                long totalRead = 0L;
                byte[] buffer = new byte[8192];
                int bytesRead;
                progressText.Text = "Extracting Memoria";
                using (var fileStream = new FileStream(Path.Combine(gameDirectory, "Memoria.Patcher.exe"), FileMode.Create, FileAccess.Write))
                {
                    while ((bytesRead = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                        progress.Report((totalRead * 1d) / (totalBytes * 1d) * 100);
                    }
                }
                progressText.Text = "Memoria Extracted";
            }
            return true;
        }

        private static void parseTitle(string title, ProgressBar progressBar, TextBlock progressText)
        {
            string pattern = @"Patching... (?<percent>\d+\.\d+)%  Elapsed: (?<elapsed>\d+:\d+)  (?<processed>\d+ \w+) / (?<total>\d+ \w+)  Remaining: (?<remaining>\d+:\d+)";
            Match match = Regex.Match(title, pattern);

            if (match.Success)
            {
                // Extract and parse the values
                double CurrentPercent = double.Parse(match.Groups["percent"].Value);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.Value = CurrentPercent;
                });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressText.Text = "Installing Memoria";
                });
            }
        }

        public static void RunPatcher(string gameDirectory, ProgressBar downloadProgressBar, TextBlock progressText)
        {
            // run the patcher
            progressText.Text = "Patching FFIX Game Files";
            downloadProgressBar.Value = 0;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = gameDirectory + @"\Memoria.Patcher.exe",
                WorkingDirectory = gameDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (s, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                process.Start();
                process.StandardInput.WriteLine("");
                // Periodically check the console window title
                while (!process.HasExited)
                {
                    parseTitle(process.MainWindowTitle, downloadProgressBar, progressText);
                    Thread.Sleep(200);
                }
            }
            progressText.Text = "Game Has been patched";
        }

        public async static Task<bool> CopySetup(string gameDirectory, TextBlock progressText)
        {
            // copy setup file to directory
            string sourcePath = Assembly.GetExecutingAssembly().Location;
            string destinationPath = gameDirectory + "/setup.exe";

            System.IO.File.Copy(sourcePath, destinationPath, true);
            progressText.Text = "Install Complete";
            return true;
        }

        public async static Task<bool> DownloadOrUsePatcher(string gameDirectory, ProgressBar downloadProgressBar, TextBlock progressText)
        {
            var progress = new Progress<double>(value => downloadProgressBar.Value = value);
            bool success = await DownloadLatestReleaseAsync(gameDirectory + @"\Memoria.Patcher.exe", progress, progressText);
            if (!success)
            {
                await ExtractEmbeddedResource(gameDirectory, progress, progressText);
            }
            return true;
        }
    }
}
