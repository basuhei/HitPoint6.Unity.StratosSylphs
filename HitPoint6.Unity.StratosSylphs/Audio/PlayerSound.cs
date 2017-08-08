using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Audio
{
	[Serializable]
	public class PlayerSound
	{
		[SerializeField]
		private AudioClip _ShotSound;

		[SerializeField]
		private AudioClip _MissileShotSound;

		[SerializeField]
		private AudioClip _MissileHitSound;

		[SerializeField]
		private AudioClip _DeadSound;

		[SerializeField]
		private AudioClip _ReloadSound;

		[SerializeField]
		private AudioClip _LockOnSound;

		[SerializeField]
		private AudioClip _DamageSound;

		public AudioClip ShotSound
		{
			get { return _ShotSound; }
		}

		public AudioClip MissileShotSound
		{
			get { return _MissileShotSound; }
		}

		public AudioClip MissileHitSound
		{
			get { return _MissileHitSound; }
		}

		public AudioClip DeadSound
		{
			get { return _DeadSound; }
		}

		public AudioClip ReloadSound
		{
			get { return _ReloadSound; }
		}

		public AudioClip LockOnSound
		{
			get { return _LockOnSound; }
		}

		public AudioClip DamageSound
		{
			get { return _DamageSound; }
		}
	}
}