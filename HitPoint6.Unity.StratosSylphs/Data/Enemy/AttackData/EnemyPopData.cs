using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using GameUnits;

	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/EnemyPop")]
	public class EnemyPopData : EnemyAttackDataBase
	{
		[SerializeField]
		private Enemy _PopEnemy;

		[SerializeField]
		private int _PopPoint;

		[SerializeField]
		private bool _Infinit;

		[SerializeField]
		private int _Count;

		[SerializeField]
		private float _Duration;

		[SerializeField]
		private float _SpeedRatio = 1f;

		public Enemy PopEnemy
		{
			get { return _PopEnemy; }
		}

		public int PopPoint
		{
			get { return _PopPoint; }
		}

		public bool Infinit
		{
			get { return _Infinit; }
		}

		public override AttackType AttackType
		{
			get
			{
				return AttackType.EnemyPop;
			}
		}

		public int Count { get { return _Count; } }

		public float SpeedRatio
		{
			get { return _SpeedRatio; }
		}

		public float Duration
		{
			get { return _Duration; }
		}
	}
}