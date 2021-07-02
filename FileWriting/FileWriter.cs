using System;
using System.IO;
using System.Text;

namespace CK3_GEDCOM.FileWriting
{
    class FileWriter
    {
        protected static void WriteFile(string path, string contents)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try {
                File.WriteAllText(path, contents);
            }
            catch (UnauthorizedAccessException)
            {
                Program.AddError($"Cannot write file {path} due to lacking authorisation! Try restarting the tool in administrator mode.");
            }
        }
        protected static void ClearFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Delete(path);
        }
        protected static void AppendToFile(string path, string contents)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                File.AppendAllText(path, contents);
            }
            catch (UnauthorizedAccessException)
            {
                Program.AddError($"Cannot write to file {path} due to lacking authorisation! Try restarting the tool in administrator mode.");
            }
        }

        protected static string GetPath(string folder, string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
        }
    }
}
