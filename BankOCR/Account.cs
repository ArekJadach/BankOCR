using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankOCR
{
    public static class Account
    {
        public const string ILLEGIBLE = " ILL";
        public const string ERROR = " ERR";

        public static string GetDecodedAccountNumberWithDescription(string input)
        {
            var number = Decoder.ParseToAccountNumber(input);

            if (number.Contains("?"))
            {
                return number + ILLEGIBLE;
            }

            if (!number.IsValid())
            {
                return number + ERROR;
            }

            return number;
        }

        public static string GetDecodedNumberWithPossibleVariations(string input)
        {
            var numberWithComment = GetDecodedAccountNumberWithDescription(input);

            var allValidVariations = GetPossibleVariationsForAccountNumber(numberWithComment);

            if (allValidVariations.Count() == 1)
            {
                return allValidVariations.First().ToString();
            }

            var variations = string.Join(", ", allValidVariations.Select(acc => $"'{acc}'"));

            return $"{numberWithComment.Substring(0, 9)} AMB [{variations}]";
        }

        public static IEnumerable<string> GetPossibleVariationsForAccountNumber(string accountNumberWithPostscript)
        {
            var correctAccountVariations = new List<string>();

            if (accountNumberWithPostscript.Contains(ERROR))
            {
                correctAccountVariations.AddRange(GetNumberVariations(accountNumberWithPostscript.Substring(0, 9)).OrderBy(i => i));
            }

            if (accountNumberWithPostscript.Contains(ILLEGIBLE))
            {
                correctAccountVariations.AddRange(GetNumberVariationsForIllegibleAccount(accountNumberWithPostscript.Substring(0, 9)));
            }

            if (accountNumberWithPostscript.Length == 9)
            {
                correctAccountVariations.AddRange(GetNumberVariations(accountNumberWithPostscript.Substring(0, 9)).OrderBy(i => i));
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
    }
}
