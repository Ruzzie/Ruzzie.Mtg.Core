using System;
using FsCheck;
using NUnit.Framework;


namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class KeyHelperTests
    {
        [FsCheck.NUnit.Property]
        public Property CreateValidUpperCaseKeyForStringPropertyTests(string input, string forwardSlashReplaceValue)
        {
            Func<bool> check = () =>
                !string.IsNullOrWhiteSpace(
                    input.CreateValidUpperCaseKeyForString(forwardSlashReplaceValue));
            return check.When(!string.IsNullOrWhiteSpace(input));
        }

        [FsCheck.NUnit.Property]
        public void CreateValidUpperCaseKeyForStringQuickCheck(string input, string forwardSlashReplaceValue)
        {
            input.CreateValidUpperCaseKeyForString(forwardSlashReplaceValue);
        }       

        [FsCheck.NUnit.Property]
        public Property GenerateBidirectionalKeyForTwoULongsIsOrderIndependent(ulong one, ulong two)
        {
            Func<bool> check = () =>
                one.GenerateBidirectionalKeyForTwoULongs(two) 
                ==
                two.GenerateBidirectionalKeyForTwoULongs(one);
            return check.ToProperty();
        }
        
        [FsCheck.NUnit.Property]
        public void GenerateHash64ForStringCaseInsensitiveQuickCheck(string input)
        {            
            input.GenerateHash64ForStringCaseInsensitive();
        }
    }
}
