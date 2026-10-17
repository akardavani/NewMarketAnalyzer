using DataProvider;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MarketAnalyzer.Bourse.Domain;
using MarketAnalyzer.DataProvider;
using MarketAnalyzer.DataProvider.TseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", false)
    .Build();
services.TryAddSingleton<IConfiguration>(configuration);
services.AddDataProviders(configuration);
var provider = services.BuildServiceProvider();

//await SyncIndexAsync();
await SyncInstrumentsAsync();

async Task SyncIndexAsync()
{
    Console.WriteLine("Start fetch index info");
    var date = new DateTime(2000, 1, 1); //GET => 1402-09-20
    var indices = new List<TseIndexInfo>();
    var client = provider.GetRequiredService<TseClient>();
    var fail = 0;
    while (date <= DateTime.Today)
    {
        Console.WriteLine($"Fetch index info at {date:yyyy-MM-dd}");
        try
        {
            var data = await client.GetIndexInfoAsync(date);
            indices.AddRange(data);
            await Task.Delay(500);
            date = date.AddDays(1);
        }
        catch (Exception)
        {
            if (fail >= 3)
            {
                fail = 0;
                await Task.Delay(TimeSpan.FromSeconds(60));
                Console.WriteLine("Index fetch failed go sleep for 60 seconds");
            }
            else
            {
                fail++;
                Console.WriteLine("Index fetch failed.");
            }
        }
    }

    await WriteFileAsync("Indices", indices);
}

static async Task SyncInstrumentsAsync()
{
    Console.Write("Enter password: ");
    var password = Console.ReadLine();

    using var connection = new SqlConnection($"Data Source=192.168.20.14;Initial Catalog=PouyanDp;User ID=salemian;Password={password};Persist Security Info=True;Encrypt = False;Pooling=False;Max Pool Size=200;MultipleActiveResultSets=False;Application Name=Fund_Instruments;");
    await connection.OpenAsync();
    using var command = connection.CreateCommand();
    command.CommandText = @"
USE [PouyanDp];
SELECT [Id] = i.InstrumentId,
       [ExchangeId] = CASE i.MarketId
                          WHEN 1 THEN 2 --بورس
                          WHEN 2 THEN 1 --فرابورس
                          WHEN 3 THEN 3 --کالا
                          WHEN 4 THEN 2 --آتی
                          WHEN 5 THEN 4 --انرژی
                          WHEN 6 THEN 1 --پایه فرابورس
                          ELSE 0 END,
       [CompanyIsin] = i.ProductIsinCode,
       [Name] = i.InstrumentShortTextDescription,
       [NativeName] = CASE i.MarketSegment
                          WHEN 'NO' THEN i.InstrumentName + N'1'
                          ELSE i.InstrumentName END,
       [FullName] = mi.Name,
       [CompanyName] = i.IssuingCompanyName,
       [NormalizedNativeName] = i.InstrumentName,
       [Mnemonic] = i.InstrumentMnemonicCode,
       [InstrumentTypeCode] = i.InstrumentType,
       [MarketSegment] = i.MarketSegment,
       [InstrumentGroupId] = i.InstrumentGroup,
       [BoardCode] = i.BoardCode,
       [SectorCode] = i.SectorCode,
       [SubSectorCode] = i.SubSectorCode,
       [SettlementDelayType] = i.SettlementDelayType,
       [SharesOrBondsNumber] = i.ShareCapital,
       [MinimumEnterableQuantity] = i.OrderMinimumQuantity,
       [MaximumEnterableQuantity] = i.OrderMaximumQuantity,
       [ParValue] = ISNULL(i.ParValueAmount, 0),
       [FixedPriceTick] = i.FixedPriceTick,
       [LotSize] = i.InstrumentLotSize,
       [BaseVolume] = i.BaseVolume,
       [FirstTradingDate] = i.FirstTradeDate,
       [UnitPriceType] = i.InstrumentPriceUnitType,
       [HighIntermediateThreshold] = i.MaximumAuthorizedPriceThreshold,
       [LowIntermediateThreshold] = i.MinimumAuthorizedPriceThreshold,
       [EventType] = i.CorporateEventType,
       [EventDateTime] = i.ChangeDate,
       [IsActive] = i.IsValid,
       [InsCode] = ISNULL(ci.TsetmcInsCode, 0),
       [MarketId] = NULL
FROM pouyan.Instrument AS i
    INNER JOIN mabnadp.Instrument AS mi
        ON mi.Code = i.InstrumentId
    LEFT JOIN [convert].Instrument AS ci
        ON ci.InstrumentId = i.InstrumentId;
";
    using var reader = await command.ExecuteReaderAsync();
    var instruments = new List<InstrumentEntry>();
    while (await reader.ReadAsync())
    {
        var instrument = new InstrumentEntry
        {
            Id = reader.GetString(0),
            ExchangeId = reader.GetInt32(1),
            CompanyIsin = reader.GetString(2),
            Name = reader.GetString(3),
            NativeName = reader.GetString(4),
            FullName = reader.GetString(5),
            CompanyName = reader.GetString(6),
            NormalizedNativeName = reader.GetString(7),
            Mnemonic = reader.GetString(8),
            InstrumentTypeCode = reader.GetInt32(9),
            MarketSegment = reader.GetString(10),
            InstrumentGroupId = reader.GetString(11),
            BoardCode = reader.GetString(12),
            SectorCode = reader.GetString(13),
            SubSectorCode = reader.GetString(14),
            SettlementDelayType = reader.GetByte(15),
            SharesOrBondsNumber = reader.GetInt64(16),
            MinimumEnterableQuantity = reader.GetInt64(17),
            MaximumEnterableQuantity = reader.GetInt64(18),
            ParValue = reader.GetInt64(19),
            FixedPriceTick = reader.GetInt64(20),
            LotSize = reader.GetInt64(21),
            BaseVolume = reader.GetInt64(22),
            FirstTradingDate = reader.GetDateTime(23),
            UnitPriceType = reader.GetByte(24),
            HighIntermediateThreshold = reader.GetInt64(25),
            LowIntermediateThreshold = reader.GetInt64(26),
            EventType = reader.GetString(27),
            EventDateTime = reader.GetDateTime(28),
            IsActive = reader.GetBoolean(29),
            InsCode = reader.GetInt64(30)
        };

        instrument.CompanyCode = instrument.Id.Substring(4, 4);
        var instrumentType = InstrumentTypes.DetectInstrumentType(
            instrument.SubSectorCode,
            instrument.InstrumentTypeCode,
            instrument.MarketSegment);
        instrument.InstrumentTypeId = instrumentType.Id;

        instruments.Add(instrument);
    }

    //UpdateBaseInstruments(instruments);
    UpdateRightInstruments(instruments);

    //await WriteFileAsync("Instruments", instruments);
    await WriteSqlFileAsync(instruments);
}

