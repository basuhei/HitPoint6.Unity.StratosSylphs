using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using Managers;

	public class Boss : MonoBehaviour
	{
		[SerializeField]
		private AudioClip _BGM;

		[SerializeField]
		private AudioClip _DestroySound;

		private void Start ()
		{
			if (_BGM != null) AudioManager.Instance.ChangeMusic (_BGM);

			gameObject.OnDestroyAsObservable ()
				.Subscribe (_ =>
				{
					if (_IsQuit) { return; }

					AudioManager.SoundEmitter.PlaySE (_DestroySound);
					AudioManager.Instance.PlayGameClear ();
				});
		}

		private bool _IsQuit = false;

		private void OnApplicationQuit ()
		{
			_IsQuit = true;
		}

		private IEnumerator _SpriteFlashing (SpriteRenderer[] sprites)
		{
			var time = Time.time;
			while (Time.time - time < 0.8f)
			{
				if (Time.frameCount % 5 == 0)
				{
					foreach (var s in sprites)
					{
						s.enabled = !s.enabled;
					}
				}
				yield return null;
			}
			foreach (var s in sprites)
			{
				s.enabled = true;
			}
		}
	}
}