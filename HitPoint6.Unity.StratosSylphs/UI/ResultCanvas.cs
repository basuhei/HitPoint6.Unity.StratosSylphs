using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class ResultCanvas : MonoBehaviour
	{
		private void Awake ()
		{
		}

		private void Start ()
		{
			if (GameManager.Player != null)
			{
				GameManager.Player.Controller.LifeController
				.DeadAsObservable
				.Subscribe (__ => gameObject.SetActive (true));
			}
			else
			{
				GameManager.Instance.OnRegisterPlayerAsObservable ()
					.Subscribe (_ =>
					 {
						 GameManager.Player.Controller.LifeController
						 .DeadAsObservable
						 .Subscribe (__ => gameObject.SetActive (true));
					 });
			}
			gameObject.SetActive (false);
		}
	}
}