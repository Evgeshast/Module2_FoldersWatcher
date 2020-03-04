using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading;

namespace FoldersWatcher
{
    class Program
    {
        static bool _cancelled;
        static void Main(string[] args)
        {
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                var culture = ConfigurationManager.AppSettings["DefaultCulture"];
                var watcher = new FileSystemWatcher();
                var filesToWatchHashTable = ConfigurationManager.GetSection("FoldersToWatch") as Hashtable;
                var dictionaryFilesToWatch = filesToWatchHashTable.Cast<DictionaryEntry>()
                    .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());
                foreach (var file in dictionaryFilesToWatch)
                {
                    watcher.Path = Path.GetFullPath(file.Value);
                }
                watcher.Filter += "*";
                watcher.Created += OnChanged;
                watcher.EnableRaisingEvents = true;
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {e.FullPath} {e.ChangeType}");
            string path = "";
            switch (Path.GetExtension(e.FullPath))
            {
                case ".jpg":
                {
                    path = $@"{ConfigurationManager.AppSettings["Pictures"]}";
                    break;
                }
                case ".txt":
                {
                    path = $@"{ConfigurationManager.AppSettings["Documents"]}";
                    break;
                }
                case ".docx":
                {
                    path = $@"{ConfigurationManager.AppSettings["Documents"]}";
                    break;
                }
                case ".pdf":
                {
                    path = $@"{ConfigurationManager.AppSettings["Documents"]}";
                    break;
                }
                case ".mp3":
                {
                    path = $@"{ConfigurationManager.AppSettings["Music"]}";
                    break;
                }
                default:
                {
                    path = $@"{ConfigurationManager.AppSettings["Default"]}";
                    break;
                }
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                File.Move(e.FullPath, $@"{path}\{e.Name}", true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
