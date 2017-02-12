using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Surviveplus.RegularExpressionQuery;
using System.Diagnostics;

namespace Net.Surviveplus.RegularExpressionQuery.Tests
{
    [TestClass]
    public class DirectoryInfoExtensionsTest
    {
        /// <summary>
        /// 現在のテストの実行についての情報および機能を提供するテスト コンテキストを取得または設定します。
        /// </summary>
        public TestContext TestContext { get; set; }

        #region 追加のテスト属性

        /// <summary>
        ///  テストを作成するときに、次の追加属性を使用することができます:
        ///  クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        } // end function

        /// <summary>
        /// クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        } // end function

        /// <summary>
        /// 各テストを実行する前にコードを実行するには、TestInitialize を使用
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
        } // end function

        /// <summary>
        /// 各テストを実行した後にコードを実行するには、TestCleanup を使用
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
        } // end function

        #endregion

        [TestMethod]
        public void QueryTest()
        {

            // TestFolder	

            var a = System.Reflection.Assembly.GetExecutingAssembly();
            var folder = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(a.Location), "TestFolder"));
            var target = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "TestFolder1"));
            var dest = new System.IO.DirectoryInfo(System.IO.Path.Combine(this.TestContext.TestRunDirectory, "DestFolder1"));

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(folder.FullName, target.FullName);

            var results = target.Query();

            Debug.WriteLine("");
            foreach (var item in results)
            {
                Debug.WriteLine(item.RelativePath);
            }

            var r2 = from item in results where item.RelativePath.EndsWith("2.xml") select item;

            Debug.WriteLine("");
            foreach (var item in r2)
            {
                Debug.WriteLine(item.RelativePath);
            }
            Assert.AreEqual(1, r2.Count());
            Assert.AreEqual(@"NewFolder1\XMLFile2.xml", r2.FirstOrDefault()?.RelativePath);

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
    }


} // end namespace