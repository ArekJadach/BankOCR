using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOCR
{
    public static class Decoder
    {
        public static List<string> DecodeFile(List<string> lines)
        {
            var results = new List<string>();

            for (int i = 0; i < lines.Count / 4; i += 4)
            {
                var accountNumber = Account.GetDecodedNumberWithPossibleVariations("\r\n" + lines[i] + "\r\n" + lines[i + 1] + "\r\n" + lines[i + 1]);

                results.Add(accountNumber);
            }

            return results;
        }

        public static string ParseToAccountNumber(string input)
        {
            var lines = input.Split("\r\n", StringSplitOptions.None);
            var accountNumber = DecodeAccountNumber(lines[1], lines[2], lines[3]);

            return accountNumber;
        }

        public static string DecodeAccountNumber(string top, string middle, string bottom)
        {
            var decodedAccountNumber = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                decodedAccountNumber += DecodeDigit(
                    top.Substring(i * 3, 3),
                    middle.Substring(i * 3, 3),
                    bottom.Substring(i * 3, 3)
                    );
            }

            return decodedAccountNumber;
        }

        public static string DecodeDigit(string top, string middle, string bottom)
        {
            var commonDigit = FindCommonDigit(GetTopLineDigitsMatches(top), GetMiddleLineDigitsMatches(middle), GetBottomLineDigitsMatches(bottom));

            if (commonDigit.HasValue)
            {
                return commonDigit.Value.ToString();
            }

            return "?";
        }

        public static List<int> GetTopLineDigitsMatches(string chars)
        {
            if (chars == "   ")
            {
                return new List<int>() { 1, 4 };
            }

            if (chars == " _ ")
            {
                return new List<int>() { 0, 2, 3, 5, 6, 7, 8, 9 };
            }

            return new List<int>();
        }

        public static List<int> GetMiddleLineDigitsMatches(string chars)
        {
            if (chars == "| |")
            {
                return new List<int>() { 0 };
            }

            if (chars == "  |")
            {
                return new List<int>() { 1, 7 };
            }

            if (chars == " _|")
            {
                return new List<int>() { 2, 3 };
            }

            if (chars == "|_|")
            {
                return new List<int>() { 4, 8, 9 };
            }

            if (chars == "|_ ")
            {
                return new List<int>() { 5, 6 };
            }

            return new List<int>();
        }

        public static List<int> GetBottomLineDigitsMatches(string chars)
        {
            if (chars == "|_|")
            {
                return new List<int>() { 0, 6, 8 };
            }

            if (chars == "  |")
            {
                return new List<int>() { 1, 4, 7 };
            }

            if (chars == "|_ ")
            {
                return new List<int>() { 2 };
            }

            if (chars == " _|")
            {
                return new List<int>() { 3, 5, 9 };
            }

            return new List<int>();
        }

        public static int? FindCommonDigit(List<int> topLineDigitsMatches, List<int> middleLineDigitsMatches, List<int> bottomLineDigitsMatches)
        {
            if (topLineDigitsMatches.Count == 0 ||
                middleLineDigitsMatches.Count == 0 ||
                bottomLineDigitsMatches.Count == 0)
            {
                return null;
            }

            var commonDigits = topLineDigitsMatches.Intersect(middleLineDigitsMatches);

            if (!commonDigits.Any())
            {
                return null;
            }

            commonDigits = commonDigits.Intersect(bottomLineDigitsMatches);

            if (commonDigits.Count() != 1)
            {
                return null;
            }

            return commonDigits.First();
        }
    }
}
