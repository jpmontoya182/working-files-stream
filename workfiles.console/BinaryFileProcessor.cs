using System;
using System.IO;
using System.Linq;

namespace workfiles.console
{
    class BinaryFileProcessor
    {
        public string InputFilePath { get; }
        public string OuputFilePath { get; }
        public BinaryFileProcessor(string inputFilePath, string ouputFilePath)
        {
            InputFilePath = inputFilePath;
            OuputFilePath = ouputFilePath;
        }

        public void Process()
        {
            //byte[] data = File.ReadAllBytes(InputFilePath);
            //// look at by the maximun hexadecimal on data.
            //byte largest = data.Max();
            //// create a new byte[] with additional byte
            //byte[] newData = new byte[data.Length + 1];

            //Array.Copy(data, newData, data.Length);
            //// copy the largest hexadecimal on the new array
            //newData[newData.Length - 1] = largest;

            //File.WriteAllBytes(OuputFilePath, newData);

            ///////// ***************

            //using FileStream inputFilStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
            //using FileStream outputFileStream = File.OpenWrite(OuputFilePath);

            //const int endOfStream = -1;
            //int largestByte = 0;

            //int currentByte = inputFilStream.ReadByte();
            //while (currentByte != endOfStream)
            //{
            //    outputFileStream.WriteByte((byte)currentByte);

            //    if (currentByte > largestByte)
            //    {
            //        largestByte = currentByte;
            //    }
            //    currentByte = inputFilStream.ReadByte();
            //}
            //outputFileStream.WriteByte((byte)largestByte);

            ///////// ***************
            
            using FileStream inputFilStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader binaryReader = new BinaryReader(inputFilStream);
            using FileStream outputFileStream = File.OpenWrite(OuputFilePath);
            using BinaryWriter binaryWriter = new BinaryWriter(outputFileStream);

            int largest = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                int currentByte = binaryReader.ReadByte();
                binaryWriter.Write(currentByte);

                if (currentByte > largest)
                {
                    largest = currentByte;
                }
            }
            binaryWriter.Write(largest);
        }
    }
}
