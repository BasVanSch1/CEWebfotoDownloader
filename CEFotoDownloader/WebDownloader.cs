using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace CEFotoDownloader
{
    class WebDownloader
    {
        static readonly HttpClient httpClient = new();
        public ObservableCollection<Article> DownloadList { get; set; } = new();
        public bool CreateZip { get; set; } = false;
        public bool IncludeExtra { get; set; } = false;

        private static readonly string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "C&E Webfoto Downloader");
        private static readonly string tmpDir = Path.Combine(Path.GetTempPath(), "CEPhotoDownload");

        public async Task Start()
        {
            if (DownloadList.Count <= 0)
            {
                MessageBox.Show("Download list is empty, please provide a file with valid product data");
                return;
            }

            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
                Directory.CreateDirectory(targetDir);
            }
            else
            {
                Directory.CreateDirectory(targetDir);
            }

            if (Directory.Exists(tmpDir))
            {
                Directory.Delete(tmpDir, true);
                Directory.CreateDirectory(tmpDir);
            }
            else
            {
                Directory.CreateDirectory(tmpDir);
            }

            ResetItemStatus();

            foreach (Article item in DownloadList)
            {
                item.status = "Waiting...";
                await Download(item);

                if (IncludeExtra)
                {
                    await DownloadExtra(item);
                }
            }

            if (CreateZip)
            {
                if (File.Exists($"{tmpDir}\\productfotos.zip"))
                {
                    File.Delete($"{tmpDir}\\productfotos.zip");
                }

                ZipFile.CreateFromDirectory($"{tmpDir}", $"{targetDir}\\productfotos.zip");
            }
        }

        private void ResetItemStatus()
        {
            foreach (Article item in DownloadList)
            {
                item.status = "";
            }
        }

        private async Task Download(Article item)
        {
            try
            {
                item.status = "Downloading...";

                using var response = await httpClient.GetAsync($"https://images.clayre-eef.com/lowres/{item.articleCode}.jpg");

                if (response.IsSuccessStatusCode)
                {
                    string _targetDir;

                    if (CreateZip)
                    {
                        _targetDir = tmpDir;
                    }
                    else
                    {
                        _targetDir = targetDir;
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using FileStream fileStream = new($"{_targetDir}\\{item.articleCode}.jpg", FileMode.Create);
                        await stream.CopyToAsync(fileStream);
                    }

                    item.status = $"Success ({response.StatusCode})";
                    Debug.WriteLine(item.articleCode + " : " + item.status);
                }
                else
                {
                    item.status = $"Failed ({response.StatusCode})"; ;
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message: {0}", ex.Message);
            }
        }

        private async Task DownloadExtra(Article item)
        {
            try
            {
                List<Article> ExtraList = new();
                for (int i = 1; i <= 10; i++)
                {
                    Article extraArticle = new(item.articleCode + $"_{i}");
                    ExtraList.Add(extraArticle);
                }

                foreach (Article extra in ExtraList)
                {
                    await Download(extra);
                }

            } catch (Exception ex) {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message: {0}", ex.Message);
            }
        }

        private static string getExecutingLoc()
        {
            string? _loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (_loc != null)
            {
                return _loc;
            }

            return string.Empty;
        }
    }
}
