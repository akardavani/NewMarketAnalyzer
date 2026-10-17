using MarketAnalyzer.AnalysisStrategy;
using MarketAnalyzer.Domain;
using MarketAnalyzer.Analysis.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketAnalyzer.UI.Web.Pages
{
    public class RunModel : PageModel
    {
        private readonly IAnalysisEngine _engine;
        private readonly StrategyUiAdapterRegistry _adapterRegistry;
        private readonly StrategyInputRegistry _inputRegistry;
        private readonly IWebHostEnvironment _environment;

        public RunModel(
            IAnalysisEngine engine,
            StrategyUiAdapterRegistry adapterRegistry,
            StrategyInputRegistry inputRegistry,
            IWebHostEnvironment environment)
        {
            _engine = engine;
            _adapterRegistry = adapterRegistry;
            _inputRegistry = inputRegistry;
            _environment = environment;
        }

        [BindProperty]
        public StrategyRunInputModel Input { get; set; } = new();

        public List<StrategyInputDefinition> Strategies { get; set; } = new();

        public StrategyInputDefinition? SelectedStrategyDefinition { get; set; }

        public UiPageResult? PageResult { get; set; }

        public async Task OnGet(string? strategy = null)
        {
            await LoadStrategies(strategy);
        }

        public async Task<IActionResult> OnGetStrategyFormAsync(string strategy)
        {
            await LoadStrategies(strategy);

            if (SelectedStrategyDefinition == null)
            {
                return Content("");
            }

            var viewModel = new StrategyParameterFormViewModel
            {
                Definition = SelectedStrategyDefinition,
                Input = Input
            };

            return Partial("_StrategyParameterForm", viewModel);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadStrategies(Input.Strategy);

            if (SelectedStrategyDefinition == null)
            {
                ModelState.AddModelError("", "Strategy انتخاب‌شده معتبر نیست.");
                return Page();
            }

            var parameters = await BuildParametersAsync(
                SelectedStrategyDefinition,
                Input.Parameters);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new AnalysisRequest
            {
                Strategy = Input.Strategy,
                Parameters = parameters
            };

            var result = await _engine.RunAsync(request, CancellationToken.None);

            PageResult = _adapterRegistry.Convert(result);

            return Page();
        }

        private async Task LoadStrategies(string? selectedStrategy)
        {
            Strategies = await _inputRegistry.GetAll();

            if (string.IsNullOrWhiteSpace(Input.Strategy))
            {
                Input.Strategy =
                    !string.IsNullOrWhiteSpace(selectedStrategy)
                    ? selectedStrategy
                    : Strategies.FirstOrDefault()?.Strategy ?? "";
            }

            SelectedStrategyDefinition =
                await _inputRegistry.GetByStrategy(Input.Strategy);

            if (SelectedStrategyDefinition == null)
            {
                return;
            }

            foreach (var parameter in SelectedStrategyDefinition.Parameters)
            {
                if (parameter.Type == StrategyParameterType.File ||
                    parameter.Type == StrategyParameterType.Files)
                {
                    continue;
                }

                if (!Input.Parameters.ContainsKey(parameter.Name))
                {
                    if (parameter.IsMultiple)
                    {
                        if (parameter.DefaultValue is IEnumerable<string> stringValues)
                        {
                            Input.Parameters[parameter.Name] = string.Join(",", stringValues);
                        }
                        else if (parameter.DefaultValue is IEnumerable<object> objectValues)
                        {
                            Input.Parameters[parameter.Name] = string.Join(",", objectValues);
                        }
                        else
                        {
                            Input.Parameters[parameter.Name] = parameter.DefaultValue?.ToString();
                        }
                    }
                    else
                    {
                        Input.Parameters[parameter.Name] = parameter.DefaultValue?.ToString();
                    }
                }

            }
        }


        private async Task<Dictionary<string, object>> BuildParametersAsync(
            StrategyInputDefinition definition,
            Dictionary<string, string?> rawValues)
        {
            var result = new Dictionary<string, object>();

            foreach (var parameter in definition.Parameters)
            {
                if (parameter.Type == StrategyParameterType.File ||
                    parameter.Type == StrategyParameterType.Files)
                {
                    var savedPaths = await SaveUploadedFilesAsync(parameter);

                    if (savedPaths.Count == 0)
                    {
                        if (parameter.IsRequired)
                        {
                            ModelState.AddModelError(
                                $"Files_{parameter.Name}",
                                $"{parameter.Label} الزامی است.");
                        }

                        continue;
                    }

                    if (parameter.Type == StrategyParameterType.File)
                    {
                        result[parameter.Name] = savedPaths[0];
                    }
                    else
                    {
                        result[parameter.Name] = savedPaths;
                    }

                    continue;
                }

                if (parameter.Type == StrategyParameterType.Select && parameter.IsMultiple)
                {
                    var inputName = $"Input.Parameters[{parameter.Name}]";

                    var selectedValues = Request.Form[inputName]
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x!)
                        .ToList();

                    if (selectedValues.Count == 0)
                    {
                        if (parameter.IsRequired)
                        {
                            ModelState.AddModelError(
                                inputName,
                                $"{parameter.Label} الزامی است.");
                        }

                        continue;
                    }

                    result[parameter.Name] = selectedValues;

                    continue;
                }

                rawValues.TryGetValue(parameter.Name, out var rawValue);

                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    if (parameter.IsRequired)
                    {
                        ModelState.AddModelError(
                            $"Input.Parameters[{parameter.Name}]",
                            $"{parameter.Label} الزامی است.");
                    }

                    continue;
                }

                result[parameter.Name] = ConvertParameterValue(parameter, rawValue);
            }

            return result;
        }

        private object ConvertParameterValue(
            StrategyParameterDefinition parameter,
            string rawValue)
        {
            return parameter.Type switch
            {
                StrategyParameterType.Number =>
                    decimal.Parse(rawValue),

                StrategyParameterType.Date =>
                    PersianDateHelper.ToGregorian(rawValue),

                StrategyParameterType.Boolean =>
                    rawValue == "true" || rawValue == "on" || rawValue == "1",

                StrategyParameterType.Timeframe =>
                    Enum.Parse<Timeframe>(rawValue, ignoreCase: true),

                _ => rawValue
            };
        }

        private async Task<List<string>> SaveUploadedFilesAsync(
            StrategyParameterDefinition parameter)
        {
            var result = new List<string>();

            if (!Request.HasFormContentType)
            {
                return result;
            }

            var inputName = $"Files_{parameter.Name}";

            var files = Request.Form.Files
                .Where(f => string.Equals(f.Name, inputName, StringComparison.OrdinalIgnoreCase))
                .Where(f => f.Length > 0)
                .ToList();

            if (files.Count == 0)
            {
                return result;
            }

            var root = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "StrategyUploads",
                Input.Strategy,
                DateTime.UtcNow.ToString("yyyyMMdd"));

            Directory.CreateDirectory(root);

            foreach (var file in files)
            {
                var safeOriginalName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(safeOriginalName);

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(root, fileName);

                await using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);

                result.Add(fullPath);
            }

            return result;
        }

        public class StrategyRunInputModel
        {
            public string Strategy { get; set; } = "";

            public Dictionary<string, string?> Parameters { get; set; } = new();
        }
    }

    public class StrategyParameterFormViewModel
    {
        public StrategyInputDefinition Definition { get; set; } = default!;

        public RunModel.StrategyRunInputModel Input { get; set; } = default!;
    }
}
