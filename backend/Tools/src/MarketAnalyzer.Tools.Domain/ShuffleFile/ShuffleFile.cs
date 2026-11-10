namespace MarketAnalyzer.Tools.Domain
{
    public static class ShuffleFile
    {
        public static (int allFiles, int filesUsed) Start(
            List<VideoFileInfo> jsonFile,
            int maxFolderCount,
            int maxFolderTime,
            string StartFinalName,
            string selectedFinalPath)
        {
            if (jsonFile == null)
                throw new ArgumentNullException(nameof(jsonFile));

            if (maxFolderCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxFolderCount),
                    "تعداد پوشه‌ها باید بیشتر از صفر باشد.");
            }

            if (maxFolderTime <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxFolderTime),
                    "مدت هدف باید بیشتر از صفر باشد.");
            }

            if (string.IsNullOrWhiteSpace(selectedFinalPath))
            {
                throw new ArgumentException(
                    "مسیر مقصد مشخص نشده است.",
                    nameof(selectedFinalPath));
            }

            if (string.IsNullOrWhiteSpace(StartFinalName))
                StartFinalName = "Mix-";

            if (StartFinalName.IndexOfAny(
                    Path.GetInvalidFileNameChars()) >= 0
                || StartFinalName.Contains("/")
                || StartFinalName.Contains("\\"))
            {
                throw new ArgumentException(
                    "پیشوند پوشه دارای کاراکتر غیرمجاز است.",
                    nameof(StartFinalName));
            }

            if (jsonFile.Any(file => file == null))
            {
                throw new ArgumentException(
                    "فهرست فایل‌ها دارای عضو null است.",
                    nameof(jsonFile));
            }

            const string historyFileName = "FilesInfo";

            var allFiles = jsonFile.Count;
            var filesUsed = jsonFile.Count(file => file.IsUsed);

            if (allFiles == 0)
                return (allFiles, filesUsed);

            var random = new Random();
            var folders = new List<FolderInfo>();

            // فایل‌های انتخاب‌شده در این اجرا.
            // تا پایان کپی، IsUsed آن‌ها تغییر نمی‌کند.
            var selectedFiles = new HashSet<VideoFileInfo>();

            for (var i = 1; i <= maxFolderCount; i++)
            {
                var folder = new FolderInfo
                {
                    Name = $"{StartFinalName}{i}",
                    Duration = 0,
                    IsComplete = false,
                    FilesInfo = new List<VideoFileInfo>()
                };

                var usedCategories = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

                while (folder.Duration < maxFolderTime)
                {
                    var candidates = jsonFile
                        .Where(file =>
                            !file.IsUsed
                            && !selectedFiles.Contains(file)
                            && file.Duration > 0
                            && !usedCategories.Contains(
                                file.RootFolderName))
                        .ToList();

                    // فایل مناسب دیگری برای این پوشه نداریم.
                    if (candidates.Count == 0)
                        break;

                    var selected = candidates[
                        random.Next(candidates.Count)];

                    folder.FilesInfo.Add(selected);
                    folder.Duration += selected.Duration;

                    usedCategories.Add(selected.RootFolderName);
                    selectedFiles.Add(selected);
                }

                folder.IsComplete =
                    folder.Duration >= maxFolderTime;

                // پوشهٔ خالی به عملیات کپی ارسال نمی‌شود.
                if (folder.FilesInfo.Count > 0)
                    folders.Add(folder);
            }

            if (selectedFiles.Count == 0)
                return (allFiles, filesUsed);

            // پیش از شروع کپی، وجود فایل‌های انتخاب‌شده بررسی شود.
            foreach (var file in selectedFiles)
            {
                if (string.IsNullOrWhiteSpace(file.FilePath)
                    || !System.IO.File.Exists(file.FilePath))
                {
                    throw new FileNotFoundException(
                        "یکی از فایل‌های انتخاب‌شده وجود ندارد.",
                        file.FilePath);
                }
            }

            Directory.CreateDirectory(selectedFinalPath);

            // استفاده از تابع کپی موجود در پروژهٔ قدیمی.
            FileInformation.Copy(folders, selectedFinalPath);

            // فقط پس از برگشت موفق تابع کپی، سابقه تغییر می‌کند.
            foreach (var file in selectedFiles)
                file.IsUsed = true;

            JsonConvertor.WriteJsonData(
                jsonFile,
                historyFileName);

            filesUsed = jsonFile.Count(file => file.IsUsed);

            return (allFiles, filesUsed);
        }
    }

    //public static class RandomNumber
    //{
    //    public static VideoFileInfo Create(
    //        List<VideoFileInfo> files,
    //        List<string> folders)
    //    {
    //        if (files == null)
    //            throw new ArgumentNullException(nameof(files));

    //        if (folders == null)
    //            throw new ArgumentNullException(nameof(folders));

    //        var usedCategories = new HashSet<string>(
    //            folders,
    //            StringComparer.OrdinalIgnoreCase);

    //        var candidates = files
    //            .Where(file =>
    //                !file.IsUsed &&
    //                file.Duration > 0 &&
    //                !usedCategories.Contains(file.RootFolderName))
    //            .ToList();

    //        if (candidates.Count == 0)
    //        {
    //            throw new InvalidOperationException(
    //                "ویدئوی استفاده‌نشده از دستهٔ جدید باقی نمانده است. " +
    //                "تعداد پوشه‌ها یا مدت هدف را کاهش دهید.");
    //        }

    //        var selected = candidates[
    //            Random.Shared.Next(candidates.Count)];

    //        selected.IsUsed = true;

    //        return selected;
    //    }
    //}
}
