using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CEFotoDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ArticleReader? ArticleReaderInstance;
        private readonly string downloadDir;

        public MainWindow()
        {
            InitializeComponent();
            downloadDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "C&E Webfoto Downloader");
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog file = new()
            {
                DefaultExt = "*.csv",
                Filter = "Artikelen bestand|*.csv;*.txt"
            };

            Nullable<bool> result = file.ShowDialog();
            if (result == true)
            {
                FileNameBlock.Text = file.FileName;
                FileNameBlock.ToolTip = file.FileName;

                ArticleReaderInstance = new(file.FileName);
                DataContext = ArticleReaderInstance;
            }
        }

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ArticleReaderInstance == null)
            {
                MessageBox.Show("Please open a products file first", "Error");
                return;
            }

            if (Directory.Exists(downloadDir) && Directory.GetFiles(downloadDir).Length > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure? previous download will be removed.", "Notice", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            WebDownloader Downloader = new()
            {
                CreateZip = (bool)ZipChk.IsChecked,
                IncludeExtra = (bool)ExtraChk.IsChecked,
                DownloadList = ArticleReaderInstance.articleList
            };

            await Downloader.Start();
        }

        private void DownloadFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(downloadDir))
            {
                Directory.CreateDirectory(downloadDir);
            }

            Process.Start("explorer.exe", downloadDir);
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new()
            {
                Owner = this
            };
            aboutWindow.Show();
        }
    }
}
