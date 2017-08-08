using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/LaserTargetPlayer")]
	public class LaserTargetPlayerData : LaserDataBase
	{
		public override AttackType AttackType
		{
			get
			{
				return AttackType.LaserTargetPlayer;
			}
		}
	}
}