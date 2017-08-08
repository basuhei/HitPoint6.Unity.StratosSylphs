using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[Serializable]
	public class RandomRange
	{
		[SerializeField]
		private float _LowX;

		[SerializeField]
		private float _HighX;

		[SerializeField]
		private float _LowY;

		[SerializeField]
		private float _HighY;

		[SerializeField]
		private float _LowSpeed;

		[SerializeField]
		private float _HighSpeed;

		public float LowSpeed { get { return _LowSpeed; } }

		public float HighSpeed { get { return _HighSpeed; } }

		public float LowX { get { return _LowX; } }

		public float HighX { get { return _HighX; } }

		public float LowY { get { return _LowY; } }

		public float HighY { get { return _HighY; } }
	}

	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/RandomLinear")]
	public class RandomLinearData : EnemyMoveDataBase
	{
		[SerializeField]
		private AnimationCurve _Easing;

		[SerializeField]
		private RandomRange _Range;

		public override MoveType Type
		{
			get
			{
				return MoveType.RandomLinear;
			}
		}

		public RandomRange Range { get { return _Range; } }

		public AnimationCurve Easing { get { return _Easing; } }
	}
}