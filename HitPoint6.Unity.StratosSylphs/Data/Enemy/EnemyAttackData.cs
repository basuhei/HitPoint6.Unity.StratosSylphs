using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	public enum AttackType
	{
		nWayShot,
		nWayShotTargetPlayer,
		nWayShotRotation,
		Laser,
		LaserTargetPlayer,
		LaserRotation,
		EnemyPop
	}

	[Serializable]
	public class EnemyAttackData
	{
		[SerializeField]
		private AttackType _Type;

		[SerializeField]
		private int _WayCount;

		[SerializeField]
		private int _LaserWidth;

		[SerializeField]
		private float _OriginAngle;

		[SerializeField]
		private float _GapAngle;

		[SerializeField]
		private bool _TargetPlayer;

		[SerializeField]
		private int _LinkageMoveBehaviour;

		[SerializeField]
		private float _DelaySec;

		[SerializeField]
		private bool _StopWithMoveBehaviour;

		[SerializeField]
		private float _Duration;

		[SerializeField]
		private float _ShotInterval;

		//TODO:回転するかどうか追加する
		[SerializeField]
		private AnimationCurve _RotationSpeed;

		[SerializeField]
		private float _LaserDuration;

		public AttackType Type { get { return _Type; } }

		public int WayCount { get { return _WayCount; } }

		public int LaserWidth { get { return _LaserWidth; } }

		public float OriginAngle { get { return _OriginAngle; } }

		public float GapAngle { get { return _GapAngle; } }

		public float DelaySec { get { return _DelaySec; } }

		public bool TargetPlayer { get { return _TargetPlayer; } }

		public float Duration { get { return _Duration; } }

		public float LaserDuration { get { return _LaserDuration; } }

		public float ShotInterval { get { return _ShotInterval; } }

		public bool StopWithMoveBehaviour { get { return _StopWithMoveBehaviour; } }

		public int LinkageMoveBehaviour { get { return _LinkageMoveBehaviour; } }

		public AnimationCurve RotationSpeed { get { return _RotationSpeed; } }
	}
}