using static CK3_GEDCOM.Helpers;

namespace CK3_GEDCOM.FileWriting
{
    class LogFileWriter : FileWriter
    {
        public static void DeleteLogFile()
        {
            string fileName = "Log.txt";
            string path = GetPath("Output", fileName);
            ClearFile(path);
        }

        public static void WriteToLog(string text)
        {
            string fileName = "Log.txt";
            string path = GetPath("Output", fileName);

            AppendToFile(path, text + NL);
        }
    }
}
