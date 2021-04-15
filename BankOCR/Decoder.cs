using BankOCR.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOCR
{
    public static class Decoder
    {
        public static List<string> DecodeFile(List<string> lines)
        {
            if (lines.Count % 4 != 0)
            {
                Console.WriteLine("Number of lines in file is incorrect! Each number has 4 lines");
            }

            if (lines.Any(x => x.Length != 27))
            {
                Console.WriteLine($"Each line should contain 27 characters.");
            }

            var results = new List<string>();

            for (int i = 0; i < lines.Count; i += 4)
            {
                var accountNumber = new AccountNumberModel
                {
                    TopLine = lines[i],
                    MiddleLine = lines[i + 1],
                    BottomLine = lines[i + 2]
                };

                var accountNumberResult = Account.GetDecodedNumberWithPossibleVariations(
                    $"\r\n{ accountNumber.TopLine }\r\n{accountNumber.MiddleLine}\r\n{accountNumber.BottomLine}");

                results.Add(accountNumberResult);
            }

            return results;
        }

        public static string ParseToAccountNumber(string input)
        {
            var lines = input.Split("\r\n", StringSplitOptions.None);

            var accountNumber = new AccountNumberModel
            {
                TopLine = lines[1],
                MiddleLine = lines[2],
                BottomLine = lines[3]
            };

            return DecodeAccountNumber(accountNumber);
        }

        public static string DecodeAccountNumber(AccountNumberModel accountNumberModel)
        {
            var decodedAccountNumber = string.Empty;
            for (int i = 0; i < 9; i++)
            {
                decodedAccountNumber += DecodeDigit(
                    accountNumberModel.TopLine.Substring(i * 3, 3),
                    accountNumberModel.MiddleLine.Substring(i * 3, 3),
                    accountNumberModel.BottomLine.Substring(i * 3, 3)
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
