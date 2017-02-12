using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Net.Surviveplus.RegularExpressionQuery;

namespace Net.Surviveplus.RegularExpressionQuery.Tests
{
    [TestClass]
    public class IEnumerableQuerableFilePathExtensionsTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhereMatchTest()
        {

            // TestFolder	

            var a = System.Reflection.Assembly.GetExecutingAssembly();
            var folder = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(a.Location), "TestFolder"));
            var target = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "TestFolder"));
            var dest = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "DestFolder"));

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(folder.FullName, target.FullName);

            var results = target.Query();

            Debug.WriteLine("");
            foreach (var item in results)
            {
                Debug.WriteLine(item.RelativePath);
            }

            var r2 = results.WhereFileNameMatch(@"(\d)\.xml$");

            Debug.WriteLine("");
            foreach (var item in r2)
            {
                Debug.WriteLine(item.RelativePath);
            }
            Assert.AreEqual(2, r2.Count());
            Assert.AreEqual(@"XMLFile1.xml", r2.FirstOrDefault()?.RelativePath);
            Assert.AreEqual(@"NewFolder1\XMLFile2.xml", r2.Skip(1).FirstOrDefault()?.RelativePath);

            foreach (var item in r2)
            {
                item.CopyTo(dest, false);
            }

            dest.Refresh();
            var destResults = dest.Query();
            Debug.WriteLine("");
            foreach (var item in destResults)
            {
                Debug.WriteLine(item.RelativePath);
            }

        }

        [TestMethod]
        public void ReplaceEachLineTest()
        {

            // TestFolder	

            var a = System.Reflection.Assembly.GetExecutingAssembly();
            var folder = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(a.Location), "TestFolder"));
            var target = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "TestFolder2"));
            var dest = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "DestFolder2"));

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(folder.FullName, target.FullName);

            var results = target.Query();

            Debug.WriteLine("");
            foreach (var item in results)
            {
                Debug.WriteLine(item.RelativePath);
            }

            var r2 = results.WhereExtensionMatch(@"\.csproj");

            Debug.WriteLine("");
            foreach (var item in r2)
            {
                Assert.AreEqual(true, item.IsMatch(@"(?<sak>\<(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>SAK\<\/(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>)"));
                Debug.WriteLine(item.RelativePath);
            }
            Assert.AreEqual(1, r2.Count());
            Assert.AreEqual(@"NewFolder1\Sample.csproj", r2.FirstOrDefault()?.RelativePath);

            foreach (var item in r2)
            {
                //item.CopyTo(dest, false).ReplaceEachLine(
                //	@"(?<sak>\<(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>SAK\<\/(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>)",
                //                 new { sak=""});

                item.CopyTo(dest, false).RemoveMatchedLines(
                        @"(?<sak>\<(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>SAK\<\/(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>)"
                    );
            }

            dest.Refresh();
            var destResults = dest.Query();
            Debug.WriteLine("");
            foreach (var item in destResults)
            {
                if (item.RelativePath.EndsWith(".csproj"))
                {
                    Assert.AreEqual(false, item.IsMatch(@"(?<sak>\<(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>SAK\<\/(SccProjectName|SccLocalPath|SccAuxPath|SccProvider)\>)"));
                }
                Debug.WriteLine(item.RelativePath);
            }

            var r3 = results.WhereExtensionMatch(@"\.sln");
            Debug.WriteLine("");
            foreach (var item in r3)
            {
                Assert.AreEqual(true, item.IsMatch(@"(?<tfvc>\tGlobalSection\(TeamFoundationVersionControl\)(.+)\tEndGlobalSection)", System.Text.RegularExpressions.RegexOptions.Singleline));
                Debug.WriteLine(item.RelativePath);
            }

            foreach (var item in r3)
            {
                item.CopyTo(dest, false).Replace(
                    @"(?<tfvc>\tGlobalSection\(TeamFoundationVersionControl\)(.+)\tEndGlobalSection)",
                    new { tfvc = "" },
                    System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            var destResults2 = dest.Query();
            Debug.WriteLine("");
            foreach (var item in destResults2)
            {
                if (item.RelativePath.EndsWith(".sln"))
                {
                    Assert.AreEqual(false, item.IsMatch(@"(?<tfvc>\tGlobalSection\(TeamFoundationVersionControl\)(.+)\tEndGlobalSection)", System.Text.RegularExpressions.RegexOptions.Singleline));
                }
                Debug.WriteLine(item.RelativePath);
            }

        } // end function

    } // end class
} // end namespace
