namespace BankOCR
{
    public static class ControlSum
    {
        private static readonly int[] Weights = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        private static int Calculate(string accountNumber)
        {
            int controlSum = 0;

            for (int i = 0; i < accountNumber.Length; i++)
            {
                controlSum += Weights[i] * int.Parse(accountNumber[i].ToString());
            }

            return controlSum;
        }

        public static bool IsValid(this string accountNumber)
        {
            int controlSum = Calculate(accountNumber);

            if (controlSum % 11 == 0)
            {
                return true;
            }

            return false;
        }
    }
}
