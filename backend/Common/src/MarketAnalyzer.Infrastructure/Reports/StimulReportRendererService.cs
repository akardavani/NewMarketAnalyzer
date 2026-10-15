using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MarketAnalyzer.Abstractions;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;

namespace MarketAnalyzer.Infrastructure.Reports
{
    internal class StimulReportRendererService : IReportRendererService, IExcelRenderer
    {
        public const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly ReportsOptions _options;
        private readonly IWebHostEnvironment _environment;
        private readonly IExcelDocumentFactory _documentFactory;
        private readonly Dictionary<ReportFormat, string> _fileExtensions = new()
        {
            { ReportFormat.Html, "txt" },
            { ReportFormat.Pdf, "pdf" },
            { ReportFormat.Excel, "xlsx" }
        };
        private readonly Dictionary<ReportFormat, string> _mimeTypes = new()
        {
            { ReportFormat.Html, "text/plain" },
            { ReportFormat.Pdf, "application/pdf" },
            { ReportFormat.Excel, ExcelMimeType }
        };
        private readonly Dictionary<ReportFormat, Action<StiReport, Stream>> _rendereres;

        public StimulReportRendererService(IOptions<ReportsOptions> options, IWebHostEnvironment environment, IExcelDocumentFactory documentFactory)
        {
            _options = options.Value;
            _environment = environment;
            _documentFactory = documentFactory;
            StiLicense.LoadFromString(_options.LicenseKey);
            StiOptions.Export.Excel.ColumnsRightToLeft = true;
            LoadFonts();

            _rendereres = new Dictionary<ReportFormat, Action<StiReport, Stream>>()
            {
                { ReportFormat.Pdf, RenderPdf },
                { ReportFormat.Excel, RenderExcel }
            };
        }

        public async Task<ReportResult> RenderAsync(string fileName, Func<IExcelDocument, Task<byte[]>> action, CancellationToken cancellation)
        {
            using var fs = CreateFileStream(ReportFormat.Excel, fileName);
            using var doc = _documentFactory.Create(fs);
            return new ReportResult
            {
                Data = await action(doc),
                ContentType = ExcelMimeType,
                FileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.ToPersian("yyyyMMddHHmmssffff")}.xlsx"
            };
        }

        public async Task<ReportResult> RenderAsync(RenderReportRequest request, CancellationToken cancellationToken)
        {
            using var fs = CreateFileStream(request.Format, request.FileName);
            var report = new StiReport();
            report.Load(fs);
            report.RegBusinessObject("Records", request.Data);
            await report.Dictionary.SynchronizeAsync();
            PrepareVariables(report, request.Variables);
            await report.RenderAsync();
            var stream = new MemoryStream();
            if (_rendereres.TryGetValue(request.Format, out var renderer))
            {
                renderer(report, stream);
            }
            else
            {
                RenderString(report, stream);
            }

            if (!_fileExtensions.TryGetValue(request.Format, out var fileExtension))
            {
                fileExtension = "txt";
            }
            if (!_mimeTypes.TryGetValue(request.Format, out var contentType))
            {
                contentType = "text/plain";
            }

            return new ReportResult
            {
                Data = stream.ToArray(),
                ContentType = contentType,
                FileName = !string.IsNullOrEmpty(request.DownloadFileName) ?
                    request.DownloadFileName :
                    $"{Path.GetFileNameWithoutExtension(request.FileName)}_{DateTime.Now.ToPersian("yyyyMMddHHmmssffff")}.{fileExtension}"
            };
        }

        private Stream CreateFileStream(ReportFormat format, string fileName)
        {
            if (!_options.FormatDirectoryMap.TryGetValue(format, out string directory))
            {
                directory = _options.SharedDirectory;
            }

            var extension = format == ReportFormat.Excel ? "xlsx" : "mrt";
            var fileInfo = _environment
                .ContentRootFileProvider
                .GetFileInfo(Path.Combine(_options.BasePath, directory, $"{fileName}.{extension}"));
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(message: "File not fount in path.", fileName: fileInfo.PhysicalPath);
            }

            return fileInfo.CreateReadStream();
        }

        private void PrepareVariables(StiReport report, IReadOnlyDictionary<string, object> variables)
        {
            if (variables is not null)
            {
                foreach (var variable in variables)
                {
                    report.Dictionary.Variables.Add(variable.Key, variable.Value);
                }
            }
            report.Dictionary.Variables["CompanyName"].Value = _options.CompanyName;
            report.Dictionary.Variables["ReportDate"].Value = DateTime.Now.ToPersian();
            report.Dictionary.Variables["ReportTime"].Value = DateTime.Now.ToString("HH:mm:ss");
        }

        private void RenderPdf(StiReport report, Stream stream)
        {
            var settings = new StiPdfExportSettings
            {
                AllowEditable = StiPdfAllowEditable.No,
                ImageCompressionMethod = StiPdfImageCompressionMethod.Jpeg,
                ImageFormat = StiImageFormat.Color,
                ImageQuality = 100,
                ImageResolutionMode = StiImageResolutionMode.Exactly,
                UseUnicode = true
            };
            var service = new StiPdfExportService();
            service.ExportPdf(report, stream, settings);
        }

        private void RenderExcel(StiReport report, Stream stream)
        {
            var settings = new StiExcel2007ExportSettings
            {
                ImageQuality = 100,
                ExportEachPageToSheet = true,
                UseOnePageHeaderAndFooter = true,
            };
            var service = new StiExcel2007ExportService();
            service.ExportExcel(report, stream, settings);
        }

        private static void RenderString(StiReport report, MemoryStream stream)
        {
            var content = report.SaveDocumentToString();
            stream.Write(Encoding.UTF8.GetBytes(content));
        }

        //private async Task LoadFontsAsync()
        //{
        //    var type = typeof(StimulReportRendererService);
        //    var fonts = typeof(StimulReportRendererService).Assembly.GetManifestResourceNames();
        //    foreach (var font in fonts)
        //    {
        //        using var stream = type.Assembly.GetManifestResourceStream(font);
        //        using var reader = new MemoryStream();
        //        await stream.CopyToAsync(reader);
        //        var content = reader.ToArray();
        //        StiFontCollection.AddFontBytes(content, Path.GetFileNameWithoutExtension(font));
        //        FontsInfoStore.LoadFontInfoToStore(Path.GetFileNameWithoutExtension(font), content);
        //    }
        //}

        private void LoadFonts()
        {
            var fontsPath = Path.Combine(_environment.ContentRootPath, "Reports", "Fonts");
            var fonts = Directory.GetFiles(fontsPath, "*.ttf", SearchOption.AllDirectories);
            foreach (var font in fonts)
            {
                //var content = File.ReadAllBytes(font);
                //var resource = new StiResource("Roboto-Black", "Roboto-Black", false, StiResourceType.FontTtf, content, false);
                //report.Dictionary.Resources.Add(resource);
                //Adding a font from resources to the font collection
                //StiFontCollection.AddResourceFont(resource.Name, resource.Content, "ttf", resource.Alias);

                StiFontCollection.AddFontFile(font);
            }
        }
    }
}
