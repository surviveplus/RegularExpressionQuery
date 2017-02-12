using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Net.Surviveplus.RegularExpressionQuery
{
    /// <summary>
    /// Control a file relative path from a specified path.
    /// You can get IEnumerable&lt;QuerableFilePath&gt; from System.IO.DirectoryInfo Query extension method,
    /// to query by using regular expression, for example to filter whether its Relative property match a pattern.
    /// </summary>
    public class QuerableFilePath
    {
        #region Properties

        /// <summary>
        /// Get or set a folder path of a file of this instance. It is base of RelativePath property.
        /// </summary>
        public System.IO.DirectoryInfo RootFolder { get; set; }


        /// <summary>
        /// A backing field of QuerableFilePath property.
        /// </summary>
        private string valueOfRelativePath;

        /// <summary>
        /// Get or set a file path of this instance. It is relative from RootFolder property.
        /// </summary>
        public string RelativePath
        {
            get
            {
                return this.valueOfRelativePath;
            }
            set
            {
                this.valueOfRelativePath = value?.Trim();

                if (this.valueOfRelativePath?.StartsWith("\\") ?? false)
                {
                    this.valueOfRelativePath = this.valueOfRelativePath.Substring(1);
                } // end if
            }
        } // end property
        #endregion


        #region Methods

        /// <summary>
        /// Copy the file to path which is combined with specified destination folder and RelativePath property.
        /// </summary>
        /// <param name="destinationRootFolder">The location of root path to which the file should be copied. </param>
        /// <param name="overWrite">True if existing files should be overwritten; otherwise False. Default is False. </param>
        /// <returns>Get new instance of QuerableFilePath for destination file.</returns>
        public QuerableFilePath CopyTo(System.IO.DirectoryInfo destinationRootFolder, bool overWrite)
        {
            var sourceFile = this.ToFileInfo();
            var destinationFile = new QuerableFilePath { RootFolder = destinationRootFolder, RelativePath = this.RelativePath };
            if (sourceFile.Exists)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sourceFile.FullName, destinationFile.ToFileInfo().FullName, overWrite);
            } // end if

            return destinationFile;

            // TODO: empty folder copy; if RelativePath is directory then, make destination directory.
        } // end function

        public System.IO.FileInfo ToFileInfo()
        {
            return new System.IO.FileInfo(System.IO.Path.Combine(this.RootFolder.FullName, this.RelativePath));
        } // end function

        public QuerableFilePath Replace<T>(string pattern, T value, RegexOptions options = RegexOptions.None)
        {
            return this.Replace<T>(pattern, new T[] { value }, options);
        }

        public QuerableFilePath Replace<T>(string pattern, IEnumerable<T> values, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();

            string newText = null;
            using (var reader = new System.IO.StreamReader(f.FullName))
            {
                var originalText = reader.ReadToEnd();
                newText = originalText.Replace(pattern, values, options);
            } // en using(reader)

            var tempFile = new System.IO.FileInfo(f.FullName + "." + System.DateTime.Now.ToFileTimeUtc().ToString() + ".tmp");

            using (var writer = new System.IO.StreamWriter(tempFile.FullName))
            {
                writer.Write(newText);
            } // end using(writer)

            //
            // If the sourceFileName and destinationFileName are on different volumes, this method will raise an exception.
            // System.IO.IOException: Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
            //
            tempFile.Replace(f.FullName, f.FullName + ".backup", true);

            return this;
        } // end function

        public QuerableFilePath ReplaceEachLine<T>(string pattern, T value, RegexOptions options = RegexOptions.None)
        {
            return this.ReplaceEachLine<T>(pattern, new T[] { value }, options);
        }

        public QuerableFilePath ReplaceEachLine<T>(string pattern, IEnumerable<T> values, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();

            var tempFile = new System.IO.FileInfo(f.FullName + "." + System.DateTime.Now.ToFileTimeUtc().ToString() + ".tmp");

            using (var reader = new System.IO.StreamReader(f.FullName))
            using (var writer = new System.IO.StreamWriter(tempFile.FullName))
            {
                while (reader.EndOfStream == false)
                {
                    var originalText = reader.ReadLine();
                    var newText = originalText.Replace(pattern, values, options);
                    writer.WriteLine(newText);
                } // next

            } // end using(reader, writer)

            //
            // If the sourceFileName and destinationFileName are on different volumes, this method will raise an exception.
            // System.IO.IOException: Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
            //
            tempFile.Replace(f.FullName, f.FullName + ".backup", true);

            return this;
        } // end function


        public QuerableFilePath RemoveMatchedLines<T>(string pattern, Func<T, bool> predicate, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();

            var tempFile = new System.IO.FileInfo(f.FullName + "." + System.DateTime.Now.ToFileTimeUtc().ToString() + ".tmp");

            using (var reader = new System.IO.StreamReader(f.FullName))
            using (var writer = new System.IO.StreamWriter(tempFile.FullName))
            {
                while (reader.EndOfStream == false)
                {
                    var originalText = reader.ReadLine();
                    var items = originalText.Matches<T>(pattern);

                    var doDelete = false;
                    if (predicate == null)
                    {
                        doDelete = items.Count() > 0;
                    }
                    else
                    {
                        doDelete = (
                                from item in items
                                let itemDelete = predicate(item)
                                where itemDelete
                                select itemDelete
                            ).Count() > 0;
                    }// end if

                    if (doDelete == false)
                    {
                        writer.WriteLine(originalText);
                    } // end if
                } // next

            } // end using(reader, writer)

            //
            // If the sourceFileName and destinationFileName are on different volumes, this method will raise an exception.
            // System.IO.IOException: Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
            //
            tempFile.Replace(f.FullName, f.FullName + ".backup", true);

            return this;
        } // end function

        public QuerableFilePath RemoveMatchedLines(string pattern, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();
            var r = new System.Text.RegularExpressions.Regex(pattern, options);

            var tempFile = new System.IO.FileInfo(f.FullName + "." + System.DateTime.Now.ToFileTimeUtc().ToString() + ".tmp");

            using (var reader = new System.IO.StreamReader(f.FullName))
            using (var writer = new System.IO.StreamWriter(tempFile.FullName))
            {
                while (reader.EndOfStream == false)
                {
                    var originalText = reader.ReadLine();
                    var doDelete = r.IsMatch(originalText);

                    if (doDelete == false)
                    {
                        writer.WriteLine(originalText);
                    } // end if
                } // next

            } // end using(reader, writer)

            //
            // If the sourceFileName and destinationFileName are on different volumes, this method will raise an exception.
            // System.IO.IOException: Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
            //
            tempFile.Replace(f.FullName, f.FullName + ".backup", true);

            return this;
        } // end function


        public IEnumerable<T> Matches<T>(string pattern, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();

            using (var reader = new System.IO.StreamReader(f.FullName))
            {
                var originalText = reader.ReadToEnd();
                var results = originalText.Matches<T>(pattern, options);
                return results;

            } // end using(reader)

        } // end function

        public bool IsMatch(string pattern, RegexOptions options = RegexOptions.None)
        {
            var f = this.ToFileInfo();
            var r = new System.Text.RegularExpressions.Regex(pattern, options);

            using (var reader = new System.IO.StreamReader(f.FullName))
            {
                var originalText = reader.ReadToEnd();
                return r.IsMatch(originalText.Replace("\n", ""));
            } // end using(reader)

        } // end function

        #endregion

    } // end class
} // end namespace
