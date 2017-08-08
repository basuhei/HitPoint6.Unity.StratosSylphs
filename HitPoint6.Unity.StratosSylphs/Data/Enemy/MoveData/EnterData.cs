using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Enter")]
	public class EnterData : EnemyMoveDataBase
	{
		[SerializeField]
		private float _Speed;

		[SerializeField]
		private AnimationCurve _Easing;

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}

		public float Speed
		{
			get { return _Speed; }
		}

		public override MoveType Type
		{
			get
			{
				return MoveType.Enter;
			}
		}
	}
}