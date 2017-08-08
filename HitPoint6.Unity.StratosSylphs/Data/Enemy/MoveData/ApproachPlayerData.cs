using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using Library.Method;

	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Approach")]
	public class ApproachPlayerData : EnemyMoveDataBase
	{
		[SerializeField, Header ("どれくらい近づくか")]
		private float _ApproachDistance;

		[SerializeField, Header ("秒数毎のスピード")]
		private AnimationCurve _Easing;

		public float AproachDistance
		{
			get
			{
				return _ApproachDistance;
			}
		}

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}

		public override MoveType Type
		{
			get
			{
				return MoveType.ApproachPlayer;
			}
		}

		private void OnValidate ()
		{
			_Easing = _Easing.Clamp01KyeTime ();
		}
	}
}