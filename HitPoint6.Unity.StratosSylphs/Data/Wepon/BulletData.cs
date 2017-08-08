using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu]
	public class BulletData : ScriptableObject
	{
		[SerializeField]
		private int _Damage;

		[SerializeField, Tooltip ("生存時間(秒単位、小数点も使えるよ)")]
		private float _LifeTime;

		[SerializeField]
		private float _Speed;

		public int Damage { get { return _Damage; } }

		public float LifeTime { get { return _LifeTime; } }

		public float Speed { get { return _Speed; } }
	}
}