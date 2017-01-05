using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class EnumFlagHelperTests
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(6)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(18)]
        [TestCase(20)]
        [TestCase(21)]
        [TestCase(24)]
        [TestCase(25)]
        [TestCase(26)]
        [TestCase(28)]
        public void ListOfAllPossibleUniqueFlagsCombinationsFlagsTest(int colorCode)
        {
            var listAllPossibleUniqueFlagsCombinations = EnumFlagHelpers<Color>.ListAllPossibleUniqueFlagsCombinations();           
            Assert.That(listAllPossibleUniqueFlagsCombinations, Contains.Item((Color) colorCode));
        }

        [Test]
        public void ListOfAllPossibleUniqueFlagsCombinationsTest()
        {
            Assert.That(EnumFlagHelpers<Color>.ListAllPossibleUniqueFlagsCombinations().Count, Is.EqualTo(32));
        }

        [Test]
        public void ListOfAllPossibleUniqueForBasicTypesTest()
        {
            var listAllPossibleUniqueFlagsCombinations = EnumFlagHelpers<BasicType>.ListAllPossibleUniqueFlagsCombinations();
            Assert.That(listAllPossibleUniqueFlagsCombinations.Count, Is.EqualTo(158));
        }

        [Test]
        public void ListOfAllPossibleUniqueForShortEnum()
        {
            Assert.That(EnumFlagHelpers<ShortFlagTypeEnum>.ListAllPossibleUniqueFlagsCombinations().Count, Is.EqualTo(4));
        }

        [Test]
        public void ListOfAllPossibleUniqueForUintEnum()
        {
            Assert.That(EnumFlagHelpers<UintFlagTypeEnum>.ListAllPossibleUniqueFlagsCombinations().Count, Is.EqualTo(4));
        }

        [Test]
        public void ListOfAllPossibleUniqueForLongEnum()
        {
            IReadOnlyCollection<LongFlagTypeEnum> combinations = EnumFlagHelpers<LongFlagTypeEnum>.ListAllPossibleUniqueFlagsCombinations();

            Assert.That(combinations.Count, Is.EqualTo(4));
            Assert.That(combinations,
                Contains.Item(LongFlagTypeEnum.A)
                    .And.Contains(LongFlagTypeEnum.B)
                    .And.Contains(LongFlagTypeEnum.C)
                    .And.Contains(LongFlagTypeEnum.B | LongFlagTypeEnum.C)            
                    );
        }

        [Test]
        public void NonEnumTypeThrowsException()
        {
            Assert.That(EnumFlagHelpers<int>.ListAllPossibleUniqueFlagsCombinations, Throws.ArgumentException);
        }

        [Test]
        public void NonEnumFlagTypeThrowsException()
        {
            Assert.That(EnumFlagHelpers<NonFlagTypeEnum>.ListAllPossibleUniqueFlagsCombinations, Throws.ArgumentException);
        }

        // ReSharper disable UnusedMember.Local
        enum NonFlagTypeEnum
        {           
            None,
            A,
            B
        }

        [Flags]
        enum ShortFlagTypeEnum : short
        {
            A = 0,
            B = 1,
            C = 2
        }

        [Flags]
        enum UintFlagTypeEnum : uint
        {
            A = 0,
            B = 1,
            C = 2
        }
        [Flags]
        enum LongFlagTypeEnum : long
        {
            A = 0,
            B = 1,
            C = 2
        }
        // ReSharper restore UnusedMember.Local
    }
}