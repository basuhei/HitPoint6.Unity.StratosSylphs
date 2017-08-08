using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI.OptionScene
{
	using Scene;

	public class BuckToTitleButtonInOnlyOptionScene : MonoBehaviour
	{
		[SerializeField]
		private Button _Button;

		[SerializeField]
		private VolumeChangeObserver _VolumeChangeChecker;

		private void Start ()
		{
			_Button.OnClickAsObservable ()
				.Where (_ => _VolumeChangeChecker.DirtyFlag)
				.Subscribe (_ => Debug.Log ("確認ダイアログを出そう"));

			_Button.OnClickAsObservable ()
				.Where (_ => !_VolumeChangeChecker.DirtyFlag)
				.Subscribe (_ => SceneChanger.SceneChange (SceneType.Title));
		}
	}
}