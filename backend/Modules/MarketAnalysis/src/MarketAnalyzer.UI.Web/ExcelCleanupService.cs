namespace MarketAnalyzer.UI.Web
{
    public class ExcelCleanupService : BackgroundService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ExcelCleanupService> _logger;

        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6);
        private readonly TimeSpan _fileMaxAge = TimeSpan.FromDays(2);

        public ExcelCleanupService(
            IWebHostEnvironment environment,
            ILogger<ExcelCleanupService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanupOldExcelFiles();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning old Excel files");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private void CleanupOldExcelFiles()
        {
            var excelRootPath = Path.Combine(_environment.ContentRootPath, "App_Data", "StrategyUploads");

            if (!Directory.Exists(excelRootPath))
                return;

            var now = DateTime.UtcNow;

            var files = Directory.GetFiles(excelRootPath, "*.xlsx", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (now - fileInfo.CreationTimeUtc > _fileMaxAge)
                {
                    try
                    {
                        fileInfo.Delete();

                        _logger.LogInformation("Deleted old Excel file: {File}", file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not delete Excel file: {File}", file);
                    }
                }
            }

            DeleteEmptyDirectories(excelRootPath);
        }

        private void DeleteEmptyDirectories(string rootPath)
        {
            var directories = Directory
                .GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                .OrderByDescending(x => x.Length);

            foreach (var directory in directories)
            {
                try
                {
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory);
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }
    }

}
