using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/nWayShot")]
	public class nWayShotData : nWayShotDataBase
	{
		public override AttackType AttackType
		{
			get
			{
				return AttackType.nWayShot;
			}
		}

		[SerializeField]
		private float _OriginAngle;

		public float OriginAngle
		{
			get { return _OriginAngle; }
		}
	}
}