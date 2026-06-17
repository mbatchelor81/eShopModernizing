using System;
using System.Text.RegularExpressions;

namespace eShopModernizedMVC.Services
{
    public static class SearchInputValidator
    {
        public const int MaxSearchTermLength = 200;
        private static readonly Regex SqlMetaCharacters = new Regex(@"[;'\""\-\-\*/\\]", RegexOptions.Compiled);

        public static string Sanitize(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return string.Empty;
            }

            searchTerm = searchTerm.Trim();

            if (searchTerm.Length > MaxSearchTermLength)
            {
                searchTerm = searchTerm.Substring(0, MaxSearchTermLength);
            }

            searchTerm = SqlMetaCharacters.Replace(searchTerm, string.Empty);

            return searchTerm;
        }

        public static bool IsValid(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return false;
            }

            if (searchTerm.Length > MaxSearchTermLength)
            {
                return false;
            }

            if (SqlMetaCharacters.IsMatch(searchTerm))
            {
                return false;
            }

            return true;
        }
    }
}
