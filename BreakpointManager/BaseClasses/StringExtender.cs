using System;
using System.Text;

namespace BreakpointManager.BaseClasses
{
	public static class StringExtender
	{
		public static string ToStringValue(this object rawValue)
		{
			if (rawValue == null)
				return string.Empty;
			else if (rawValue is string)
				return (string)rawValue;
			else
				return rawValue.ToString();
		}

		public static bool IsNotNullOrEmpty(this string value)
				=> !string.IsNullOrEmpty(value);
		public static bool IsNullOrEmpty(this string value)
				=> string.IsNullOrEmpty(value);

		public static byte[] ToByteArray(this string value)
				=> Encoding.UTF8.GetBytes(value);

		public static string GetRtfUnicodeEscapedString(this string value)
		{
			var sb = new StringBuilder();
			foreach (var c in value)
			{
				if (c == '\\' || c == '{' || c == '}')
					sb.Append(@"\" + c);
				else if (c <= 0x7f)
					sb.Append(c);
				else
					sb.Append("\\u" + Convert.ToUInt32(c) + "?");
			}
			return sb.ToString();
		}

	}
}
