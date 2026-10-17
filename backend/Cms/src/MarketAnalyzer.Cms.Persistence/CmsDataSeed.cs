using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Persistence
{
    internal class CmsDataSeed : IDataSeed
    {
        private readonly IDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IServiceProvider _serviceProvider;

        public CmsDataSeed(IDbContext dbContext, IWebHostEnvironment environment, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _environment = environment;
            _serviceProvider = serviceProvider;
        }

        public int Order => 600;

        public async Task SeedAsync()
        {
            await SeedFileTypesAsync();
            await SeedFilePurposesAsync();
            await SeedMenusAsync();
            await SeedTemplatesAsync();
        }

        private async Task SeedMenusAsync()
        {
            //Clean old menus
            await _dbContext.Set<Menu>().ExecuteDeleteAsync();

            using var stream = typeof(CmsDataSeed).Assembly.GetManifestResourceStream(typeof(CmsDataSeed), "Menu.json");
            var entries = await System.Text.Json.JsonSerializer.DeserializeAsync<List<MenuEntry>>(stream);
            foreach (var entry in entries)
            {
                ProcessMenuEntry(entry, null);
            }

            void ProcessMenuEntry(MenuEntry entry, Menu parent)
            {
                var menu = Menu.Create(
                    id: entry.Id,
                    title: entry.Title,
                    path: entry.Path,
                    displayOrder: entry.DisplayOrder,
                    icon: entry.Icon,
                    requiredRoute: entry.RequiredRoute,
                    isActive: entry.IsActive,
                    requiredInvestmentTypes: entry.RequiredInvestmentTypes);

                if (entry.SubMenus?.Count > 0)
                {
                    foreach (var subMenu in entry.SubMenus)
                    {
                        ProcessMenuEntry(subMenu, menu);
                    }
                }

                _dbContext.Set<Menu>().Add(menu);
                parent?.AddSubMenu(menu);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedFileTypesAsync()
        {
            var entries = new List<FileType>()
            {
                FileType.Create(extension: "bmp", mimeType: "image/bmp"),
                FileType.Create(extension: "png", mimeType: "image/png"),
                FileType.Create(extension: "jpg", mimeType: "image/jpeg"),
                FileType.Create(extension: "jpeg", mimeType: "image/jpeg"),

                FileType.Create(extension: "zip", mimeType: "application/zip"),
                FileType.Create(extension: "rar", mimeType: "application/vnd.rar"),

                FileType.Create(extension: "pdf", mimeType: "application/pdf"),

                FileType.Create(extension: "dbs", mimeType: "application/xml"),
                FileType.Create(extension: "tck", mimeType: "application/xml"),

                FileType.Create(extension: "xls", mimeType: "application/vnd.ms-excel"),
                FileType.Create(extension: "xlsx", mimeType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                FileType.Create(extension: "docx", mimeType: "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            };

            var fileTypes = await _dbContext
                .Set<FileType>()
                .ToListAsync();

            foreach (var entry in entries)
            {
                var fileType = fileTypes.FirstOrDefault(e => e.Extension == entry.Extension);
                if (fileType is null)
                {
                    _dbContext.Set<FileType>().Add(entry);
                }
                else
                {
                    fileType.Update(mimeType: entry.MimeType);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedFilePurposesAsync()
        {
            var fileTypes = await _dbContext
                .Set<FileType>()
                .ToDictionaryAsync(e => e.Extension, e => e);

            var entries = new List<FilePurpose>()
            {
                FilePurpose.FrontNationalCard,
                FilePurpose.BackNationalCard,
                FilePurpose.BirthCertificateFirstPage,
                FilePurpose.BirthCertificateSecondPage,
                FilePurpose.BirthCertificateThirdPage,
                FilePurpose.Passport,
                FilePurpose.InvestorSignature,
                FilePurpose.StatuteChanges,
                FilePurpose.ProspectusChanges,
                FilePurpose.DepositSlip,
                FilePurpose.TckTradeFile,
                FilePurpose.DbsTradeFile,
                FilePurpose.StockPriceAdjustment,
                FilePurpose.NewsAttachment,
                FilePurpose.PillarRole,
                FilePurpose.InvestmentPolicyStatement,
                FilePurpose.CompanyBankAccount
            };

            void AddAcceptableType(FilePurpose purpose)
            {
                const int MegaByte = 1024 * 1024;
                if (purpose == FilePurpose.FrontNationalCard ||
                    purpose == FilePurpose.BackNationalCard ||
                    purpose == FilePurpose.BirthCertificateFirstPage ||
                    purpose == FilePurpose.BirthCertificateSecondPage ||
                    purpose == FilePurpose.BirthCertificateThirdPage ||
                    purpose == FilePurpose.Passport ||
                    purpose == FilePurpose.InvestorSignature)
                {
                    purpose.AddFileType(fileTypes["pdf"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["jpg"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["jpeg"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["png"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["bmp"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["zip"], 10 * MegaByte);
                    purpose.AddFileType(fileTypes["rar"], 10 * MegaByte);
                }
                else if (purpose == FilePurpose.StatuteChanges || purpose == FilePurpose.ProspectusChanges || purpose == FilePurpose.PillarRole || purpose == FilePurpose.InvestmentPolicyStatement)
                {
                    purpose.AddFileType(fileTypes["pdf"], 15 * MegaByte);
                }
                else if (purpose == FilePurpose.DepositSlip)
                {
                    purpose.AddFileType(fileTypes["pdf"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["jpg"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["jpeg"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["png"], 5 * MegaByte);
                    purpose.AddFileType(fileTypes["bmp"], 5 * MegaByte);
                }
                else if (purpose == FilePurpose.TckTradeFile)
                {
                    purpose.AddFileType(fileTypes["tck"], 50 * MegaByte);
                }
                else if (purpose == FilePurpose.DbsTradeFile)
                {
                    purpose.AddFileType(fileTypes["dbs"], 50 * MegaByte);
                }
                else if (purpose == FilePurpose.StockPriceAdjustment)
                {
                    purpose.AddFileType(fileTypes["xls"], 50 * MegaByte);
                    purpose.AddFileType(fileTypes["xlsx"], 50 * MegaByte);
                }
                else if (purpose == FilePurpose.NewsAttachment)
                {
                    purpose.AddFileType(fileTypes["pdf"], 0);
                    purpose.AddFileType(fileTypes["jpg"], 0);
                    purpose.AddFileType(fileTypes["jpeg"], 0);
                    purpose.AddFileType(fileTypes["png"], 0);
                    purpose.AddFileType(fileTypes["bmp"], 0);
                    purpose.AddFileType(fileTypes["zip"], 0);
                    purpose.AddFileType(fileTypes["rar"], 0);
                    purpose.AddFileType(fileTypes["xls"], 0);
                    purpose.AddFileType(fileTypes["xlsx"], 0);
                    purpose.AddFileType(fileTypes["docx"], 0);
                }
                else if (purpose == FilePurpose.CompanyBankAccount)
                {
                    purpose.AddFileType(fileTypes["pdf"], 0);
                    purpose.AddFileType(fileTypes["jpg"], 0);
                    purpose.AddFileType(fileTypes["jpeg"], 0);
                    purpose.AddFileType(fileTypes["png"], 0);
                    purpose.AddFileType(fileTypes["bmp"], 0);
                    purpose.AddFileType(fileTypes["zip"], 0);
                    purpose.AddFileType(fileTypes["rar"], 0);
                    purpose.AddFileType(fileTypes["xls"], 0);
                    purpose.AddFileType(fileTypes["xlsx"], 0);
                    purpose.AddFileType(fileTypes["docx"], 0);
                }
                else
                {
                    throw new NotSupportedException($"Not supported FilePurpose: {purpose.Name}");
                }
            }

            var purposes = await _dbContext
                .Set<FilePurpose>()
                .Include(e => e.AcceptableTypes)
                .ToListAsync();

            foreach (var entry in entries)
            {
                var purpose = purposes.FirstOrDefault(e => e.Id == entry.Id);
                if (purpose is null)
                {
                    purpose = entry;
                    AddAcceptableType(purpose);
                    _dbContext.Set<FilePurpose>().Add(purpose);
                }
                else
                {
                    purpose.Update(entry.Name);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedTemplatesAsync()
        {
            var knownFiles = new Dictionary<string, KnownFileInfo>()
            {
                { "BatchRequestsTemplate.xlsx", new KnownFileInfo() { Id = CmsConsts.BatchRequestsTemplateId, Name = "فایل نمونه درخواست صدور/ابطال دسته‌ای" } },
                { "BatchInvestors", new KnownFileInfo() { Id = CmsConsts.BatchInvestorsTemplateId, Name = "فایل نمونه ثبت نام سرمایه گذار دسته‌ای" } },
                { "ImportTradesExcelTemplate.xlsx", new KnownFileInfo() { Id = CmsConsts.ImportTradesExcelTemplateId, Name = "فایل نمونه اکسل معاملات" } },
                { "ImportTradesJson", new KnownFileInfo() { Id = CmsConsts.ImportTradesJsonTemplateId, Name = "فایل نمونه جیسون معاملات" } },
                { "CreateUnderwritingRequestTemplate.xlsx", new KnownFileInfo() { Id = CmsConsts.CreateUnderwritingRequestTemplateId, Name = "فایل نمونه اکسل درخواست پذیره‌نویسی دسته‌ای" } },
            };
            var directory = Path.Combine(_environment.ContentRootPath, "Templates");
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                return;
            }

            var fileTypes = await _dbContext
                .Set<FileType>()
                .ToListAsync();

            var storedFiles = await _dbContext
                .Set<FileStore>()
                .Where(e => knownFiles.Values.Select(f => f.Id).Contains(e.Id))
                .ToListAsync();

            var fileStores = new List<FileStore>(files.Length);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!knownFiles.TryGetValue(fileName, out var fileInfo))
                {
                    throw new Exception($"The file [{fileName}] is not known.");
                }

                var extension = Path.GetExtension(file)?.Replace(".", "");
                var fileType = fileTypes.FirstOrDefault(e => e.Extension == extension);
                if (fileType is null)
                {
                    throw new Exception($"The file type for [{extension}] not fund.");
                }
                var content = await File.ReadAllBytesAsync(file);
                fileStores.Add(FileStore.Create(
                    id: fileInfo.Id,
                    name: fileInfo.Name,
                    fileSize: content.Length,
                    fileType: fileType,
                    content: content));
            }

            foreach (var entry in fileStores)
            {
                var file = storedFiles.FirstOrDefault(e => e.Id == entry.Id);
                if (file is null)
                {
                    _dbContext.Set<FileStore>().Add(entry);
                }
                else
                {
                    file.Update(
                        name: entry.Name,
                        fileSize: entry.FileSize,
                        fileType: entry.FileType,
                        content: entry.Content);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        #region [Private Types]

        private class KnownFileInfo
        {
            public required string Id { get; init; }
            public required string Name { get; init; }
        }

        private class MenuEntry
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Path { get; set; }
            public int DisplayOrder { get; set; }
            public string Icon { get; set; }
            public bool IsActive { get; set; }
            public string RequiredRoute { get; set; }
            public List<MenuEntry> SubMenus { get; set; }
            public List<int> RequiredInvestmentTypes { get; set; }
        }

        #endregion
    }
}
