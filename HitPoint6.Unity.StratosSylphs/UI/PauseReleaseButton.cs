using HitPoint6.Unity.StratosSylphs.Managers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	public class PauseReleaseButton : MonoBehaviour
	{
		[SerializeField]
		private StageManager _Manager;

		[SerializeField]
		private Button _Button;

		private void Start ()
		{
			_Button.OnClickAsObservable ()
				.Subscribe (_ => _Manager.PauseRelease ());
		}
	}
}