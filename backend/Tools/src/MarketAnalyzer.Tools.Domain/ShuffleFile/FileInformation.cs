namespace MarketAnalyzer.Tools.Domain
{
    public static class FileInformation
    {
        public static void Copy(
            List<FolderInfo> folders,
            string selectedFinalPath)
        {
            if (folders == null)
                throw new ArgumentNullException(nameof(folders));

            if (string.IsNullOrWhiteSpace(selectedFinalPath))
            {
                throw new ArgumentException(
                    "مسیر مقصد مشخص نشده است.",
                    nameof(selectedFinalPath));
            }

            var outputRoot = Path.GetFullPath(selectedFinalPath);

            Directory.CreateDirectory(outputRoot);

            foreach (var folder in folders)
            {
                if (folder == null)
                    throw new ArgumentException("اطلاعات پوشه نامعتبر است.");

                ValidateFolderName(folder.Name);

                if (folder.FilesInfo == null || folder.FilesInfo.Count == 0)
                    continue;

                var destinationFolder = Path.Combine(
                    outputRoot,
                    folder.Name);

                Directory.CreateDirectory(destinationFolder);

                foreach (var item in folder.FilesInfo)
                {
                    if (item == null ||
                        string.IsNullOrWhiteSpace(item.FilePath))
                    {
                        throw new ArgumentException(
                            "مسیر یکی از فایل‌ها نامعتبر است.");
                    }

                    var sourcePath = Path.GetFullPath(item.FilePath);

                    if (!System.IO.File.Exists(sourcePath))
                    {
                        throw new FileNotFoundException(
                            "فایل مبدأ پیدا نشد.",
                            sourcePath);
                    }

                    var originalName = Path.GetFileName(sourcePath);

                    var destinationName =
                        $"{Guid.NewGuid():N} - {originalName}";

                    var destinationPath = Path.Combine(
                        destinationFolder,
                        destinationName);

                    System.IO.File.Copy(
                        sourcePath,
                        destinationPath,
                        overwrite: false);
                }
            }
        }

        private static void ValidateFolderName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name == "." ||
                name == ".." ||
                name.Contains("/") ||
                name.Contains("\\") ||
                name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException(
                    "نام پوشهٔ خروجی نامعتبر است.");
            }
        }

    }
}
