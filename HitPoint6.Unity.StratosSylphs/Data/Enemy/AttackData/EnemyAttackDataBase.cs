using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	public abstract class EnemyAttackDataBase : ScriptableObject
	{
		[SerializeField]
		private float _ShotInterval;

		[SerializeField, Header ("開始時間")]
		private float _StartTime;

		public abstract AttackType AttackType
		{
			get;
		}

		public float ShotInterval
		{
			get { return _ShotInterval; }
		}

		public float StartTime { get { return _StartTime; } }
	}
}