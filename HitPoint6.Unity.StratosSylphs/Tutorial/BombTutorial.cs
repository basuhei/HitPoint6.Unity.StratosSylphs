using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using GameUnits;
	using Managers;
	using TalkEvent;
	using Utils.CSV;

	public class BombTutorial : ITutorial
	{
		private TalkEventSystem _System;
		private TextAsset _CSV;
		private EventEnemyPositionSet[] _EnemyPopData;
		private TimeSpan _Delay;
		private TutorialInputObserver _InputObserver = new TutorialInputObserver ();

		private SpriteRenderer _GuideRenderer;
		private GuideSpritesWithPosition _GuideInfo;

		public BombTutorial (TalkEventSystem talkSystem,
							TextAsset csv,
							EventEnemyPositionSet[] enemyPopData,
							float delaySec,
							SpriteRenderer guideRenderer,
							GuideSpritesWithPosition guideInfo)
		{
			_System = talkSystem;
			_CSV = csv;
			_EnemyPopData = enemyPopData;
			_Delay = TimeSpan.FromSeconds (delaySec);

			_GuideRenderer = guideRenderer;
			_GuideInfo = guideInfo;
		}

		public void InputObsavationStart ()
		{
			GameManager.Player.CanControl = false;
			_System.EventDoneAsObservable ()
				.First ()
				.Subscribe (_ =>
				 {
					 GameManager.Player.Controller.BombController.ReminingBombCount.Value = 2;
					 _GuideRenderer.enabled = true;
					 _GuideRenderer.transform.position = _GuideInfo.Position;
					 _GuideRenderer.sprite = _GuideInfo.Sprite;
					 GameManager.Player.CanControl = true;
					 var bombLaunched = false;
					 var isAllEnemyDestroied = false;
					 GameManager.Player.Controller.BombController.LaunchMissileAsObservable ()
					 .First ()
					 .Subscribe (__ =>
					 {
						 bombLaunched = true;
					 }).AddTo (_System);

					 var enemies = new Enemy[_EnemyPopData.Length];
					 for (int i = 0; i < _EnemyPopData.Length; i++)
					 {
						 enemies[i] = UnityEngine.Object.Instantiate (_EnemyPopData[i].Enemy, _EnemyPopData[i].PopPoint, Quaternion.identity);
					 }

					 var enemiesDestroyStream = enemies[1].OnDestroyAsObservable ();
					 for (int i = 1; i < enemies.Length; i++)
					 {
						 enemiesDestroyStream.Zip (enemies[i].OnDestroyAsObservable (), (l, r) => r);
					 }
					 enemiesDestroyStream.First ()
					 .Subscribe (__ => isAllEnemyDestroied = true).AddTo (_System);
					 Func<bool> bombLaunch = () => bombLaunched;
					 Func<bool> checkEliminateEnemies = () => isAllEnemyDestroied;

					 _InputObserver.ObsavationCompleteAsObsavable ()
						.First ()
						.Subscribe (__ => _GuideRenderer.enabled = false).AddTo (_System);
					 _InputObserver.ObsavationStart (bombLaunch, checkEliminateEnemies);
				 }).AddTo (_System);
			_System.TalkStart (TalkMessageReader.GetTalkData (_CSV));
		}

		public IObservable<Unit> ProcessDoneAsObservable ()
		{
			return _InputObserver.ObsavationCompleteAsObsavable ().Delay (_Delay);
		}
	}
}