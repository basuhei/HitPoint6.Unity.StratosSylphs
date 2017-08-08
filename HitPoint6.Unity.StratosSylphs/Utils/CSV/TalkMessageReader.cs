using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Utils.CSV
{
	using Data;

	public static class TalkMessageReader
	{
		private class TalkMapper : CsvClassMap<TalkMessage>
		{
			public TalkMapper ()
			{
				Map (t => t.Name).Index (0);
				Map (t => t.Face).Index (1).TypeConverter<EmotionConverter> ();
				Map (t => t.LeftSide).Index (2).TypeConverter<StandSideConverter> ();
				Map (t => t.IsInside).Index (3).TypeConverter<DirectionConverter> ();
				Map (t => t.Message).Index (4);
				Map (t => t.Other).Index (5);
			}
		}

		public static TalkMessage[] GetTalkData (TextAsset asset)
		{
			var text = asset.text;
			var reader = new StringReader (text);
			var csv = new CsvReader (reader);
			csv.Configuration.RegisterClassMap<TalkMapper> ();
			var record = csv.GetRecords<TalkMessage> ();
			return record.ToArray ();
		}

		public static TalkMessage[] GetTalkData (string assetPath)
		{
			return GetTalkData (Resources.Load<TextAsset> (assetPath));
		}
	}
}