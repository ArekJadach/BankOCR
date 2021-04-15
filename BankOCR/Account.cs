using BankOCR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankOCR
{
    public static class Account
    {
        public const string ILLEGIBLE = "ILL";
        public const string ERROR = "ERR";

        public static AccountNumberWithPostscript GetDecodedAccountNumberWithPostscript(string input)
        {
            var number = Decoder.ParseToAccountNumber(input);

            var accNumberWithPostscript = new AccountNumberWithPostscript
            {
                AccountNumber = number
            };

            return accNumberWithPostscript.GetPostscript();
        }

        public static string GetDecodedAccountNumberWithPostscriptAsString(string input)
        {
            var accountNumberWithPostscript = GetDecodedAccountNumberWithPostscript(input);

            if (accountNumberWithPostscript.Postscript == null)
            {
                return accountNumberWithPostscript.AccountNumber;
            }

            return accountNumberWithPostscript.AccountNumber + " " + accountNumberWithPostscript.Postscript;
        }

        public static string GetDecodedNumberWithPossibleVariations(string input)
        {
            var numberWithComment = GetDecodedAccountNumberWithPostscript(input);

            var allValidVariations = GetPossibleVariationsForAccountNumber(numberWithComment);

            if (!allValidVariations.Any())
            {
                return (numberWithComment.Postscript == null ? 
                    numberWithComment.AccountNumber : numberWithComment.AccountNumber + " " + numberWithComment.Postscript);
            }

            if (allValidVariations.Count() == 1)
            {
                return allValidVariations.First().ToString();
            }

            var variations = string.Join(", ", allValidVariations.Select(acc => $"'{acc}'"));

            return $"{numberWithComment.AccountNumber} AMB [{variations}]";
        }

        public static IEnumerable<string> GetPossibleVariationsForAccountNumber(AccountNumberWithPostscript accountNumberWithPostscript)
        {
            var correctAccountVariations = new List<string>();

            if (accountNumberWithPostscript.Postscript == ERROR)
            {
                correctAccountVariations.AddRange(GetNumberVariations(accountNumberWithPostscript.AccountNumber).OrderBy(i => i));
            }

            if (accountNumberWithPostscript.Postscript == ILLEGIBLE)
            {
                correctAccountVariations.AddRange(GetNumberVariationsForIllegibleAccount(accountNumberWithPostscript.AccountNumber));
            }

            if (accountNumberWithPostscript.Postscript is null)
            {
                correctAccountVariations.AddRange(GetNumberVariations(accountNumberWithPostscript.AccountNumber).OrderBy(i => i));
            }

            return correctAccountVariations;
        }

        public static List<string> GetNumberVariations(string accountNumber)
        {
            var correctAccountVariations = new List<string>();

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    var numberSimilarities = Digit.DigitsSimilarities[(int)char.GetNumericValue(accountNumber[i])];

                    if (numberSimilarities.Length > 0 && numberSimilarities.Length > j)
                    {
                        StringBuilder sb = new StringBuilder(accountNumber);
                        sb[i] = char.Parse(numberSimilarities[j].ToString());

                        if (sb.ToString().IsValid())
                        {
                            correctAccountVariations.Add(sb.ToString());
                        }
                    }
                }
            }

            return correctAccountVariations;
        }

        public static List<string> GetNumberVariationsForIllegibleAccount(string number)
        {
            var correctAccountVariations = new List<string>();

            for (int i = 0; i < 9; i++)
            {
                var newNumber = number.Replace("?", i.ToString());

                if (newNumber.IsValid())
                {
                    correctAccountVariations.Add(newNumber);
                }
            }

            return correctAccountVariations;
        }

        public static AccountNumberWithPostscript GetPostscript(this AccountNumberWithPostscript accNumberWithPostscript)
        {
            if (accNumberWithPostscript.AccountNumber.Contains("?"))
            {
                accNumberWithPostscript.Postscript = ILLEGIBLE;
            }
            else if (!accNumberWithPostscript.AccountNumber.IsValid())
            {
                accNumberWithPostscript.Postscript = ERROR;
            }

            return accNumberWithPostscript;
        }
    }
}
