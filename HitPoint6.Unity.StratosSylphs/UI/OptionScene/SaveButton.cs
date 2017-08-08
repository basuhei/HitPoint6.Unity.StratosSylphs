using HitPoint6.Unity.StratosSylphs.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI.OptionScene
{
	public class SaveButton : MonoBehaviour
	{
		[SerializeField]
		private Button _SaveButton;

		private void Awake ()
		{
			_SaveButton.OnClickAsObservable ()
				.Subscribe (_ => AudioVolumeSave.Save ());
		}
	}
}