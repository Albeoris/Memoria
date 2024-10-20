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

namespace Installer.Classes
{
    public class Installer
    {
        public static async Task<bool> DownloadLatestReleaseAsync(string downloadPath, IProgress<double> progress, TextBlock progressText)
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

        private static void ExtractEmbeddedResource(string gameDirectory, IProgress<double> progress, TextBlock progressText)
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
        }

        private static void RunPatcher(string gameDirectory, ProgressBar downloadProgressBar, TextBlock progressText)
        {
            // run the patcher
            progressText.Text = "Patching FFIX Game Files";
            downloadProgressBar.Value = 100;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = gameDirectory + "Memoria.Patcher.exe",
                Arguments = "-installer",
                UseShellExecute = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
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
                process.WaitForExit();
            }
            progressText.Text = "Game Has been patched";
        }

        private static void CopySetup(string gameDirectory, TextBlock progressText)
        {
            // copy setup file to directory
            string sourcePath = Assembly.GetExecutingAssembly().Location;
            string destinationPath = gameDirectory + "/setup.exe";

            File.Copy(sourcePath, destinationPath, true);
            progressText.Text = "Install Complete";
        }

        // don't actually call this method it's here to keep track of what needs to be called as part of the installer
        private static async void Run(string gameDirectory, ProgressBar downloadProgressBar, TextBlock progressText)
        {

            // Attempting download of patcher
            var progress = new Progress<double>(value => downloadProgressBar.Value = value);
            bool success = await DownloadLatestReleaseAsync(gameDirectory + "Memoria.Patcher.exe", progress, progressText);
            if (!success)
            {
                ExtractEmbeddedResource(gameDirectory, progress, progressText);
            }

            RunPatcher(gameDirectory, downloadProgressBar, progressText);
            CopySetup(gameDirectory, progressText);

            RegistryValues.Instance.AddToUninstallList(gameDirectory);
        }
    }
}
