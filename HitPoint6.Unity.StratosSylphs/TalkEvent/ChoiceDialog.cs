using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.TalkEvent
{
	public class ChoiceDialog : MonoBehaviour
	{
		[SerializeField]
		private Button _OKButton;

		[SerializeField]
		private Button _CancelButton;

		private void Awake ()
		{
			_OKButton.gameObject.SetActive (false);
			_CancelButton.gameObject.SetActive (false);
		}

		public IEnumerator DialogPop ()
		{
			_OKButton.gameObject.SetActive (true);
			_CancelButton.gameObject.SetActive (true);
			yield return _OKButton.OnClickAsObservable ()
				.Select (_ => true)
				.Merge (_CancelButton.OnClickAsObservable ().Select (_ => false))
				.First ()
				.StartAsCoroutine (result => Result = result);
			_OKButton.gameObject.SetActive (false);
			_CancelButton.gameObject.SetActive (false);
		}

		public bool Result { get; private set; }
	}
}