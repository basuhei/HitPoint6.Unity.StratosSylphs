using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/Laser")]
	public class LaserData : LaserDataBase
	{
		[SerializeField]
		private float _OriginAngle;

		public override AttackType AttackType
		{
			get
			{
				return AttackType.Laser;
			}
		}

		public float OriginAngle
		{
			get { return _OriginAngle; }
		}
	}
}