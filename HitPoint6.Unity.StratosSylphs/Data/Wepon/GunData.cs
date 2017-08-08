using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu]
	public class GunData : ScriptableObject
	{
		[SerializeField, Tooltip ("一秒間に撃てる弾の数")]
		private float _ShotPerSecond;

		[SerializeField]
		private uint _ReminingBulletCount;

		[SerializeField]
		private float _ReloadTime;

		[SerializeField]
		private float _ReloadTimeAuto;

		[SerializeField]
		private GameObject _Shootable;

		public float ShotPerSecond
		{
			get { return _ShotPerSecond; }
		}

		public uint ReminingBulletCount
		{
			get { return _ReminingBulletCount; }
		}

		public GameObject Shootable
		{
			get { return _Shootable; }
		}

		public float ReloadTime
		{
			get
			{
				return _ReloadTime;
			}
		}

		public float ReloadTimeAuto
		{
			get
			{
				return _ReloadTimeAuto;
			}
		}
	}
}