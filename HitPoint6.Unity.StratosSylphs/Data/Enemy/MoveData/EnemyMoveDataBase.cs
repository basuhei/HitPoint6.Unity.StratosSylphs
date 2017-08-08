using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	public abstract class EnemyMoveDataBase : ScriptableObject
	{
		[SerializeField]
		private float _Duration;

		[SerializeField]
		private SwingData _SwingData;

		public abstract MoveType Type
		{
			get;
		}

		public float Duration
		{
			get
			{
				return _Duration;
			}
		}

		public SwingData SwingData
		{
			get { return _SwingData; }
		}
	}
}