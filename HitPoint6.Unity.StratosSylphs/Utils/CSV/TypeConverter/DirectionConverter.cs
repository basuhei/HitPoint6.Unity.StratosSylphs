using CsvHelper.TypeConversion;
using System;

namespace HitPoint6.Unity.StratosSylphs.Utils.CSV
{
	public class DirectionConverter : ITypeConverter
	{
		private const string Inside = "内向き";
		private const string Outside = "外向き";

		public bool CanConvertFrom (Type type)
		{
			return type == typeof (string);
		}

		public bool CanConvertTo (Type type)
		{
			return type == typeof (bool);
		}

		public object ConvertFromString (TypeConverterOptions options, string text)
		{
			if (Inside == text)
			{
				return new bool? (true);
			}
			else if (Outside == text)
			{
				return new bool? (false);
			}
			else
			{
				return null;
			}
		}

		public string ConvertToString (TypeConverterOptions options, object value)
		{
			bool? v = (bool?)value;
			if (!v.HasValue)
			{
				return string.Empty;
			}
			if (v.Value)
			{
				return Inside;
			}
			else
			{
				return Outside;
			}
		}
	}
}