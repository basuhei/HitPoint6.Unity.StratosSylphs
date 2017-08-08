using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using Library.Method;

	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Liniear")]
	public class LinearMoveData : EnemyMoveDataBase
	{
		[SerializeField, Header ("秒数毎のスピード")]
		private AnimationCurve _Easing;

		[SerializeField]
		private Vector2 _Direction;

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}

		public Vector2 Direction
		{
			get { return _Direction; }
		}

		public override MoveType Type
		{
			get
			{
				return MoveType.Linear;
			}
		}

		private void OnValidate ()
		{
			_Easing = _Easing.Clamp01KyeTime ();
		}
	}
}