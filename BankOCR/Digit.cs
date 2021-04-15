using System.Collections.Generic;

namespace BankOCR
{
    public static class Digit
    {
        public static readonly Dictionary<int, int[]> DigitsSimilarities = new Dictionary<int, int[]>()
        {
            { 0, new int[] { 8 } },
            { 1, new int[] { 7 } },
            { 2, new int[] { } },
            { 3, new int[] { 9 } },
            { 4, new int[] { } },
            { 5, new int[] { 6, 9 } },
            { 6, new int[] { 5, 8 } },
            { 7, new int[] { 1 } },
            { 8, new int[] { 0, 6, 9 } },
            { 9, new int[] { 3, 5, 8 } }
        };
    }
}
