using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Audio
{
	using Managers;

	[DisallowMultipleComponent]
	[RequireComponent (typeof (AudioSource))]
	public class SoundEmitter : MonoBehaviour
	{
		private AudioSource _AudioSource;

		public void Awake ()
		{
			AudioManager.SoundEmitter = this;
			_AudioSource = GetComponent<AudioSource> ();
		}

		public void PlaySE (AudioClip clip)
		{
			if (clip == null) { return; }
			_AudioSource.PlayOneShot (clip);
		}
	}
}