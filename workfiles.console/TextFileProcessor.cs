using System;
using System.IO;
using System.Text;

namespace workfiles.console
{
    public class TextFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get;  }
        public TextFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }
        public void Process()
        {
            ///  using read all text 
            //string originalText = File.ReadAllText(InputFilePath);
            //string processedText = originalText.ToUpperInvariant();
            //File.WriteAllText(OutputFilePath, processedText);

            /// using read all lines
            //string[] lines = File.ReadAllLines(InputFilePath, Encoding.UTF8);
            //lines[1] = lines[1].ToUpperInvariant();
            //try
            //{
            //    File.WriteAllLines(OutputFilePath, lines, Encoding.UTF8);
            //}
            //catch (Exception ex)
            //{
            //    throw ;
            //}

            // using var inputFileStream = new FileStream(InputFilePath, FileMode.Open);
            using StreamReader inputStreamReader = File.OpenText(InputFilePath);

            // using var outputFileStream = new FileStream(OutputFilePath, FileMode.CreateNew);
            using var outputStreamWriter = new StreamWriter(OutputFilePath);
            var currentLineNumber = 1;
            while (!inputStreamReader.EndOfStream)
            {
                string inputLine = inputStreamReader.ReadLine();

                if (currentLineNumber == 2)
                {
                    inputLine = inputLine.ToUpperInvariant();
                }

                if (inputStreamReader.EndOfStream)
                {
                    outputStreamWriter.Write(inputLine);
                }
                else
                { 
                    outputStreamWriter.WriteLine(inputLine);
                }

                currentLineNumber++;
            }           
        }
    }
}
