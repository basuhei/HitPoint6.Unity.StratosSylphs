using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class PlaySEOnButtonDown : MonoBehaviour
	{
		[SerializeField]
		private AudioClip _Se;

		private void Awake ()
		{
			GetComponent<Button> ()
				.OnClickAsObservable ()
				.Subscribe (_ => AudioManager.SoundEmitter.PlaySE (_Se));
		}
	}
}