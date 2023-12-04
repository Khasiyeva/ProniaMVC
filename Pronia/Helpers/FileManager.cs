namespace Pronia.Helpers
{
    public static class FileManager
    {

        public static string Upload(this IFormFile file,string envPath,string folderName)
        {
            string fileName = file.FileName;
            if (fileName.Length > 64)
            {
                fileName = fileName.Substring(fileName.Length-64);

            }
            fileName = Guid.NewGuid().ToString() + fileName;

            string path = envPath + folderName + fileName;
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }
        public static bool CheckContent(this IFormFile file, string content)
        {
            return file.ContentType.Contains(content);
        }
        public static bool CheckLenght(this IFormFile file, int lenght)
        {
            return file.Length <= lenght*1024;
        }

        public static void DeleteFile(this string imgUrl, string envPath, string folderName)
        {
            string path = envPath + folderName + imgUrl;

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void UpdateFile(IFormFile updatedFile,string imgUrl, string envPath, string folderName)
        {
            string path = envPath + folderName + imgUrl;

            if (File.Exists(path))
            {
                File.Delete(path);
                string newFileName = Upload(updatedFile, envPath, folderName);

            }
        }

    }
}
