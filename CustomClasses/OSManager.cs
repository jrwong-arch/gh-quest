using System.Collections.Generic;
using System.IO;
using Newtonsoft;
using Rhino;

namespace gh_quest.CustomClasses
{
    public class OSManager
    {
    
        public static string GetFilePath(string folderName, string fileName)
        {
            string folderPath = ".//" + folderName;
            string filePath = Path.Combine(folderPath, fileName);

            return filePath;
        }
    }
}