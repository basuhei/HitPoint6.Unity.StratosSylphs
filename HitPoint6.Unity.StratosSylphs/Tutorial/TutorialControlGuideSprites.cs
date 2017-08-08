using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	[Serializable]
	public class GuideSpritesWithPosition
	{
		[SerializeField]
		private Sprite _Sprite;

		[SerializeField]
		private Vector2 _Position;

		public Sprite Sprite { get { return _Sprite; } }

		public Vector2 Position { get { return _Position; } }
	}

	[Serializable]
	public class TutorialControlGuideSprites
	{
		[SerializeField]
		private GuideSpritesWithPosition _Move;

		[SerializeField]
		private GuideSpritesWithPosition _Shot;

		[SerializeField]
		private GuideSpritesWithPosition _Bomb;

		public GuideSpritesWithPosition Move { get { return _Move; } }

		public GuideSpritesWithPosition Shot { get { return _Shot; } }

		public GuideSpritesWithPosition Bomb { get { return _Bomb; } }
	}
}