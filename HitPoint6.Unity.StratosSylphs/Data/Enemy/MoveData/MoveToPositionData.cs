using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using Library.Method;

	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/MoveToPosition")]
	public class MoveToPositionData : EnemyMoveDataBase
	{
		[SerializeField, Header ("ワールド座標上での移動先")]
		private Vector2 _TargetPosition;

		[SerializeField, Header ("正規化された距離によるイージング")]
		private AnimationCurve _Easing;

		public Vector2 TargetPosition
		{
			get { return _TargetPosition; }
		}

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}

		public override MoveType Type
		{
			get
			{
				return MoveType.MoveToPoint;
			}
		}

		private void OnValidate ()
		{
			_Easing = _Easing.Clamp01KyeTime ();
		}
	}
}