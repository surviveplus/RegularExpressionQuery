using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Surviveplus.RegularExpressionQuery
{
    /// <summary>
    /// Static class which is defined extension methods for System.IO.DirectoryInfo.
    /// </summary>
    public static class DirectoryInfoExtensions
    {

        /// <summary>
        /// Get IEnumerable&lt;QuerableFilePath&gt; for all of files of a directory recursively. 
        /// This method is eager evaluation.
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns IEnumerable&lt;QuerableFilePath&gt.
        /// </returns>
        public static IEnumerable<QuerableFilePath> Query(this System.IO.DirectoryInfo me)
        {
            if (me == null) throw new ArgumentNullException("me");
            me.Refresh();

            var results = new List<QuerableFilePath>();

            Action<System.IO.DirectoryInfo, Action<System.IO.DirectoryInfo>> recursion = null;
            recursion = (folder, exec) =>
            {
                if (folder.Exists == false)
                {
                    return;
                } // end if

                exec(folder);

                foreach (var item in folder.GetDirectories())
                {
                    recursion(item, exec);
                } // next item
            };

            recursion(me, (folder) =>
            {
                results.Add(new QuerableFilePath { RootFolder = me, RelativePath = folder.FullName.Replace(me.FullName, "") });

                foreach (var item in folder.GetFiles())
                {
                    results.Add(new QuerableFilePath { RootFolder = me, RelativePath = item.FullName.Replace(me.FullName, "") });
                }
            });

            return results;

        } // end function
    } // end class
} // end namespace
