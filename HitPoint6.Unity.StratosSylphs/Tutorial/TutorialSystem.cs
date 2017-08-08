using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using GameUnits;
	using UI;
	using Effect;

	public class TutorialSystem : MonoBehaviour
	{
		[SerializeField]
		private TutorialEvent _EventData;

		[SerializeField]
		private TalkEvent.TalkEventSystem _System;

		[SerializeField]
		private Canvas _TargetSightCanvas;

		[SerializeField]
		private Player _Player;

		[SerializeField]
		private Sprite _NextBackGround;

		[SerializeField]
		private Sprite _BaseBackGround;

		[SerializeField]
		private SpriteRenderer _BackGroundRenderer;

		[SerializeField]
		private SpriteRenderer _GuideRenderer;

		[SerializeField]
		private EventEnemyPositionSet[] _ShotTutorialEnemyPopData;

		[SerializeField]
		private EventEnemyPositionSet[] _BombTutorialEnemyPopData;

		[SerializeField]
		private TutorialControlGuideSprites _ControlGuide;

		[SerializeField]
		private float _Delay;

		[SerializeField]
		private GameObject _CloudPrefab;

		private List<ITutorial> TutorialList = new List<ITutorial> (2 * 4);

		private void Start ()
		{
			if (_ShotTutorialEnemyPopData.Length == 0)
			{
				throw new InvalidProgramException ("ShotTutorialEnemyPopDataが設定されていません");
			}
			if (_BombTutorialEnemyPopData.Length == 0)
			{
				throw new InvalidProgramException ("BombTutorialEnemyPopDataが設定されていません");
			}
			if (_Delay <= 0)
			{
				throw new InvalidProgramException ("Delayは0以上でなければいけません");
			}
			_Player.gameObject.SetActive (false);
			var opening = new Opening (_System, _EventData.Opening, _EventData.Opening2, _Player.gameObject, _NextBackGround, _CloudPrefab, _BaseBackGround, _BackGroundRenderer);

			var moveTutorial = new MoveTutorial (_System, _EventData.MoveTutorial, _Delay, _GuideRenderer, _ControlGuide.Move);

			var shotTutorial = new ShotTutorial (_System, _EventData.ShotTutorial, _ShotTutorialEnemyPopData, _Delay, _GuideRenderer, _ControlGuide.Shot);

			var bombTutorial = new BombTutorial (_System, _EventData.BombTutorial, _BombTutorialEnemyPopData, _Delay, _GuideRenderer, _ControlGuide.Bomb);

			var ending = new Ending (_System, _EventData.Ending, _EventData.Ending2);

			moveTutorial.ProcessDoneAsObservable ().Subscribe (_ => Debug.Log ("MoveOwari"));

			TutorialList.Add (opening);
			TutorialList.Add (moveTutorial);
			TutorialList.Add (shotTutorial);
			TutorialList.Add (bombTutorial);
			TutorialList.Add (ending);

			for (int i = 0; i < TutorialList.Count - 1; i++)
			{
				var loopCount = i;
				TutorialList[i].ProcessDoneAsObservable ()
					.First ()
					.Subscribe (_ => TutorialList[loopCount + 1].InputObsavationStart ());
			}

			this.UpdateAsObservable ()
				.Where (_ =>Input.GetKeyDown (KeyCode.Escape))
				.Subscribe (_ => Scene.SceneChanger.SceneChange (Scene.SceneType.Title));
			opening.ProcessDoneAsObservable ()
				.First ()
				.Subscribe (_ => _TargetSightCanvas.enabled = true);
			opening.InputObsavationStart ();

		}

		private void OnDestroy ()
		{
			_Player = null;
			_BackGroundRenderer = null;
			_System = null;
			_EventData = null;
			TutorialList.Clear ();
			TutorialList = null;
		}
	}
}