using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Net.Surviveplus.RegularExpressionQuery
{
    /// <summary>
    /// Static class which is defined extension methods for IEnumerable&lt;QuerableFilePath&gt;.
    /// </summary>
    public static class IEnumerableQuerableFilePathExtensions
    {
        /// <summary>
        /// Copy the file to path which is combined with specified destination folder and RelativePath property.
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <param name="destinationRootFolder">The location of root path to which the file should be copied. </param>
        /// <param name="overWrite">True if existing files should be overwritten; otherwise False. Default is False. </param>
        /// <returns>Get new instance of IEnumerable&lt;QuerableFilePath&gt; for destination file.</returns>
        public static IEnumerable<QuerableFilePath> ToCopy(this IEnumerable<QuerableFilePath> me, System.IO.DirectoryInfo destinationRootFolder, bool overWrite)
        {
            return (from item in me select item.CopyTo(destinationRootFolder, overWrite)).ToList();
        } // end function

        public static IEnumerable<System.IO.FileInfo> ToFileInfo(this IEnumerable<QuerableFilePath> me)
        {
            return (from item in me select item.ToFileInfo()).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> Replace<T>(this IEnumerable<QuerableFilePath> me, string pattern, T value, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.Replace(pattern, value, options)).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> Replace<T>(this IEnumerable<QuerableFilePath> me, string pattern, IEnumerable<T> values, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.Replace(pattern, values, options)).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> ReplaceEachLine<T>(this IEnumerable<QuerableFilePath> me, string pattern, T value, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.ReplaceEachLine(pattern, value, options)).ToList();
        }

        public static IEnumerable<QuerableFilePath> ReplaceEachLine<T>(this IEnumerable<QuerableFilePath> me, string pattern, IEnumerable<T> values, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.ReplaceEachLine(pattern, values, options)).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> RemoveMatchedLines<T>(this IEnumerable<QuerableFilePath> me, string pattern, Func<T, bool> predicate, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.RemoveMatchedLines(pattern, predicate, options)).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> RemoveMatchedLines(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.RemoveMatchedLines(pattern, options)).ToList();
        } // end function

        public static IEnumerable<T> Matches<T>(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            return (
                from item in me
                from r in item.Matches<T>(pattern, options)
                select r).ToList();
        } // end function

        public static bool IsMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            return (from item in me select item.IsMatch(pattern, options)).Count() > 0;
        } // end function

        public static IEnumerable<QuerableFilePath> WhereMatch(IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            return (from item in me where item.IsMatch(pattern, options) select item).ToList();
        } // end function

        #region Where_Match

        public static IEnumerable<QuerableFilePath> WhereRelativePathMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.RelativePath) select item).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> WhereFileNameMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.ToFileInfo().Name) select item).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> WhereExtensionMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.ToFileInfo().Extension) select item).ToList();
        } // end function

        #endregion

        #region Where_NotMatch

        public static IEnumerable<QuerableFilePath> WhereRelativePathNotMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.RelativePath) == false select item).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> WhereFileNameNotMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.ToFileInfo().Name) == false select item).ToList();
        } // end function

        public static IEnumerable<QuerableFilePath> WhereExtensionNotMatch(this IEnumerable<QuerableFilePath> me, string pattern, RegexOptions options = RegexOptions.None)
        {
            var r = new Regex(pattern, options);
            return (from item in me where r.IsMatch(item.ToFileInfo().Extension) == false select item).ToList();
        } // end function

        #endregion

    } // end class
} // end namespace
