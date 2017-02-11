using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Net.Surviveplus.RegularExpressionQuery
{

    /// <summary>
    /// Static class which is defined extension methods.
    /// </summary>
    public static class StringExtensions
	{

		/// <summary>
		/// Get a instance which extracted values from the string by using regular expression pattern.
		/// </summary>
		/// <typeparam name="T">Set a type of instance. You must define the class which has properties that name are same of the group name of pattern.</typeparam>
		/// <param name="me">The instance of the type which is added this extension method.</param>
		/// <param name="pattern">Set regular expression pattern string.</param>
		/// <returns>
		/// An IEnumerable&gt;T&lt; whose elements are the result of  matching of regular expression.
		/// </returns>
		public static IEnumerable<T> Matches<T>(this String me, string pattern)
		{
			if (me == null) throw new ArgumentNullException("me");

			var r = new Regex(pattern);

			var matches = r.Matches(me);
			if (matches.Count > 0)
			{

				foreach (Match match in matches)
				{
					T b = Activator.CreateInstance<T>();

					foreach (string groupName in r?.GetGroupNames())
					{
						try
						{
							var value = match.Groups[groupName].Value;
							var prop = typeof(T).GetProperty(groupName);

							var parseMethod = prop?.PropertyType?.GetMethod("Parse",new Type[]{typeof( string)}  );
							object typedValue = parseMethod?.Invoke(null, new object[] { value }) ?? value;

							typeof(T).GetProperty(groupName)?.SetValue(b, typedValue);
						}
						catch (Exception ex)
						{
							Debug.WriteLine("groupName:" + groupName ?? "(null)");
							Debug.WriteLine(ex.ToString());
						}
					}

					yield return b;
				}
			}

		} // end function


		/// <summary>
		/// Replace a text by using regular expression pattern.
		/// </summary>
		/// <typeparam name="T">Set a type of instance which has values.  You must define the class which has properties that name are same of the group name of pattern.</typeparam>
		/// <param name="me">The instance of the type which is added this extension method.</param>
		/// <param name="pattern">Set regular expression pattern string.</param>
		/// <param name="value">Set a instance whitch has values.</param>
		/// <returns>A string that all the values were replaced with.</returns>
		public static string Replace<T>(this String me, string pattern, T value) {
			return me.Replace<T>( pattern, new T[] { value } );
		}

		private class ReplaceUnit<T>{
			public string Name { get; set; }
			public Group Group { get; set; }
			public T Value { get; set; }
		}

		/// <summary>
		/// Replace a text by using regular expression pattern.
		/// </summary>
		/// <typeparam name="T">Set a type of instance which has values.  You must define the class which has properties that name are same of the group name of pattern.</typeparam>
		/// <param name="me">The instance of the type which is added this extension method.</param>
		/// <param name="pattern">Set regular expression pattern string.</param>
		/// <param name="values">Set IEnumerable&gt;T&lt; of instances whitch has values.</param>
		/// <returns>A string that all the values were replaced with.</returns>
		public static string Replace<T>(this String me, string pattern, IEnumerable<T> values)
		{
			if (me == null) throw new ArgumentNullException("me");
			var r = new Regex(pattern, RegexOptions.Multiline);

			var list = new SortedList<int, ReplaceUnit<T>>();

			var matches = r.Matches(me);
			if (matches.Count > 0)
			{
				var ve= values.GetEnumerator();
				foreach (Match match in matches)
				{
					ve.MoveNext();
					var value = ve.Current;

					foreach (var item in from name in r.GetGroupNames() select new {name = name, Group= match.Groups[name] })
					{
                        int dummy;
						if (int.TryParse( item.name, out dummy) ) continue;
						list[item.Group.Index] = new ReplaceUnit<T>() { Name = item.name, Group = item.Group, Value = value };
					}
				}

				var result = new StringBuilder();
				var lastIndex = 0;
				Action<int, int> append = (start, length) =>
				{
					var text = me.Substring(start, length);
					result.Append(text);
					lastIndex = start + length;
				};
				Action<int, int, string> replace = (start, length, text) =>
				{
					result.Append(text);
					lastIndex = start + length;
				};

				foreach (var kvp in list)
				{
					var nextIndex = kvp.Key;
					if (lastIndex < nextIndex)
					{
						append(lastIndex, nextIndex - lastIndex);
					} // end if

					var value = kvp.Value.Value;
					var t = value.GetType().GetProperty(kvp.Value.Name);
					var newText = t?.GetValue(value)?.ToString() ?? "";

					replace(nextIndex, kvp.Value.Group.Length, newText);
				} // next kvp

				if (lastIndex < me.Length)
				{
					result.Append(me.Substring(lastIndex));
				}// end if

				return result.ToString();
			}
			else
			{
				return me;
			} // end if

		} // end function
	} // end class



} // end namespace

