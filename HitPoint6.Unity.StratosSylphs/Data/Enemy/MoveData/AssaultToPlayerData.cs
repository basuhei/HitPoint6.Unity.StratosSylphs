using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using Library.Method;

	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Assault")]
	public class AssaultToPlayerData : EnemyMoveDataBase
	{
		[SerializeField, Header ("秒数毎のスピードを設定")]
		private AnimationCurve _Easing;

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}

		public override MoveType Type
		{
			get
			{
				return MoveType.AssaultToPlayer;
			}
		}

		private void OnValidate ()
		{
			_Easing = _Easing.Clamp01KyeTime ();
		}
	}
}