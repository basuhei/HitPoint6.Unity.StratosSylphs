using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using GameUnits;

	public abstract class LaserDataBase : EnemyAttackDataBase
	{
		[SerializeField]
		private Laser _Laser;

		[SerializeField, Header ("何回撃つか")]
		private uint _ShotCount;

		public Laser Laser
		{
			get { return _Laser; }
		}

		public uint ShotCount
		{
			get { return _ShotCount; }
		}
	}
}