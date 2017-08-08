using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/AttackData/LaserRotation")]
	public class LaserRotationData : LaserDataBase
	{
		[SerializeField]
		private float _OriginAngle;

		[SerializeField]
		private AnimationCurve _RotationSpeed;

		public AnimationCurve RotationSpeed
		{
			get { return _RotationSpeed; }
		}

		public float OriginAngle
		{
			get { return _OriginAngle; }
		}

		public override AttackType AttackType
		{
			get
			{
				return AttackType.LaserRotation;
			}
		}
	}
}