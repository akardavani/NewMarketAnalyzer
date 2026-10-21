using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Tools.Domain;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Tools.Application
{
    public class VaultStatus
    {
        public bool Initialized { get; init; }
        public string? Salt { get; init; }
        public int Iterations { get; init; }
        public string? VerifierCipher { get; init; }
        public string? VerifierIv { get; init; }
    }
    public class PasswordEntryItem
    {
        public int Id { get; init; }
        public string Cipher { get; init; } = "";
        public string Iv { get; init; } = "";
        public DateTime Modified { get; init; }
    }

    public class VaultStatusQuery : IRequest<VaultStatus> { public required int UserId { get; init; } }
    internal class VaultStatusQueryHandler : IRequestHandler<VaultStatusQuery, VaultStatus>
    {
        private readonly IDbContext _db;
        public VaultStatusQueryHandler(IDbContext db) => _db = db;
        public async Task<VaultStatus> HandleAsync(VaultStatusQuery r, CancellationToken ct = default)
        {
            var v = await _db.Set<PasswordVault>().AsNoTracking().FirstOrDefaultAsync(x => x.UserId == r.UserId, ct);
            if (v is null) return new VaultStatus { Initialized = false };
            return new VaultStatus { Initialized = true, Salt = v.KdfSalt, Iterations = v.KdfIterations, VerifierCipher = v.VerifierCipher, VerifierIv = v.VerifierIv };
        }
    }

    public class PasswordEntriesQuery : IRequest<List<PasswordEntryItem>> { public required int UserId { get; init; } }
    internal class PasswordEntriesQueryHandler : IRequestHandler<PasswordEntriesQuery, List<PasswordEntryItem>>
    {
        private readonly IDbContext _db;
        public PasswordEntriesQueryHandler(IDbContext db) => _db = db;
        public async Task<List<PasswordEntryItem>> HandleAsync(PasswordEntriesQuery r, CancellationToken ct = default)
            => await _db.Set<PasswordEntry>().AsNoTracking().Where(e => e.UserId == r.UserId).OrderBy(e => e.Id)
                .Select(e => new PasswordEntryItem { Id = e.Id, Cipher = e.Cipher, Iv = e.Iv, Modified = e.LastModificationTime }).ToListAsync(ct);
    }

    /// <summary>ساخت کاسه (فقط یک بار). کلیدها از سمت کلاینت تولید می‌شوند.</summary>
    public class InitVaultCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public string Salt { get; init; } = "";
        public int Iterations { get; init; }
        public string VerifierCipher { get; init; } = "";
        public string VerifierIv { get; init; } = "";
    }
    internal class InitVaultCommandHandler : IRequestHandler<InitVaultCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public InitVaultCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(InitVaultCommand r, CancellationToken ct = default)
        {
            if (r.Iterations < 100_000 || r.Iterations > 2_000_000) return ApiResult.FromError<int>("تعداد iteration نامعتبر است.");
            if (string.IsNullOrWhiteSpace(r.Salt) || string.IsNullOrWhiteSpace(r.VerifierCipher) || string.IsNullOrWhiteSpace(r.VerifierIv))
                return ApiResult.FromError<int>("داده‌ی کلید نامعتبر است.");
            if (await _db.Set<PasswordVault>().AnyAsync(x => x.UserId == r.UserId, ct))
                return ApiResult.FromError<int>("کاسه قبلاً ساخته شده است.");
            var v = PasswordVault.Create(r.UserId);
            v.SetKeys(r.Salt, r.Iterations, r.VerifierCipher, r.VerifierIv);
            _db.Set<PasswordVault>().Add(v);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(v.Id);
        }
    }

    /// <summary>ایجاد یا به‌روزرسانی یک ورودی (Id = 0 یعنی جدید).</summary>
    public class SavePasswordEntryCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public int Id { get; init; }
        public string Cipher { get; init; } = "";
        public string Iv { get; init; } = "";
    }
    internal class SavePasswordEntryCommandHandler : IRequestHandler<SavePasswordEntryCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SavePasswordEntryCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(SavePasswordEntryCommand r, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(r.Cipher) || r.Cipher.Length > 32000 || string.IsNullOrWhiteSpace(r.Iv) || r.Iv.Length > 100)
                return ApiResult.FromError<int>("داده‌ی رمزنگاری‌شده نامعتبر است.");
            if (!await _db.Set<PasswordVault>().AnyAsync(x => x.UserId == r.UserId, ct))
                return ApiResult.FromError<int>("کاسه ساخته نشده است.");
            PasswordEntry? e;
            if (r.Id == 0) { e = PasswordEntry.Create(r.UserId); _db.Set<PasswordEntry>().Add(e); }
            else
            {
                e = await _db.Set<PasswordEntry>().FirstOrDefaultAsync(x => x.Id == r.Id && x.UserId == r.UserId, ct);
                if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            }
            e.Update(r.Cipher, r.Iv);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(e.Id);
        }
    }

    public class RemovePasswordEntryCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public int Id { get; init; } }
    internal class RemovePasswordEntryCommandHandler : IRequestHandler<RemovePasswordEntryCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemovePasswordEntryCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemovePasswordEntryCommand r, CancellationToken ct = default)
        {
            var e = await _db.Set<PasswordEntry>().FirstOrDefaultAsync(x => x.Id == r.Id && x.UserId == r.UserId, ct);
            if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            _db.Set<PasswordEntry>().Remove(e); await _db.SaveChangesAsync(ct);
            return ApiResult.From(r.Id);
        }
    }

    public class PasswordRekeyItem { public int Id { get; init; } public string Cipher { get; init; } = ""; public string Iv { get; init; } = ""; }

    /// <summary>تغییر master password: همهٔ ورودی‌ها با کلید جدید دوباره رمز می‌شوند، در یک تراکنش.</summary>
    public class RekeyVaultCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public string Salt { get; init; } = "";
        public int Iterations { get; init; }
        public string VerifierCipher { get; init; } = "";
        public string VerifierIv { get; init; } = "";
        public List<PasswordRekeyItem> Entries { get; init; } = new();
    }
    internal class RekeyVaultCommandHandler : IRequestHandler<RekeyVaultCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RekeyVaultCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RekeyVaultCommand r, CancellationToken ct = default)
        {
            if (r.Iterations < 100_000 || r.Iterations > 2_000_000) return ApiResult.FromError<int>("تعداد iteration نامعتبر است.");
            var v = await _db.Set<PasswordVault>().FirstOrDefaultAsync(x => x.UserId == r.UserId, ct);
            if (v is null) return ApiResult.FromError<int>("کاسه ساخته نشده است.");
            var existing = await _db.Set<PasswordEntry>().Where(e => e.UserId == r.UserId).ToListAsync(ct);
            // جلوگیری از از دست رفتن داده: باید دقیقاً همهٔ ورودی‌های موجود فرستاده شوند
            var ids = existing.Select(e => e.Id).ToHashSet();
            if (r.Entries.Count != ids.Count || r.Entries.Any(x => !ids.Contains(x.Id)))
                return ApiResult.FromError<int>("ورودی‌ها با داده‌ی سرور همخوانی ندارند. صفحه را تازه کنید.");
            foreach (var x in r.Entries)
            {
                if (string.IsNullOrWhiteSpace(x.Cipher) || x.Cipher.Length > 32000 || string.IsNullOrWhiteSpace(x.Iv)) return ApiResult.FromError<int>("داده‌ی نامعتبر.");
            }
            var map = existing.ToDictionary(e => e.Id);
            foreach (var x in r.Entries) map[x.Id].Update(x.Cipher, x.Iv);
            v.SetKeys(r.Salt, r.Iterations, r.VerifierCipher, r.VerifierIv);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(r.Entries.Count);
        }
    }
}
