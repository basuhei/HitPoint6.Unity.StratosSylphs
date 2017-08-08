using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Data
{
	[CreateAssetMenu (menuName = "TalkEvent/SpritePack")]
	public class TalkEventSpriteData : ScriptableObject
	{
		[SerializeField, Header ("普通")]
		private Sprite _Normal;

		[SerializeField, Header ("キリッ")]
		private Sprite _Seriously;

		[SerializeField, Header ("困った")]
		private Sprite _Trouble;

		[SerializeField, Header ("ニヤッ")]
		private Sprite _Grin;

		[SerializeField, Header ("ほほえみ")]
		private Sprite _Smile;

		[SerializeField, Header ("驚き")]
		private Sprite _Surprise;

		[SerializeField, Header ("敬礼")]
		private Sprite _Salute;

		[SerializeField]
		private Sprite _Body;

		/// <summary>
		/// 普通顔
		/// </summary>
		public Sprite Normal { get { return _Normal; } }

		/// <summary>
		/// キリッ
		/// </summary>
		public Sprite Seriously { get { return _Seriously; } }

		/// <summary>
		/// 困った
		/// </summary>
		public Sprite Trouble { get { return _Trouble; } }

		/// <summary>
		/// ニヤッ
		/// </summary>
		public Sprite Grin { get { return _Grin; } }

		/// <summary>
		/// ほほえみ
		/// </summary>
		public Sprite Smile { get { return _Smile; } }

		/// <summary>
		/// 驚き
		/// </summary>
		public Sprite Surprise { get { return _Surprise; } }

		/// <summary>
		/// 敬礼
		/// </summary>
		public Sprite Salute { get { return _Salute; } }

		public Sprite Body { get { return _Body; } }
	}
}