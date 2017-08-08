using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using Effect;
	using Library.CustomizedMonoBehavior;
	using Scene;

	public class TransitionManager : SingletonMonoBehaviour<TransitionManager>
	{
		private static readonly string _FadeAssetPath = "Effect/FadeCanvas";

		[SerializeField]
		private string _Title;

		[SerializeField]
		private string _Option;

		[SerializeField]
		private string _StageOne;

		[SerializeField]
		private string _StageTwo;

		[SerializeField]
		private string _Result;

		[SerializeField]
		private string _Tutorial;

		private bool _Transitioning = false;

		private static Subject<SceneType> SceneChangeComopleteObserver = new Subject<SceneType> ();

		public static IObservable<SceneType> SceneChangeComopleteAsObservable ()
		{
			return SceneChangeComopleteObserver;
		}

		public void TranstionStart (SceneType scene)
		{
			if (_Transitioning)
			{
				return;
			}
			switch (scene)
			{
				case SceneType.Title:
					_SceneChangeCore (_Title, SceneType.Title);
					break;

				case SceneType.Option:
					_SceneChangeCore (_Option, SceneType.Option);
					break;

				case SceneType.StageOne:
					_SceneChangeCore (_StageOne, SceneType.StageOne);
					break;

				case SceneType.StageTwo:
					_SceneChangeCore (_StageTwo, SceneType.StageTwo);
					break;

				case SceneType.Result:
					_SceneChangeCore (_Result, SceneType.Result);
					break;

				case SceneType.Tutorial:
					_SceneChangeCore (_Tutorial, SceneType.Tutorial);
					break;

				case SceneType.Exit:
					Application.Quit ();
					break;

				default:
					break;
			}
		}

		private void _SceneChangeCore (string nextScene, SceneType scene)
		{
			if (_Transitioning) { return; }
			_Transitioning = true;
			if (FadeInOut.Instance == null)
			{
				var resouce = Resources.Load<FadeInOut> (_FadeAssetPath);
				Instantiate (resouce);
			}
			FadeInOut.Instance.OnFadeCompleteAsObservable ()
				.First ()
				.Subscribe (_ =>
				 {
					 SceneManager.LoadScene (nextScene.Split ('.').First ().Split ('/').Last ());
					 SceneChangeComopleteObserver.OnNext (scene);
					 FadeInOut.Instance.FadeIn (1f);
					 FadeInOut.Instance.OnFadeCompleteAsObservable ()
					 .First ()
					 .Subscribe (__ => _Transitioning = false);
				 });
			FadeInOut.Instance.FadeOut (1f);
		}
	}
}