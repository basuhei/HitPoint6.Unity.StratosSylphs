using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Managers;

	public class EnemySoundPresenter : MonoBehaviour
	{
		private void PlaySE (AudioClip clip)
		{
			AudioManager.SoundEmitter.PlaySE (clip);
		}

		private void PlayBGM (AudioClip clip)
		{
			AudioManager.Instance.ChangeMusic (clip);
		}
	}
}