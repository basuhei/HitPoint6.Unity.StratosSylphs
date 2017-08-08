using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using GameUnits;

	[Serializable]
	public class TutorialEvent
	{
		[SerializeField]
		private Player _Player;

		[SerializeField]
		private TextAsset _Opning;

		[SerializeField]
		private TextAsset _Opening2;

		[SerializeField]
		private TextAsset _MoveTutorial;

		[SerializeField]
		private TextAsset _ShotTutorial;

		[SerializeField]
		private TextAsset _BombTutorial;

		[SerializeField]
		private TextAsset _Ending;

		[SerializeField]
		private TextAsset _Ending2;

		public TextAsset MoveTutorial { get { return _MoveTutorial; } }

		public TextAsset Opening { get { return _Opning; } }

		public TextAsset Opening2 { get { return _Opening2; } }

		public TextAsset ShotTutorial { get { return _ShotTutorial; } }

		public TextAsset BombTutorial { get { return _BombTutorial; } }

		public TextAsset Ending { get { return _Ending; } }

		public TextAsset Ending2 { get { return _Ending2; } }
	}
}