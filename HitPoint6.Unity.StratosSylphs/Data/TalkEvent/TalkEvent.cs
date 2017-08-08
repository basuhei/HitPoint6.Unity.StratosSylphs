using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Data
{
	using Utils.CSV;

	[Serializable]
	public class TalkEvent
	{
		[SerializeField]
		private TextAsset _Csv;

		private TalkMessage[] _Cash = null;

		public TalkMessage[] Message
		{
			get
			{
				if (_Cash == null)
				{
					_Cash = TalkMessageReader.GetTalkData (_Csv);
				}
				return _Cash;
			}
		}
	}
}