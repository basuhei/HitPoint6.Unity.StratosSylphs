using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Audio
{
	[Serializable]
	public class EnemySound
	{
		[SerializeField]
		private AudioClip _DestroySound;

		[SerializeField]
		private AudioClip _BossDestroySound;

		public AudioClip BossDestroySound
		{
			get { return _BossDestroySound; }
		}

		public AudioClip DestroySound
		{
			get { return _DestroySound; }
		}
	}
}