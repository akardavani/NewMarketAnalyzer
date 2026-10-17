using MarketAnalyzer.Abstractions;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MarketAnalyzer.Infrastructure.Authentication
{
    internal class OtpAuthenticationService : IOtpAuthenticationService
    {
        private static readonly Encoding _encoding = new UTF8Encoding(false, true);

        public OtpAuthenticationService()
        {

        }

        public static byte[] GenerateRandomKey()
        {
            var bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return bytes;
        }

        public int GenerateCode(TimeSpan lifetime, byte[] securityToken, string userId)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            // Allow a variance of no greater than 9 minutes in either direction
            var currentTimeStep = GetCurrentTimeStepNumber(lifetime);
            using var hashAlgorithm = new HMACSHA1(securityToken);
            return ComputeTotp(hashAlgorithm, currentTimeStep, userId);
        }

        public bool ValidateCode(TimeSpan lifetime, byte[] securityToken, int code, string userId)
        {
            if (securityToken == null)
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            // Allow a variance of no greater than 9 minutes in either direction
            var currentTimeStep = GetCurrentTimeStepNumber(lifetime);
            using var hashAlgorithm = new HMACSHA1(securityToken);
            for (var i = -2; i <= 2; i++)
            {
                var computedTotp = ComputeTotp(hashAlgorithm, (ulong)((long)currentTimeStep + i), userId);
                if (computedTotp == code)
                {
                    return true;
                }
            }

            // No match
            return false;
        }

        private static ulong GetCurrentTimeStepNumber(TimeSpan lifetime)
        {
            var delta = DateTimeOffset.UtcNow - DateTimeOffset.UnixEpoch;
            return (ulong)(delta.Ticks / lifetime.Ticks);
        }

        private static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier)
        {
            // # of 0's = length of pin
            const int Mod = 1_000_000;

            // See https://tools.ietf.org/html/rfc4226
            // We can add an optional modifier
            var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(timestepAsBytes, modifier));

            // Generate DT string
            var offset = hash[^1] & 0xf;
            Debug.Assert(offset + 4 < hash.Length);
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | hash[offset + 3] & 0xff;

            return binaryCode % Mod;
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (string.IsNullOrEmpty(modifier))
            {
                return input;
            }

            var modifierBytes = _encoding.GetBytes(modifier);
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            Buffer.BlockCopy(input, 0, combined, 0, input.Length);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }
    }
}
