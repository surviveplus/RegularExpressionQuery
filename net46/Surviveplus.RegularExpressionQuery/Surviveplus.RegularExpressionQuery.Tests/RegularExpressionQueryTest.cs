using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Net.Surviveplus.RegularExpressionQuery;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Surviveplus.RegularExpressionQuery.Tests
{
    public class Sample
	{
		public string lang { get; set; }

		public int number1 { get; set; }

		public int number2 { get; set; }
	}
	[TestClass]
	public class StringExtensionsTest
	{


		[TestMethod]
		public void MatchesTest()
		{
			var a = (from b in "http://example.com/ja-jp/session/10/item/20".Matches<Sample>(@"^.+\/(?<lang>ja\-jp)\/.+\/(?<number1>\d+)\/.+\/(?<number2>\d+)(.*)$") select b).FirstOrDefault();

			Assert.AreEqual("ja-jp", a?.lang);
			Assert.AreEqual(10, a?.number1);
			Assert.AreEqual(20, a?.number2);

		} // end function

		[TestMethod]
		public void ReplaceTest()
		{
			var a = new Sample { lang = "en-us", number1 = 11, number2 = 21 };
			var result = "http://example.com/ja-jp/session/10/item/20".Replace<Sample>(@"^.+\/(?<lang>ja\-jp)\/.+\/(?<number1>\d+)\/.+\/(?<number2>\d+)(.*)$", a);

			Assert.AreEqual("http://example.com/en-us/session/11/item/21", result);
        } // end function

		[TestMethod]
		public void MatchesAndReplaceTest()
		{
			var url = "http://example.com/ja-jp/session/10/item/20";
			var pattern = @"^.+\/(?<lang>ja\-jp)\/.+\/(?<number1>\d+)\/.+\/(?<number2>\d+)(.*)$";

			var a = (from b in url.Matches<Sample>(pattern) select b).FirstOrDefault();
			Assert.IsNotNull(a);
			a.number1 += 1;
			a.number2 += 1;

			var result = url.Replace<Sample>(pattern, a);
			Assert.AreEqual("http://example.com/ja-jp/session/11/item/21", result);

		} // end function

		[TestMethod]
		public void ReplaceTestMeny()
		{
			var a = new Sample[]{ new Sample { lang = "en-us", number1 = 11, number2 = 21 },  new Sample { lang = "en-us", number1 = 12, number2 = 22 }};
			var result = "http://example.com/ja-jp/session/10/item/20\nhttp://example.com/ja-jp/session/10/item/20".Replace<Sample>(@"^.+\/(?<lang>ja\-jp)\/.+\/(?<number1>\d+)\/.+\/(?<number2>\d+)(.*)$", a);

			Assert.AreEqual("http://example.com/en-us/session/11/item/21\nhttp://example.com/en-us/session/12/item/22", result);
		} // end function

        [TestMethod]
        public void ReplaceTestGroupMeny()
        {
            var a = new Sample { lang = "en-us", number1 = 11, number2 = 21 };
            var result = "http://example.com/ja-jp/session/10/item/20".Replace<Sample>(@"^.+\/(?<lang>ja\-jp)\/(.+)\/(?<number1>\d+)\/(.+)\/(?<number2>\d+)(.*)$", a);

            Assert.AreEqual("http://example.com/en-us/session/11/item/21", result);
        }

    } // end class
} // end namespace
