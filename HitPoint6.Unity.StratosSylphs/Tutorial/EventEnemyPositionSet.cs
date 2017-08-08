using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using GameUnits;

	[Serializable]
	public class EventEnemyPositionSet
	{
		[SerializeField]
		private Enemy _Enemy;

		[SerializeField]
		private Vector2 _PopPoint;

		public Enemy Enemy { get { return _Enemy; } }

		public Vector2 PopPoint { get { return _PopPoint; } }
	}
}