using Agility.Sdk.Model.Capture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalAgility.Sdk;

namespace EnhancedWorkspace
{
    public class Test
    {

        public void WriteByteArrayToFile(byte[] bytearray, string filePath)
        {
            File.WriteAllBytes(filePath, bytearray);
        }

        public void WriteIntArrayToFile(int[] bytearray, string filePath)
        {
            byte[] bytes = bytearray.SelectMany(BitConverter.GetBytes).ToArray();
            File.WriteAllBytes(filePath, bytes.ToArray<byte>());
        }
        public void WriteLongArrayToFile(long[] bytearray, string filePath)
        {
            byte[] bytes = bytearray.SelectMany(BitConverter.GetBytes).ToArray();
            File.WriteAllBytes(filePath, bytes.ToArray<byte>());
        }
    }
}
