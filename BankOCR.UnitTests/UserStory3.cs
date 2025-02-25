﻿using NUnit.Framework;

namespace BankOCR.UnitTests
{
    public class UserStory3
    {
        [TestCase(@"
 _  _  _  _  _  _  _  _     
| || || || || || || ||_   |
|_||_||_||_||_||_||_| _|  |", "000000051")]
        [TestCase(@"
    _  _  _  _  _  _     _ 
|_||_|| || ||_   |  |  | _ 
  | _||_||_||_|  |  |  | _|", "49006771? ILL")]
        [TestCase(@"
    _  _     _  _  _  _  _ 
  | _| _||_| _ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _ ", "1234?678? ILL")]
        public void Tests(string input, string expectedResult)
        {
            Assert.That(Account.GetDecodedAccountNumberWithPostscriptAsString(input), Is.EqualTo(expectedResult));
        }
    }
}