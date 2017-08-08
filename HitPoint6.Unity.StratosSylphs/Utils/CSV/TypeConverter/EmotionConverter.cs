using CsvHelper.TypeConversion;
using System;

namespace HitPoint6.Unity.StratosSylphs.Utils.CSV
{
	using Data;

	public class EmotionConverter : ITypeConverter
	{
		private const string Normal = "普通";
		private const string Seriously = "キリッ";
		private const string Trouble = "困った";
		private const string Grin = "ニヤッ";
		private const string Smile = "ほほえみ";
		private const string Surprise = "驚き";
		private const string Salute = "敬礼";

		public bool CanConvertFrom (Type type)
		{
			return type == typeof (string);
		}

		public bool CanConvertTo (Type type)
		{
			return typeof (Emotion) == type;
		}

		public object ConvertFromString (TypeConverterOptions options, string text)
		{
			switch (text)
			{
				case Normal:
					return Emotion.Normal;

				case Seriously:
					return Emotion.Seriously;

				case Trouble:
					return Emotion.Trouble;

				case Grin:
					return Emotion.Grin;

				case Smile:
					return Emotion.Smile;

				case Surprise:
					return Emotion.Surprise;

				case Salute:
					return Emotion.Salute;

				default:
					return Emotion.None;
			}
		}

		public string ConvertToString (TypeConverterOptions options, object value)
		{
			switch ((Emotion)value)
			{
				case Emotion.None:
					return string.Empty;

				case Emotion.Normal:
					return Normal;

				case Emotion.Seriously:
					return Seriously;

				case Emotion.Trouble:
					return Trouble;

				case Emotion.Grin:
					return Grin;

				case Emotion.Smile:
					return Smile;

				case Emotion.Surprise:
					return Surprise;

				case Emotion.Salute:
					return Salute;

				default:
					return string.Empty;
			}
		}
	}
}