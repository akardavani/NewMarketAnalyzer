$src = "MarketIntelligence\src"
$from = "$src\MarketIntelligence.Persistence\Context\Seed"
$to   = "$src\MarketIntelligence.Infrastructure\Context\Seed"

New-Item -ItemType Directory -Force $to | Out-Null
Get-ChildItem $from -File | ForEach-Object { git mv $_.FullName $to }
# اگر از git استفاده نمی‌کنید، به جای خط بالا:
# Move-Item "$from\*" $to

# حذف خط کپی JSON از Persistence.csproj
$p = "$src\MarketIntelligence.Persistence\MarketIntelligence.Persistence.csproj"
(Get-Content $p) | Where-Object { $_ -notmatch 'Context\\Seed' } | Set-Content $p
