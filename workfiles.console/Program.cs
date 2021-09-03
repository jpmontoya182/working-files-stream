using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using static System.Console;

namespace workfiles.console
{
    class Program
    {
        private static MemoryCache FilesToProcess = MemoryCache.Default;
        //*** private static ConcurrentDictionary<string, string> FileToProcess = new ConcurrentDictionary<string, string>();
        static void Main(string[] args)
        {
            WriteLine("Parsing command line options");
            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR : {directoryToWatch} does not exists");
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes.");

                ProccessExistingFiles(directoryToWatch);

                using var inputFileWatcher = new FileSystemWatcher(directoryToWatch);
                //*** using var timer = new Timer(ProcessFiles, null, 0, 1000);
                // setup
                inputFileWatcher.IncludeSubdirectories = false;
                inputFileWatcher.InternalBufferSize = 32768; // 32KB
                inputFileWatcher.Filter = "*.*"; // this is the default.
                inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                inputFileWatcher.Created += FileCreated;
                inputFileWatcher.Changed += FileChanged;
                inputFileWatcher.Deleted += FileDeleted;
                inputFileWatcher.Renamed += FileRenamed;
                inputFileWatcher.Error += WatcherError;

                inputFileWatcher.EnableRaisingEvents = true;

                WriteLine("Press enter to quit");
                ReadLine();
            }
        }
        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created : {e.Name} - type: {e.ChangeType} *");

            //*** FileToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }
        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed : {e.Name} - type: {e.ChangeType}");

            //*** FileToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);

        }
        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File deleted : {e.Name} - type: {e.ChangeType}");
        }
        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed : {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }
        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be active {e.GetException()}");
        }
        private static void ProcessWithArgumentsInCall(string[] args)
        {
            var command = args[0];

            if (command == "--file")
            {
                var filePath = args[1];
                // check if path is absolute
                if (!Path.IsPathFullyQualified(filePath))
                {
                    WriteLine($"ERROR: path '{filePath}' must be qualified.");
                    ReadLine();
                    return;
                }
                WriteLine($"Single file {filePath} selected");
                ProcessSingleFile(filePath);
            }
            else if (command == "--dir")
            {
                var directoryPath = args[1];
                var fileType = args[2];
                ProcessDirectory(directoryPath, fileType);
            }
            else
            {
                WriteLine("Invalid command line options");
            }
            WriteLine("Press enter to quit");
            ReadLine();
        }
        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }
        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            switch (fileType)
            {
                case "TEXT":
                    string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFilePath in txtFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported");
                    break;
            }



        }
        // *** this method process files on the concurrent collection
        //private  static void ProcessFiles(object stateInfo)
        //{
        //    foreach (var fileName in FileToProcess.Keys)
        //    {
        //        if (FileToProcess.TryRemove(fileName, out _))
        //        {
        //            var fileProcessor = new FileProcessor(fileName);
        //            fileProcessor.Process();
        //        }
        //    }
        //}
        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);
            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };
            FilesToProcess.Add(item, policy);
        }
        private static void ProcessFile(CacheEntryRemovedArguments args)
        {
            WriteLine($"* Cache item removed : {args.CacheItem.Key} because {args.RemovedReason}");

            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(args.CacheItem.Key);
                fileProcessor.Process();
            }

        }
        private static void ProccessExistingFiles(string inputDirectory)
        {
            WriteLine($"Checking {inputDirectory} for existing files");

            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                WriteLine($"   - Found {filePath}");
                AddToCache(filePath);
            }
        }
    }
}
