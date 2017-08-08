using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Constants;
	using Managers;
	using Scene;

	public class SceneChangeButton : MonoBehaviour
	{
		[SerializeField]
		private Button _Button;

		[SerializeField]
		private SceneType _NextScene;

		[SerializeField]
		private bool _IsResultSecene = false;

		[SerializeField]
		private AudioClip _Clip;

		private void Awake ()
		{
			if (_NextScene == SceneType.StageOne && !_IsResultSecene)
			{
				_Button.OnClickAsObservable ().Merge (this.UpdateAsObservable ()
				.Where (_ =>  Input.GetButtonDown (ButtonName.Bomb)
							 | Input.GetButtonDown (ButtonName.ReLoad)))
					.Subscribe (_ =>
					{
						if (_Clip) AudioManager.SoundEmitter.PlaySE (_Clip);
						SceneChanger.SceneChange (_NextScene);
					});
			}
			else if (_IsResultSecene && _NextScene == SceneType.Title)
			{
				_Button.OnClickAsObservable ().Merge (this.UpdateAsObservable ()
						.Where (_ =>  Input.GetButtonDown (ButtonName.Bomb)
									 | Input.GetButtonDown (ButtonName.ReLoad)))
							.Subscribe (_ =>
							{
								if (_Clip) AudioManager.SoundEmitter.PlaySE (_Clip);
								SceneChanger.SceneChange (_NextScene);
							});
			}
			else
			{
				_Button.OnClickAsObservable ()
					.Subscribe (_ =>
					{
						if (_Clip) AudioManager.SoundEmitter.PlaySE (_Clip);
						SceneChanger.SceneChange (_NextScene);
					});
			}
		}
	}
}