static void UpdateRightInstruments(List<InstrumentEntry> instruments)
{
    //IRO1IPTR0001 ==> 
    //IRR1IPTR0101

    var query = from i in instruments
                from r in instruments.Where(r => r.Id == FormatRightInstrumentId(i))
                select new { BaseInstrument = i, RightInstrument = r };

    foreach (var instrument in query.Where(e => e.RightInstrument.Id != e.BaseInstrument.Id))
    {
        if (IsAbnormalRightInstrument(instrument.RightInstrument))
        {
            Console.WriteLine(instrument.BaseInstrument.Id);
            Console.WriteLine(instrument.RightInstrument.Id);
            Console.WriteLine(new string('=', 100));
        }

        instrument.RightInstrument.BaseInstrumentId = instrument.BaseInstrument.Id;
        instrument.BaseInstrument.RightInstrumentId = instrument.RightInstrument.Id;
    }
}

static void UpdateBaseInstruments(List<InstrumentEntry> instruments)
{
    var stocks = instruments
        .Where(e => e.InstrumentTypeId == InstrumentType.Future.Id ||
                    InstrumentTypes.Options.Any(b => b.Id == e.InstrumentTypeId ||
                    InstrumentTypes.Embeddeds.Any(b => b.Id == e.InstrumentTypeId)));

    foreach (var instrument in stocks)
    {
        var baseInstrument = instruments
            .FirstOrDefault(e => e.Id != instrument.Id &&
                                 e.MarketSegment == e.MarketSegment &&
                                 e.CompanyCode == instrument.CompanyCode);
        instrument.BaseInstrumentId = baseInstrument?.Id;
    }
}

static bool IsAbnormalRightInstrument(InstrumentEntry instrument)
{
    if (instrument.InstrumentTypeId != InstrumentType.Right.Id)
    {
        return true;
    }

    ReadOnlySpan<char> chars = instrument.NormalizedNativeName;
    if (chars[^1] == 'ح')
    {
        return false;
    }

    for (var i = chars.Length - 1; i >= 0; i--)
    {
        var ch = chars[i];
        if (char.IsDigit(ch) || ch == ' ')
        {
            continue;
        }
        return ch != 'ح';
    }

    return false;
}

static string FormatRightInstrumentId(InstrumentEntry instrument)
{
    //IRO1IPTR0001
    //IRR1IPTR0101
    var chars = new Span<char>(instrument.Id.ToCharArray());
    chars[2] = 'R';
    chars[9] = '1';
    return new string(chars);
}

