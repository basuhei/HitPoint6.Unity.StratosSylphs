using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using GameUnits;

	public abstract class nWayShotDataBase : EnemyAttackDataBase
	{
		[SerializeField, Header ("動作時間")]
		private float _Duration;

		[SerializeField]
		private float _GapAngle;

		[SerializeField]
		private uint _WayCount;

		[SerializeField]
		private Bullet _Bullet;

		public float GapAngle
		{
			get { return _GapAngle; }
		}

		public uint WayCount
		{
			get { return _WayCount; }
		}

		public float Duration
		{
			get { return _Duration; }
		}

		public Bullet Bullet
		{
			get { return _Bullet; }
		}
	}
}