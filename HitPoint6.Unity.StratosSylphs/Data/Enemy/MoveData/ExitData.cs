using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Exit")]
	public class ExitData : EnemyMoveDataBase
	{
		[SerializeField]
		private float _Speed;

		public override MoveType Type
		{
			get
			{
				return MoveType.Exit;
			}
		}

		public float Speed
		{
			get { return _Speed; }
		}
	}
}