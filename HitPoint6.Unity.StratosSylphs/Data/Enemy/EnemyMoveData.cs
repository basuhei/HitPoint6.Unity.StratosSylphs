using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[Serializable]
	public class EnemyMoveData
	{
		[SerializeField]
		private Vector2 _Direction;

		[SerializeField]
		private AnimationCurve _Easing;

		[SerializeField]
		private float _OperatingTime;

		[SerializeField]
		private MoveType _Type;

		[SerializeField]
		private float _ApproachDisntance;

		[SerializeField]
		private float _Speed;

		public Vector2 Direction { get { return _Direction; } }

		public AnimationCurve Easing { get { return _Easing; } }

		public float OparatingTime { get { return _OperatingTime; } }

		public MoveType Type { get { return _Type; } }

		public float ApproachDistance { get { return _ApproachDisntance; } }

		public float Speed { get { return _Speed; } }
	}
}