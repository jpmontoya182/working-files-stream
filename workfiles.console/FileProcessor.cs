using System;
using System.IO;
using static System.Console;

namespace workfiles.console
{
    class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompletedDirectoryName = "complete";

        public string InputFilePath { get; set; }

        public FileProcessor(string filePath) => InputFilePath = filePath;
        public void Process()
        {
            WriteLine($"Begin process of {InputFilePath}");

            // check if file exists
            if (!File.Exists(InputFilePath))
            {
                WriteLine($"ERROR: file {InputFilePath} does not exist.");
                return;
            }

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            WriteLine($"Root data path is {rootDirectoryPath}");

            // check if backup dir exists
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);
            if (!Directory.Exists(backupDirectoryPath))
            {
                WriteLine($"Attemptig to create {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            }

            // copy file to backup dir
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);

            WriteLine($"Copying {inputFileName} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);

            // Move to in progress dir
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgessFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            WriteLine($"Moving {InputFilePath} to {inProgessFilePath}");
            File.Move(InputFilePath, inProgessFilePath, true);

            // determine type of file
            string extension = Path.GetExtension(inputFileName);

            // Move file after processing is complete 
            string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);
            WriteLine($"Moving {inProgessFilePath} to {completedDirectoryPath}");

            string completedFileName = $"{Path.GetFileNameWithoutExtension(inputFileName)}--{Guid.NewGuid()}{extension}";
            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

            switch (extension)
            {
                case ".txt":
                    var textProcessor = new TextFileProcessor(inProgessFilePath, completedFilePath);
                    textProcessor.Process();
                    break;
                case ".data":
                    var binaryProcessor = new BinaryFileProcessor(inProgessFilePath, completedFilePath);
                    binaryProcessor.Process();
                    break;
                case ".csv":
                    var csvProcessor = new CsvFileProcessor(inProgessFilePath, completedFilePath);
                    csvProcessor.Process();
                    break;
                default:
                    WriteLine($"{extension} is an unsupported file type");
                    break;
            }

            WriteLine($"Completed processing of {inProgessFilePath}");
            WriteLine($"Deleting {inProgessFilePath}");
            File.Delete(inProgessFilePath);



        }


    }
}
