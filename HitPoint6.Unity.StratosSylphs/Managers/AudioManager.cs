using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using Audio;
	using Library.CustomizedMonoBehavior;
	using Scene;

	[DisallowMultipleComponent]
	public class AudioManager : SingletonMonoBehaviour<AudioManager>
	{
		[SerializeField]
		private PlayerSound _PlayerSound;

		[SerializeField]
		private EnemySound _EnemySound;

		[SerializeField]
		private BGM _BGM;

		[SerializeField]
		private AudioMixer _AudioMixer;

		[SerializeField]
		private float _MusicFadeTime = 5f;

		private AudioSource _AudioSource;

		private AudioClip _PreviousBGM;

		public static PlayerSound PlayerSound
		{
			get; private set;
		}

		public static EnemySound EnemySound
		{
			get; private set;
		}

		public static BGM BGM
		{
			get; private set;
		}

		public static SoundEmitter SoundEmitter
		{
			get; set;
		}

		public static AudioMixer AudioMixer
		{
			get; private set;
		}

		protected override void Awake ()
		{
			base.Awake ();
			NullCheck ();

			AudioMixer = _AudioMixer;
			PlayerSound = _PlayerSound;
			EnemySound = _EnemySound;
			_AudioSource = GetComponent<AudioSource> ();
			BGM = _BGM;

			GameManager.Instance.OnRegisterPlayerAsObservable ()
						.DelayFrame (10)
						.Subscribe (player =>
						{
							player.Controller.LifeController.DeadAsObservable
										.First ()
										.Subscribe (_ =>
							 {
								 Debug.Log ("ゲームオーバー時のBGM切り替え処理");
								 _AudioSource.Stop ();
								 _AudioSource.loop = false;
								 _AudioSource.clip = _BGM.GameOver;
								 _AudioSource.Play ();
							 });
						});
			Observable.NextFrame ()
				.Subscribe (_ =>
				 {
					 if (GameManager.Player != null) GameManager.Player.Controller.LifeController
					   .DeadAsObservable
					   .First ()
					   .Subscribe (__ =>
						{
							_AudioSource.Stop ();
							_AudioSource.loop = false;
							_AudioSource.clip = _BGM.GameOver;
							_AudioSource.Play ();
						});
				 });
		}

		private void Start ()
		{
			_AudioSource.clip = _BGM.Title;
			_AudioSource.loop = true;
			_AudioSource.Play ();
			/*GameSceneManager.Instance.SceneChangeAsObservable ()
				.Where (scene => scene == "Title")
				.Subscribe (_ =>
				{
					_AudioSource.Stop ();
					_AudioSource.loop = true;
					_AudioSource.clip = _BGM.Title;
					_AudioSource.Play();
				});

			GameSceneManager.Instance.SceneChangeAsObservable ()
				.Where (scene => scene == "TestStage1")
				.Subscribe (_ =>
				{
					_AudioSource.Stop ();
					_AudioSource.loop = true;
					_AudioSource.clip = _BGM.Stage;
					_AudioSource.Play ();
				});*/
			TransitionManager.SceneChangeComopleteAsObservable ().Subscribe (SwitchMusic);
		}

		public void PlayDefaultMusic ()
		{
			StartCoroutine (ChangeMusicCore (_PreviousBGM));
		}

		public void PlayMusic (AudioClip source)
		{
			_AudioSource.Stop ();
			_AudioSource.loop = true;
			_AudioSource.clip = source;
			_AudioSource.Play ();
		}

		public void ChangeMusic (AudioClip source)
		{
			_PreviousBGM = _AudioSource.clip;
			StartCoroutine (ChangeMusicCore (source));
		}

		private IEnumerator ChangeMusicCore (AudioClip source)
		{
			float volume = _AudioSource.volume;

			while (_AudioSource.volume > 0)
			{
				_AudioSource.volume -= 1 / _MusicFadeTime * Time.deltaTime;
				yield return null;
			}
			_AudioSource.volume = volume;
			_AudioSource.Stop ();
			_AudioSource.loop = true;
			if (source == null)
			{
				yield break;
			}
			_AudioSource.clip = source;
			_AudioSource.Play ();
		}

		private void SwitchMusic (SceneType scene)
		{
			switch (scene)
			{
				case SceneType.Title:
					PlayMusic (_BGM.Title);
					break;

				case SceneType.Option:

					//TODO:オプションのBGMを追加
					break;

				case SceneType.StageOne:
					PlayMusic (_BGM.Stage);
					break;

				case SceneType.StageTwo:
					break;

				case SceneType.Result:
					break;

				case SceneType.Tutorial:
					PlayMusic (_BGM.Tutorial);
					break;

				default:
					break;
			}
		}

		public void PlayGameClear ()
		{
			_AudioSource.Stop ();
			_AudioSource.loop = false;
			_AudioSource.clip = _BGM.Clear;
			_AudioSource.Play ();
		}

		[System.Diagnostics.Conditional ("UNITY_EDITOR"),
		System.Diagnostics.Conditional ("DEVELOPMENT_BUILD")]
		private void NullCheck ()
		{
			if (_AudioMixer == null)
			{
				TimeManager.TimeScale = 0.0f;
				throw new InvalidOperationException ("ミキサーがアタッチされていません");
			}
		}
	}
}