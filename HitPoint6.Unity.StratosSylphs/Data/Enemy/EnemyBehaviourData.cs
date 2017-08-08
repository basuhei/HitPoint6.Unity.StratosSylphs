using System.Collections.Generic;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	public class EnemyBehaviourData : ScriptableObject
	{
		[SerializeField]
		private bool _Loop;

		[SerializeField]
		private int _LoopIndex;

		[SerializeField]
		private EnemyAttackData[] _AttackBehaviour;

		[SerializeField]
		private EnemyMoveData[] _MoveBehaviour;

		public IEnumerable<EnemyAttackData> AttackBehaviour
		{
			get { return _AttackBehaviour; }
		}

		public IEnumerable<EnemyMoveData> MoveBehaviour
		{
			get { return _MoveBehaviour; }
		}

		public bool Loop
		{
			get { return _Loop; }
		}

		public int LoopIndex
		{
			get { return _LoopIndex; }
		}
	}
}