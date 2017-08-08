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

	public class ShotTutorial : ITutorial
	{
		private TutorialInputObserver _InputObserver = new TutorialInputObserver ();
		private TextAsset _CSV;
		private TalkEventSystem _System;
		private EventEnemyPositionSet[] _EnemyData;
		private TimeSpan _DelaySec;
		private SpriteRenderer _GuideRenderer;
		private GuideSpritesWithPosition _GuideInfo;

		public ShotTutorial (TalkEventSystem system, TextAsset csv, EventEnemyPositionSet[] data, float delay, SpriteRenderer guideRenderer, GuideSpritesWithPosition guideInfo)
		{
			_System = system;
			_CSV = csv;
			_EnemyData = data;
			_DelaySec = TimeSpan.FromSeconds (delay);
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
					 _GuideRenderer.enabled = true;
					 _GuideRenderer.transform.position = _GuideInfo.Position;
					 _GuideRenderer.sprite = _GuideInfo.Sprite;
					 var enemyList = new Enemy[_EnemyData.Length];
					 for (int i = 0; i < _EnemyData.Length; i++)
					 {
						 enemyList[i] = UnityEngine.Object.Instantiate (_EnemyData[i].Enemy, _EnemyData[i].PopPoint, Quaternion.identity);
					 }

					 GameManager.Player.CanControl = true;

					 var enemiesDestroied = false;
					 var reloaded = false;
					 var shooted = false;
					 Func<bool> reload = () => reloaded;
					 Func<bool> shot = () => shooted;
					 Func<bool> eliminateEnemies = () => enemiesDestroied;
					 IObservable<Unit> enemiesDestroyStream = enemyList[0].OnDestroyAsObservable ();
					 for (int i = 1; i < enemyList.Length; i++)
					 {
						 enemiesDestroyStream = enemiesDestroyStream.Zip (enemyList[i].OnDestroyAsObservable (), (l, r) => r);
					 }
					 enemiesDestroyStream.First ()
					 .Subscribe (__ => enemiesDestroied = true).AddTo (_System);

					 GameManager.Player.Controller.FiringController.ReloadAsObservable ()
					 .First ()
					 .Subscribe (__ => reloaded = true).AddTo (_System);
					 GameManager.Player.Controller.FiringController.ShotAsObservable ()
					 .First ()
					 .Subscribe (__ => shooted = true).AddTo (_System);

					 _InputObserver.ObsavationCompleteAsObsavable ()
						.First ()
						.Subscribe (__ => _GuideRenderer.enabled = false).AddTo (_System);
					 _InputObserver.ObsavationStart (reload, shot, eliminateEnemies);
				 }).AddTo (_System);
			_System.TalkStart (TalkMessageReader.GetTalkData (_CSV));
		}

		public IObservable<Unit> ProcessDoneAsObservable ()
		{
			return _InputObserver.ObsavationCompleteAsObsavable ().Delay (_DelaySec);
		}
	}
}