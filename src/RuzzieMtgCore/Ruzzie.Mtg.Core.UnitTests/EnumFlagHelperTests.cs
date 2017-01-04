using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class EnumFlagHelperTests
    {
        [Test]
        public void ListOfAllPossibleUniqueFlagsCombinationsTest()
        {
            Assert.That(EnumFlagHelpers<Color>.ListAllPossibleUniqueFlagsCombinations().Count, Is.EqualTo(16));
        }

        [Test]
        public void ListOfAllPossibleUniqueForBasicTypesTest()
        {
            Assert.That(EnumFlagHelpers<BasicType>.ListAllPossibleUniqueFlagsCombinations().Count, Is.EqualTo(37));
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
                    .And.Contains(LongFlagTypeEnum.B | LongFlagTypeEnum.C));
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