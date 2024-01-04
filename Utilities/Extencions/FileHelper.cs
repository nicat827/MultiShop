namespace MultiShop.Utilities.Extencions
{
    public static class FileHelper
    {
        public static bool CheckFileType(this IFormFile file)
        {
            if (file.ContentType.Contains("image/")) return true;
            return false;
        }
        public static bool CheckFileSize(this IFormFile file, int maxSize)
        {
            if (file.Length <= maxSize * 1024) return true;
            return false;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, string rootPath, params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string path = rootPath;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        public static void DeleteFile(this string fileName, string rootPath, params string[] folders) 
        {
            string path = rootPath;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, fileName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
