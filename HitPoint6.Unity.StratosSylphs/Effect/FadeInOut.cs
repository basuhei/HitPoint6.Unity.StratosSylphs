using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	public enum Fade
	{
		In = 0,
		Out = 1
	}

	public class FadeInOut : MonoBehaviour
	{
		[SerializeField]
		private RawImage _Texture;
		private Subject<Fade> _FadeStartObserver = new Subject<Fade> ();
		private Subject<Fade> _FadeObserver = new Subject<Fade> ();
		private bool _IsFade = false;

		private static FadeInOut _Instance;

		public static FadeInOut Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<FadeInOut> ();
					if (_Instance == null)
					{
						Debug.LogWarning (typeof (FadeInOut));
					}
					return _Instance;
				}
				return _Instance;
			}
		}

		private void Awake ()
		{
			if (_Instance == null)
			{
				_Instance = this;
				DontDestroyOnLoad (this);
			}
			if (_Instance != this)
			{
				Destroy (gameObject);
			}
			if (_FadeObserver == null)
			{
				_FadeObserver = new Subject<Fade> ();
			}

			gameObject.SetActive (false);
		}

		public IObservable<Fade> OnFadeStartAsObservable()
		{
			return _FadeStartObserver;
		}

		public IObservable<Fade> OnFadeCompleteAsObservable ()
		{
			return _FadeObserver;
		}

		public void FadeIn (float fadeTime)
		{
			if (_IsFade) { return; }
			_FadeStartObserver.OnNext (Fade.In);
			gameObject.SetActive (true);
			StartCoroutine (_FadeCore (fadeTime, Fade.In));
		}

		public void FadeOut (float fadeTime)
		{
			if (_IsFade) { return; }
			_FadeStartObserver.OnNext (Fade.Out);
			gameObject.SetActive (true);
			StartCoroutine (_FadeCore (fadeTime, Fade.Out));
		}

		private IEnumerator _FadeCore (float fadeTime, Fade fade)
		{

			_IsFade = true;
			var startTime = Time.time;
			var currentColor = _Texture.color;
			var targetColor = new Color (currentColor.r, currentColor.g, currentColor.b, (int)fade);
			while (fadeTime > Time.time - startTime)
			{
				_Texture.color = Color.Lerp (currentColor, targetColor, (Time.time - startTime) / fadeTime);
				yield return null;
			}
			_IsFade = false;
			_FadeObserver.OnNext (fade);
			if (fade == Fade.In)
			{
				gameObject.SetActive (false);
			}
		}
	}
}