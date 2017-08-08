using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/nWayShotTargetPlayer")]
	public class nWayShotTargetPlayerData : nWayShotDataBase
	{
		public override AttackType AttackType
		{
			get
			{
				return AttackType.nWayShotTargetPlayer;
			}
		}
	}
}