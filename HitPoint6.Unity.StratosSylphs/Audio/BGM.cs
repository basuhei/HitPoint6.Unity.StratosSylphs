using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Audio
{
	[Serializable]
	public class BGM
	{
		[SerializeField]
		private AudioClip _Title;

		[SerializeField]
		private AudioClip _Stage;

		[SerializeField]
		private AudioClip _GameOver;

		[SerializeField]
		private AudioClip _Clear;

		[SerializeField]
		private AudioClip _Tutorial;

		public AudioClip Title { get { return _Title; } }

		public AudioClip Stage { get { return _Stage; } }

		public AudioClip GameOver { get { return _GameOver; } }

		public AudioClip Clear { get { return _Clear; } }

		public AudioClip Tutorial { get { return _Tutorial; } }
	}
}