static async Task WriteSqlFileAsync(List<InstrumentEntry> instruments)
{
    var baseCommand = "INSERT INTO [bourse].[Instruments] ([Id], [ExchangeId], [CompanyIsin], [Name], [NativeName], [FullName], [CompanyName], [NormalizedNativeName], [Mnemonic], [InstrumentTypeCode], [MarketSegment], [InstrumentGroupId], [BoardCode], [SectorCode], [SubSectorCode], [SettlementDelayType], [SharesOrBondsNumber], [MinimumEnterableQuantity], [MaximumEnterableQuantity], [ParValue], [FixedPriceTick], [LotSize], [BaseVolume], [FirstTradingDate], [UnitPriceType], [HighIntermediateThreshold], [LowIntermediateThreshold], [EventType], [EventDateTime], [IsActive], [InsCode], [RightInstrumentId], [HashCode], [InstrumentTypeId], [MarketId], [BaseInstrumentId], [Creator], [CreationTime], [LastModifier], [LastModificationTime]) VALUES ";
    var chunkNumber = 1;
    var sb = new StringBuilder();
    var commands = new List<string>((int)Math.Ceiling(instruments.Count / 100M));
    foreach (var chunk in instruments.Chunk(100))
    {
        Console.WriteLine($"Chunk: {chunkNumber++}");

        var index = 1;
        var values = new List<string>(100);
        foreach (var instrument in chunk)
        {
            Console.WriteLine($"Instrument: {index++}");
            sb.Clear();
            sb.Append('(');

            sb.Append($"N'{instrument.Id}', ");
            sb.Append($"{instrument.ExchangeId}, ");
            sb.Append($"N'{instrument.CompanyIsin}', ");
            sb.Append($"N'{instrument.Name.NormalizeArabicName()}', ");
            sb.Append($"N'{instrument.NativeName.NormalizeArabicName()}', ");
            sb.Append($"N'{instrument.FullName.NormalizeArabicName()}', ");
            sb.Append($"N'{instrument.CompanyName.NormalizeArabicName()}', ");
            sb.Append($"N'{instrument.NormalizedNativeName.NormalizeArabicName()}', ");
            sb.Append($"N'{instrument.Mnemonic}', ");
            sb.Append($"{instrument.InstrumentTypeCode}, ");
            sb.Append($"N'{instrument.MarketSegment}', ");
            sb.Append($"N'{instrument.InstrumentGroupId}', ");
            sb.Append($"N'{instrument.BoardCode}', ");
            sb.Append($"N'{instrument.SectorCode}', ");
            sb.Append($"N'{instrument.SubSectorCode}', ");
            sb.Append($"{instrument.SettlementDelayType}, ");
            sb.Append($"{instrument.SharesOrBondsNumber}, ");
            sb.Append($"{instrument.MinimumEnterableQuantity}, ");
            sb.Append($"{instrument.MaximumEnterableQuantity}, ");
            sb.Append($"{instrument.ParValue}, ");
            sb.Append($"{instrument.FixedPriceTick}, ");
            sb.Append($"{instrument.LotSize}, ");
            sb.Append($"{instrument.BaseVolume}, ");
            sb.Append($"CAST(N'{instrument.FirstTradingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}' AS DATE), ");
            sb.Append($"{instrument.UnitPriceType}, ");
            sb.Append($"{instrument.HighIntermediateThreshold}, ");
            sb.Append($"{instrument.LowIntermediateThreshold}, ");
            sb.Append($"N'{instrument.EventType}', ");
            sb.Append($"CAST(N'{instrument.EventDateTime.ToString("O", CultureInfo.InvariantCulture)}' AS DateTime2), ");
            sb.Append($"{(instrument.IsActive ? "1" : "0")}, ");
            sb.Append($"{instrument.InsCode}, ");
            sb.Append(string.IsNullOrEmpty(instrument.RightInstrumentId) ? "NULL, " : $"N'{instrument.RightInstrumentId}', ");
            sb.Append($"'{ComputeHashString(instrument)}', ");
            sb.Append($"{instrument.InstrumentTypeId}, ");
            sb.Append("NULL, ");
            sb.Append(string.IsNullOrEmpty(instrument.BaseInstrumentId) ? "NULL, " : $"N'{instrument.BaseInstrumentId}', ");
            sb.Append("0, ");
            sb.Append("SYSDATETIME(), ");
            sb.Append("0, ");
            sb.Append("SYSDATETIME() ");

            sb.Append(')');
            values.Add(sb.ToString());
        }

        commands.Add(baseCommand + "\r\n" + string.Join(",\r\n", values));
    }

    var query = "USE [db_FundProd]\r\nGO\r\n" + string.Join("\r\nGO\r\n", commands);
    using (var writer = new StreamWriter("Instruments.sql", false, Encoding.UTF8))
    {
        await writer.WriteAsync(query);
    }
    Console.WriteLine("Write data completed.");
}

static async Task WriteFileAsync<T>(string fileName, T data)
{
    Console.WriteLine($"Write data to file `{fileName}`");
    using var fs = new FileStream($"{fileName}.json", FileMode.Create, FileAccess.Write, FileShare.None);
    await JsonSerializer.SerializeAsync(fs, data, new JsonSerializerOptions()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    });
    Console.WriteLine("Write data completed.");
}

static string ComputeHashString<T>(T obj)
{
    var buffer = JsonSerializer.SerializeToUtf8Bytes(obj);
    var hash = SHA256.HashData(buffer);
    return Convert.ToHexString(hash);
}