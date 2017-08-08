using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class GameClear : MonoBehaviour
	{
		private void Awake ()
		{
			UIManager.GameClear = this;
			/*this.UpdateAsObservable ()
				.Where (_ => gameObject.activeInHierarchy)
				.Where (_ => Input.anyKeyDown)
				.Subscribe (_ => GameSceneManager.Instance.SceneChange ("Title", 2f));*/
		}

		private void Start ()
		{
			/*GameSceneManager.Instance.SceneChangeAsObservable ()
				.Subscribe (_ => Debug.Log ("aaa"));
			GameSceneManager.Instance.SceneChangeAsObservable ()
				.Subscribe (_ =>
				{
					gameObject.SetActive (false);
				});*/
			gameObject.SetActive (false);
		}

		public void PopClearText ()
		{
			gameObject.SetActive (true);
		}
	}
}