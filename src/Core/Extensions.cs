using System;
using System.Collections.Generic;

namespace RCAudioPlayer.Core
{
	public static class Extensions
	{
		static public string ToStr(this TimeSpan time)
		{
			if (time.TotalHours >= 1)
				return time.ToString(@"h\:mm\:ss");
			else
				return time.ToString(@"m\:ss");
		}

		static public string ToStr(this object? value, int digits = 2)
		{
			if (value is double d)
				return Math.Round(d, digits).ToString();
			if (value is float f)
				return Math.Round(f, digits).ToString();
			return value?.ToString() ?? "null";
		}

		static public V? Get<K, V>(this IDictionary<K, V> dict, K key) where K : notnull
			where V : class
		{
			if (dict.TryGetValue(key, out V? value))
				return value;
			return null;
		}

		static public void Skip<T>(this IEnumerable<T> enumerable)
		{
			foreach (var _ in enumerable);
		}

		static public async System.Threading.Tasks.Task Skip<T>(this IAsyncEnumerable<T> enumerable)
		{
			await foreach (var _ in enumerable);
		}
	}
}