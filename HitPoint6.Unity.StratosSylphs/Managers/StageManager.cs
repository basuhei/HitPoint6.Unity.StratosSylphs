using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using Constants;
	using Data;
	using Effect;
	using Scene;
	using TalkEvent;
	using UI;

	public enum StageState
	{
		Pause,
		Event,
		Play,
		Talk,
		GameOver
	}

	public class StageManager : MonoBehaviour
	{
		private ReactiveProperty<StageState> _State = new ReactiveProperty<StageState> (StageState.Play);

		private float _StageDeltaTime;

		[SerializeField]
		private EnemySpawnerManager[] _EnemySpawner;

		[SerializeField]
		private Score _Score;

		[SerializeField]
		private AudioClip _PauseSound;

		[SerializeField]
		private TalkEventSystem _TalkSystem;

		[SerializeField]
		private TalkEvent[] _LastTalkAssets;

		[SerializeField]
		private TalkEvent _EscapeTalkAsset;

		[SerializeField]
		private TalkEvent _ClearTalkAsset;

		[SerializeField]
		private Canvas _ClearGraphic;

		public ReadOnlyReactiveProperty<StageState> State
		{
			get;
			private set;
		}

		private void Awake ()
		{
			State = _State.ToReadOnlyReactiveProperty ();

			_State.Value = StageState.Play;
		}

		private void Start ()
		{
			_TalkSystem.EventStartAsObservable ().Subscribe (_ => _State.Value = StageState.Talk);

			//TODO:ゲームステージの終了検知を入れないとだめかも？
			_TalkSystem.EventDoneAsObservable ()
				.Where (data => data.Message != _EscapeTalkAsset.Message.Last ().Message
								&& data.Message != _ClearTalkAsset.Message.Last ().Message)
				.Subscribe (_ =>
				{
					EnemySpawnerManager.StartProcess ();
					_State.Value = StageState.Play;
					TimeManager.EnemyTimeScale = 1.0f;
					TimeManager.TimeScale = 1.0f;
				})
				.AddTo (this);

			_TalkSystem.EventDoneAsObservable ()
				.First (data => data.Message == _ClearTalkAsset.Message.Last ().Message)
				.Subscribe (_ =>
				 {
					 FadeInOut.Instance.OnFadeCompleteAsObservable ()
						.First (fade => fade == Fade.Out)
						.Subscribe (__ =>
						{
							AudioManager.Instance.ChangeMusic (null);
							_ClearGraphic.gameObject.SetActive (true);
							FadeInOut.Instance.OnFadeCompleteAsObservable ()
							.First (fade => fade == Fade.In)
							.Subscribe (r =>
							 {
								 SceneOverValueHolder.ScoreValue = _Score.ScoreValue.Value;
								 this.UpdateAsObservable ()
								 .First (___ => Input.GetButtonDown (Constants.ButtonName.Shot))
								 .Subscribe (rr => SceneChanger.SceneChange (SceneType.Result));
							 });
							FadeInOut.Instance.FadeIn (1f);
						});
					 FadeInOut.Instance.FadeOut (1f);
				 }).AddTo (this);

			_TalkSystem.EventDoneAsObservable ()
				.Where (data => data.Message == _EscapeTalkAsset.Message.Last ().Message)
				.First ()
				.Subscribe (_ =>
				{
					SceneOverValueHolder.ScoreValue = _Score.ScoreValue.Value;
					SceneOverValueHolder.StageClearTime = _StageDeltaTime;
					SceneChanger.SceneChange (SceneType.Result);
				}).AddTo (this);

			//ポーズ処理
			_State.Where (state => state == StageState.Play)
				.Subscribe (_ =>
				{
					GameManager.Player.CanControl = true;

					this.UpdateAsObservable ()
					.Where (__ => _State.Value == StageState.Play)
					.Where (__ => Input.GetButtonDown (ButtonName.Pause))
					.First ()
					.Subscribe (__ =>
					{
						TimeManager.Pause ();
						_State.Value = StageState.Pause;
					});
				}).AddTo (this);

			//ポーズ解除処理
			_State.Where (state => state == StageState.Pause)
				.Subscribe (_ =>
				{
					this.UpdateAsObservable ()
					.Where (__ => Input.GetButtonDown (ButtonName.Pause))
					.TakeUntil (_State.Where (state => state == StageState.Play))
					.First ()
					.Subscribe (__ =>
					{
						TimeManager.PauseRelease ();
						_State.Value = StageState.Play;
					});
				}).AddTo (this);

			_State.Where (state => state == StageState.Pause)
					.Subscribe (_ => AudioManager.SoundEmitter.PlaySE (_PauseSound)).AddTo (this);
			_State.Where (state => state == StageState.Talk)
				.Subscribe (_ =>
				{
					EnemySpawnerManager.StopProcess ();
					GameManager.Player.CanControl = false;
					TimeManager.TimeScale = 1.0f;
					TimeManager.EnemyTimeScale = 0.0f;
				}).AddTo (this);

			//ボスポップ時の処理
			var spawnObservable = _EnemySpawner.Select (manager => manager.EnemySpawnAsObservable ());
			/*	Observable.Merge (spawnObservable)
					.Where (enemy => enemy.tag == "Boss")
					.Subscribe (boss =>
					{
						boss.OnDestroyAsObservable().Throttle(TimeSpan.FromSeconds(2))
						.Subscribe(_ =>
						{
							SceneOverValueHolder.ScoreValue = _Score.ScoreValue.Value;
							SceneOverValueHolder.StageClearTime = _StageDeltaTime;
							SceneChanger.SceneChange (SceneType.Result);
						});
						UnityEngine.Debug.Log ("bossPop",gameObject);
					});*/

			this.UpdateAsObservable ()
				.Where (_ => _State.Value == StageState.Play)
				.Subscribe (_ => _StageDeltaTime += TimeManager.DeltaTime);

			this.OnDestroyAsObservable ()
				.Subscribe (_ =>
				{
					if (_State.Value == StageState.Pause) TimeManager.PauseRelease ();
				});

			GameManager.Player.Controller.LifeController.DeadAsObservable
				.Subscribe (_ =>
				 {
					 TimeManager.TimeScale = 1.0f;
					 TimeManager.EnemyTimeScale = 0.0f;
					 EnemySpawnerManager.StopProcess ();
				 }).AddTo (this);
		}

		private void OnDestroy ()
		{
			TimeManager.Reset ();
		}

		public void PauseRelease ()
		{
			if (_State.Value != StageState.Pause) { return; }
			TimeManager.PauseRelease ();
			_State.Value = StageState.Play;
		}
	}
}