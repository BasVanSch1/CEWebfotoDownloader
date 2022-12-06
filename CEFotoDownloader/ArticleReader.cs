using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace CEFotoDownloader
{
    class ArticleReader
    {
        public ObservableCollection<Article> articleList { get; private set; }

        public ArticleReader(string source)
        {
            this.articleList = CreateCollection(source);
        }

        private static ObservableCollection<Article> CreateCollection(string source)
        {
            ObservableCollection<Article> list = new();

            if (source == "" || source == String.Empty)
            {
                return list;
            }

            try
            {
                string[] lines = File.ReadAllLines(source);

                foreach (string line in lines)
                {
                    Article Article = new(line);
                    Article.status = "Waiting...";

                    if (!list.Contains(Article))
                    {
                        list.Add(Article);
                    }

                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error: file not found! \n{ex.Message}", "Error");
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }

            return list;
        }
    }
}
