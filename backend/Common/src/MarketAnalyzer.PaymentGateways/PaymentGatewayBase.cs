using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.PaymentGateways
{
    internal abstract class PaymentGatewayBase : IPaymentGateway
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            //TODO: Config
        };

        protected PaymentGatewayBase(string uniqueName)
        {
            if (string.IsNullOrEmpty(uniqueName))
            {
                throw new ArgumentException($"'{nameof(uniqueName)}' cannot be null or empty.", nameof(uniqueName));
            }

            UniqueName = uniqueName;
        }

        public string UniqueName { get; }
        public virtual byte[] EncryptionIV { get; }
        public virtual byte[] EncryptionKey { get; }
        protected abstract Dictionary<int, string> StatusDescriptions { get; }

        public virtual string GetCodeDescription(int code)
        {
            if (StatusDescriptions.TryGetValue(code, out string desc))
            {
                return desc;
            }

            return $"خطای نامشخصی با کد {code} رخ داده است.";
        }

        public abstract Task<PaymentRequestResult> RequestAsync(OnlinePaymentRequest request, CancellationToken cancellation = default);

        public async Task<IActionResult> ProcessRequestAsync(ProcessRequestOptions request, CancellationToken cancellation = default)
        {
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "text/html; charset=utf-8",
                Content = await GenerateHtmlContentAsync(request, cancellation)
            };
        }

        public async Task<string> GenerateHtmlContentAsync(ProcessRequestOptions request, CancellationToken cancellation = default)
        {
            var assembly = GetType().Assembly;
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Html.{UniqueName}_PaymentForm.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync(cancellation);
            return FormatResponseContent(html, request);
        }

        public abstract Task<TransactionResult> ProcessTransactionAsync(ProcessTransactionRequest request, CancellationToken cancellation = default);

        public abstract Task<VerifyTransactionResult> VerifyTransactionAsync(VerifyTransactionRequest request, CancellationToken cancellation = default);

        protected static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

        protected abstract string FormatResponseContent(string content, ProcessRequestOptions request);

        protected virtual string EncryptNationalCode(string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
            {
                return null;
            }

            var textToEncryption = $"{GetPersonalityType(nationalCode)}|{nationalCode}|{new Random().Next(1000000, 9999999)}|";
            return Encrypt(textToEncryption);
        }

        protected virtual string Encrypt(string plain)
        {
            if (string.IsNullOrEmpty(plain))
            {
                return null;
            }

            using var aes = Aes.Create();
            using var encryptor = aes.CreateEncryptor(EncryptionKey, EncryptionIV);
            using var stream = new MemoryStream();
            using var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cryptoStream);
            sw.Write(plain);
            return Convert.ToBase64String(stream.ToArray());
        }

        private static string GetPersonalityType(string nationalCode)
        {
            if (nationalCode.Length == 11)
            {
                return "1";
            }
            else if (nationalCode.Length >= 12)
            {
                return "2";
            }
            else
            {
                return "0";
            }
        }
    }
}