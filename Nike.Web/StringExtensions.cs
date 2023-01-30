using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Nike.Web
{
    public static class StringExtensions
    {
        public enum MsisdnFormat
        {
            WithCountryCode = 1,
            With0 = 2,
            With00 = 3,
            WithPlus = 4,
            WithoutPrefix = 5
        }

        // Blah Blah Blah
        /// <summary>
        ///     Abp: Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return invariantCulture ? str.ToLowerInvariant() : str.ToLower();

            return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        }

        public static T ToEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        public static string GetValueFromTag(this string content, string tagName)
        {
            var target = $"<{tagName}>(.*)</{tagName}>";
            var value = Regex.Match(content, target);
            return value.Groups[1].Value;
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        ///     Check if input text is a valid Iranian cell number.
        /// </summary>
        /// <param name="text">input cell number.</param>
        /// <returns></returns>
        public static bool IsValidPhoneNumber(this string text)
        {
            return Regex.IsMatch(text, @"^(98|0)\d{10}$");
        }

        /// <summary>
        ///     Check if input text is a valid email address.
        /// </summary>
        /// <param name="email">input email address.</param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }


        public static string UppercaseFirst(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        public static XElement GetElementsByName(this string xml, string tagName)
        {
            var elements = XElement.Parse(xml);
            var result = elements.Descendants().FirstOrDefault(x => x.Name.LocalName == tagName);
            return result;
        }

        public static string ApplyUnifiedYeke(this string data)
        {
            return string.IsNullOrEmpty(data)
                ? data
                : data.Replace("ي", "ی")
                    .Replace("ك", "ک");
        }

        public static string ToEnglishNumber(this string data)
        {
            return string.IsNullOrEmpty(data)
                ? data
                : data.Replace("۱", "1")
                    .Replace("۲", "2")
                    .Replace("۳", "3")
                    .Replace("۴", "4")
                    .Replace("۵", "5")
                    .Replace("۶", "6")
                    .Replace("۷", "7")
                    .Replace("۸", "8")
                    .Replace("۹", "9")
                    .Replace("٠", "0")
                    .Replace("١", "1")
                    .Replace("٢", "2")
                    .Replace("٣", "3")
                    .Replace("٤", "4")
                    .Replace("٥", "5")
                    .Replace("٦", "6")
                    .Replace("٧", "7")
                    .Replace("٨", "8")
                    .Replace("٩", "9");
        }

        public static string AddQueryString(this string url, string key, string value)
        {
            url = url.Trim().TrimEnd('/');
            var query = $"{key}={value}";
            return Regex.IsMatch(url, @"\w=\w") ? $"{url}&{query}" : $"{url}?{query}";
        }

        public static string FetchNumberFromString(this string input)
        {
            return Regex.Match(input, @"\d+").Value;
        }

        public static string ToNormalizeMsisdn(this string msisdn, MsisdnFormat format = MsisdnFormat.WithoutPrefix,
            int msisdnLength = 10,
            string countryCode = "98")
        {
            msisdn = msisdn.ToEnglishNumber()
                .Substring(msisdn.Length - msisdnLength);
            switch (format)
            {
                case MsisdnFormat.WithoutPrefix:
                    return msisdn;
                case MsisdnFormat.WithCountryCode:
                    return $"{countryCode}{msisdn}";
                case MsisdnFormat.With0:
                    return $"0{msisdn}";
                case MsisdnFormat.With00:
                    return $"00{msisdn}";
                case MsisdnFormat.WithPlus:
                    return $"+{countryCode}{msisdn}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}