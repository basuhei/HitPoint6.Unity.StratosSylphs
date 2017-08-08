using CsvHelper.TypeConversion;
using System;

namespace HitPoint6.Unity.StratosSylphs.Utils.CSV
{
	public class StandSideConverter : ITypeConverter
	{
		private const string Left = "左";
		private const string Right = "右";

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
			switch (text)
			{
				case Left:
					return new bool? (true);

				case Right:
					return new bool? (false);

				default:
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
				return Left;
			}
			else
			{
				return Right;
			}
		}
	}
}