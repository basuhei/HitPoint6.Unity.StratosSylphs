using System.Linq;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/nWayShotRotation")]
	public class nWayShotRotationData : nWayShotData
	{
		[SerializeField]
		private AnimationCurve _RotationSpeed;

		public override AttackType AttackType
		{
			get
			{
				return AttackType.nWayShotRotation;
			}
		}

		public AnimationCurve RotationSpeed
		{
			get { return _RotationSpeed; }
		}

		private void OnValidate ()
		{
			_RotationSpeed.MoveKey (0, new Keyframe (0, _RotationSpeed.keys[0].value));
			_RotationSpeed.MoveKey (_RotationSpeed.keys.Count () - 1, new Keyframe (Duration, _RotationSpeed.keys[_RotationSpeed.keys.Count () - 1].value));
		}
	}
}