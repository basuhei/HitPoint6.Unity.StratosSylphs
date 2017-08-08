using System.Collections.Generic;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/EnemyActionData")]
	public class EnemyActionData : ScriptableObject
	{
		[SerializeField]
		private string _AnimationStateMessage;

		[SerializeField]
		private EnemyMoveDataBase[] _MoveData;

		[SerializeField]
		private EnemyAttackDataBase[] _AttackData;

		public IEnumerable<EnemyMoveDataBase> MoveData
		{
			get { return _MoveData; }
		}

		public IEnumerable<EnemyAttackDataBase> AttackData
		{
			get { return _AttackData; }
		}

		public string AnimationStateMessage
		{
			get { return _AnimationStateMessage; }
		}

		private void OnValidate ()
		{
			for (int i = 0; i < _MoveData.Length; i++)
			{
				if (_MoveData[i] == null)
				{
					Debug.LogAssertion (this.name + "の、" + i + "番目のMoveDataが空です");
				}
			}
			for (int i = 0; i < _AttackData.Length; i++)
			{
				if (_AttackData[i] == null)
				{
					Debug.LogAssertion (this.name + "の、" + i + "番目のAttackDataが空です");
				}
			}
		}
	}
}