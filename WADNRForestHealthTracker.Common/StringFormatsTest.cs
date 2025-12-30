// TODO .net migration tests

///*-----------------------------------------------------------------------
//<copyright file="StringFormatsTest.cs" company="Sitka Technology Group">
//Copyright (c) Sitka Technology Group. All rights reserved.
//<author>Sitka Technology Group</author>
//</copyright>

//<license>
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Affero General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

//Source code is available upon request via <support@sitkatech.com>.
//</license>
//-----------------------------------------------------------------------*/
//using System.Collections.Generic;
//using System.Web;
//using NUnit.Framework;

//namespace WADNRForestHealthTracker.Common
//{
//    [TestFixture]
//    public class StringFormatsTest
//    {
//        [Test]
//        public void ParseNullableDecimalFromCurrencyString()
//        {
//            Assert.That(StringFormats.ParseNullableDecimalFromCurrencyString("$100"), Is.EqualTo(100m));
//            Assert.That(StringFormats.ParseNullableDecimalFromCurrencyString("$200,000"), Is.EqualTo(200000m));
//            Assert.That(StringFormats.ParseNullableDecimalFromCurrencyString("$100.10"), Is.EqualTo(100.10m));
//            Assert.That(StringFormats.ParseNullableDecimalFromCurrencyString("-$100.10"), Is.EqualTo(-100.10m));
//            Assert.That(StringFormats.ParseNullableDecimalFromCurrencyString("($100.10)"), Is.EqualTo(-100.10m));
//        }

//        [Test]
//        public void FormatFileSizeHumanReadableTest()
//        {
//            Assert.That(StringFormats.ToHumanReadableByteSize(0), Is.EqualTo("0 B"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024), Is.EqualTo("1 KB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024 * 1024), Is.EqualTo("1 MB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024 * 1024 + 512 * 1024), Is.EqualTo("1.5 MB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024 * 1024 + 231 * 1024), Is.EqualTo("1.23 MB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024 * 1024 * 1024), Is.EqualTo("1 GB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024L * 1024L * 1024L * 1024L), Is.EqualTo("1 TB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024L * 1024L * 1024L * 1024L * 1024L), Is.EqualTo("1 PB"));
//            Assert.That(StringFormats.ToHumanReadableByteSize(1024L * 1024L * 1024L * 1024L * 1024L * 1024L), Is.EqualTo("1 EB"));
//        }

//        [Test]
//        public void MakeAbsoluteLinksToApplicationDomainRelativeNullTest()
//        {
//            var result = StringFormats.MakeAbsoluteLinksToApplicationDomainRelative(null);

//            Assert.That(result, Is.Null);
//        }

//        [Test]
//        public void MakeAbsoluteLinksToApplicationDomainRelativeInnerNullTest()
//        {
//            var htmlString = new HtmlString(null);
//            var result = htmlString.MakeAbsoluteLinksToApplicationDomainRelative();

//            Assert.That(result, Is.EqualTo(htmlString));
//        }

//        [Test]
//        public void MakeAbsoluteLinksToApplicationDomainRelativeStringEmptyTest()
//        {
//            var result = new HtmlString(string.Empty).MakeAbsoluteLinksToApplicationDomainRelative();

//            Assert.That(result.ToString(), Is.EqualTo(string.Empty));
//        }

//        [Test]
//        public void MakeAbsoluteLinksToApplicationDomainRelativeActualAppDomainTest()
//        {
//            const string relativeUrl = "/awesome/awesomepage.cshtml";
//            var absoluteUrl = $"http://{WADNRForestHealthTrackerWebConfiguration.ApplicationDomain}{relativeUrl}";
//            var result = new HtmlString(absoluteUrl).MakeAbsoluteLinksToApplicationDomainRelative();

//            Assert.That(result.ToString(), Is.EqualTo(relativeUrl));
//        }

//        [Test]
//        public void MakeAbsoluteLinksToApplicationDomainRelativeOutsideDomainTest()
//        {
//            const string relativeUrl = "/awesome/awesomepage.cshtml";
//            var absoluteUrl = $"https://{"example.org"}{relativeUrl}";
//            var result = new HtmlString(absoluteUrl).MakeAbsoluteLinksToApplicationDomainRelative();

//            Assert.That(result.ToString(), Is.EqualTo(absoluteUrl));
//        }

//        [Test, TestCaseSource(nameof(TestToRangeStringTestCases))]
//        public string TestToRangeString(List<int> numbers)
//        {
//            return numbers.ToRangeString();
//        }

//        public static IEnumerable<TestCaseData> TestToRangeStringTestCases
//        {
//            get
//            {
//                yield return new TestCaseData(null).Returns("").SetDescription("Returns empty string for null list");
//                yield return new TestCaseData(new List<int>()).Returns("").SetDescription("Returns empty string for empty list");
//                yield return new TestCaseData(new List<int> {1, 2, 3, 5, 6, 8, 10, 11, 12}).Returns("1 - 3, 5 - 6, 8, 10 - 12").SetDescription("Returns range in a normal case");
//                yield return new TestCaseData(new List<int> {1, 2, 2, 2, 3, 5, 6, 8, 10, 11, 12}).Returns("1 - 3, 5 - 6, 8, 10 - 12").SetDescription("Returns range of list with duplicates");
//                yield return new TestCaseData(new List<int> {5, 6, 8, 3, 10, 11, 1, 2, 12}).Returns("1 - 3, 5 - 6, 8, 10 - 12").SetDescription("Returns range of unordered list");
//            }
//        }
//    }
//}