﻿using System;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.GovWatch.Registry.Web {
    public static class SecurityHelper {
        private const string URI_FORMAT = "otpauth://{0}/{1}?secret={2}&issuer={3}&digits={4}&period={5}";

        public static string GenerateOtpUri(string issuer, string user, string secret,
                                            int digits = 6,
                                            int period = 30,
                                            string otpType = "totp") {

            return string.Format(URI_FORMAT,
                otpType,                                    // 0
                WebUtility.UrlEncode(issuer + ":" + user),  // 1
                secret,                                     // 2
                WebUtility.UrlEncode(issuer),               // 3
                digits,                                     // 4
                period);                                    // 5
        }

        public static string FormatSecret(string secret, string separator = "-", Func<string, string> formatter = null) {
            if (secret == null) throw new ArgumentNullException(nameof(secret));
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentException("Value cannot be null or empty string.", nameof(separator));
            if (formatter == null) formatter = s => s;

            var result = new StringBuilder();

            var currentPosition = 0;
            while (currentPosition + 4 < secret.Length) {
                result.Append(secret.Substring(currentPosition, 4)).Append(separator);
                currentPosition += 4;
            }
            if (currentPosition < secret.Length) result.Append(secret.Substring(currentPosition));

            return formatter(result.ToString());
        }

        public static string GenerateRandomPassword(int length = 20) {
            if (length < 10) throw new ArgumentOutOfRangeException(nameof(length));

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create()) {
                var passwordData = new byte[length * 6 / 8];
                rng.GetBytes(passwordData);
                return Convert.ToBase64String(passwordData).Replace('+', '-').Replace('/', '_');
            }
        }

        public static bool IsIdentitySuccess(this PageModel page, IdentityResult result) {
            if (!result.Succeeded) {
                foreach (var err in result.Errors) {
                    page.ModelState.AddModelError(string.Empty, $"{err.Description} [{err.Code}]");
                }
            }
            return result.Succeeded;
        }

    }
}
