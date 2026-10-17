# اجرا از ریشه backend:  pwsh ./Relocate-MarketIntelligence.ps1
$ErrorActionPreference = 'Stop'
$root = Get-Location
$old  = Join-Path $root 'Modules/MarketAnalysis/src'
$new  = Join-Path $root 'MarketIntelligence/src'
New-Item -ItemType Directory -Force $new | Out-Null

# 1) نگاشت پروژه‌ها (قدیم => جدید)
$map = [ordered]@{
  'MarketAnalyzer.Domain'                  = 'MarketIntelligence.Domain'
  'MarketAnalyzer.Application'             = 'MarketIntelligence.Application'
  'MarketAnalyzer.AnalysisStrategy'        = 'MarketIntelligence.Application'   # ادغام
  'MarketAnalyzer.Analysis.Infrastructure' = 'MarketIntelligence.Infrastructure'
  'MarketAnalyzer.Collectors'              = 'MarketIntelligence.Collectors'
}
$oldDirs = Get-ChildItem $old -Directory | ForEach-Object Name
foreach ($k in $map.Keys) {
  $src = Get-ChildItem $old -Directory | Where-Object { $_.Name -eq $k -or $_.Name -like "*$($k.Split('.')[-1])" } | Select-Object -First 1
  if (-not $src) { Write-Warning "پیدا نشد: $k"; continue }
  $dst = Join-Path $new $map[$k]
  New-Item -ItemType Directory -Force $dst | Out-Null
  Get-ChildItem $src.FullName -Force | Where-Object { $_.Name -notin 'bin','obj' -and $_.Extension -ne '.csproj' } |
    ForEach-Object { Copy-Item $_.FullName $dst -Recurse -Force }
  $csproj = Get-ChildItem $src.FullName -Filter *.csproj | Select-Object -First 1
  $target = Join-Path $dst "$($map[$k]).csproj"
  if ($csproj -and -not (Test-Path $target)) { Copy-Item $csproj.FullName $target }
}

# 2) Persistence: انتقال Context از Infrastructure
$infra = Join-Path $new 'MarketIntelligence.Infrastructure'
$pers  = Join-Path $new 'MarketIntelligence.Persistence'
New-Item -ItemType Directory -Force $pers | Out-Null
if (Test-Path "$infra/Context") { Move-Item "$infra/Context" "$pers/Context" -Force }
if (Test-Path "$infra/Migrations") { Remove-Item "$infra/Migrations" -Recurse -Force }   # Sqlite؛ بعداً برای SqlServer بازتولید
if (-not (Test-Path "$pers/MarketIntelligence.Persistence.csproj")) {
@'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net8.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>disable</Nullable></PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.10" />
    <PackageReference Include="EFCore.BulkExtensions.SqlServer" Version="8.1.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\MarketIntelligence.Domain\MarketIntelligence.Domain.csproj" />
  </ItemGroup>
  <ItemGroup><None Update="Context\Seed\**\*.json" CopyToOutputDirectory="PreserveNewest" /></ItemGroup>
</Project>
'@ | Set-Content "$pers/MarketIntelligence.Persistence.csproj" -Encoding utf8
}

# 3) حذف کد مرده (تأییدشده)
$dead = @(
  "$pers/Context/MarketDbContextFactory.cs",
  "$new/MarketIntelligence.Domain/Telegram",
  "$new/MarketIntelligence.Collectors/Program.cs",
  "$new/MarketIntelligence.Collectors/Worker.cs",
  "$new/MarketIntelligence.Collectors/CronJobService.cs"
)
$dead += Get-ChildItem $new -Recurse -Include SettingsLoader.cs,JsonConvertor.cs | ForEach-Object FullName
foreach ($d in $dead) { if (Test-Path $d) { Remove-Item $d -Recurse -Force; Write-Host "حذف: $d" } }

# 4) بازنویسی namespace و ارجاعات
$ns = [ordered]@{
  'MarketAnalyzer.Analysis.Infrastructure' = 'MarketIntelligence.Infrastructure'
  'MarketAnalyzer.AnalysisStrategy'        = 'MarketIntelligence.Application'
  'MarketAnalyzer.Collectors'              = 'MarketIntelligence.Collectors'
  'MarketAnalyzer.Domain'                  = 'MarketIntelligence.Domain'
  'MarketAnalyzer.Application'             = 'MarketIntelligence.Application'
}
$files = Get-ChildItem $new -Recurse -Include *.cs,*.csproj | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
foreach ($f in $files) {
  $t = Get-Content $f.FullName -Raw
  $o = $t
  foreach ($k in $ns.Keys) { $t = $t.Replace($k, $ns[$k]) }
  $t = $t -replace 'MarketIntelligence\.Infrastructure\.Context', 'MarketIntelligence.Persistence.Context'
  $t = $t -replace 'Include="Microsoft\.EntityFrameworkCore\.Sqlite"[^/]*/>', ''
  $t = $t -replace '(Include="EFCore\.BulkExtensions)"\s+Version="[^"]+"', '$1.SqlServer" Version="8.1.3"'
  if ($t -ne $o) { Set-Content $f.FullName $t -Encoding utf8 -NoNewline }
}
Write-Warning "ارجاعات ProjectReference داخل csprojها را بررسی کنید (مسیرها ..\MarketIntelligence.*)."

# 5) Solution
$sln = Join-Path $root 'MarketAnalyzer.sln'
dotnet sln $sln list | Select-String 'Modules[\\/]MarketAnalysis' | ForEach-Object { dotnet sln $sln remove $_.Line.Trim() }
Get-ChildItem $new -Recurse -Filter *.csproj | ForEach-Object { dotnet sln $sln add $_.FullName --solution-folder MarketIntelligence }

# 6) Api
$api = Join-Path $root 'MarketAnalyzer.Api'
$apiProj = "$api/MarketAnalyzer.Api.csproj"
$p = Get-Content $apiProj -Raw
$p = $p -replace '\s*<ProjectReference Include="\.\.\\Modules\\MarketAnalysis[^"]*" />', ''
Set-Content $apiProj $p -Encoding utf8 -NoNewline
foreach ($n in 'Application','Persistence','Collectors') {
  dotnet add $apiProj reference "$new/MarketIntelligence.$n/MarketIntelligence.$n.csproj"
}
Get-ChildItem $api -Recurse -Include *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
  $t = Get-Content $_.FullName -Raw; $o = $t
  foreach ($k in $ns.Keys) { $t = $t.Replace($k, $ns[$k]) }
  if ($t -ne $o) { Set-Content $_.FullName $t -Encoding utf8 -NoNewline }
}
Write-Host "Program.cs: اضافه کنید → builder.Services.AddMarketCollectors(); و ثبت MarketDbContext با UseSqlServer(\"Defualt\")" -ForegroundColor Yellow

# 7) حذف پوشه قدیمی (UI.Web و CollectorRunner هم داخل همین است)
Remove-Item (Join-Path $root 'Modules') -Recurse -Force
Write-Host "تمام. حالا dotnet build بزنید." -ForegroundColor Green
