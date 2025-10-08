using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _3DPrintProjectTracker
{
    internal class MainScreen
    {
        public string CreateNewLocalDB()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\test.json";
            string fileContents = "{}";
            File.WriteAllText(filePath, fileContents);

            return filePath;
        }
    }
}
