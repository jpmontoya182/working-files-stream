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
            byte[] data = File.ReadAllBytes(InputFilePath);
            // look at by the maximun hexadecimal on data.
            byte largest = data.Max();
            // create a new byte[] with additional byte
            byte[] newData = new byte[data.Length + 1];

            Array.Copy(data, newData, data.Length);
            // copy the largest hexadecimal on the new array
            newData[newData.Length - 1] = largest;

            File.WriteAllBytes(OuputFilePath, newData);
        }
    }
}
