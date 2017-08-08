using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using Library.DeviceInput.Constants;
	using Managers;
	using TalkEvent;
	using Utils.CSV;

	public class MoveTutorial : ITutorial
	{
		private TutorialInputObserver _InputObserver = new TutorialInputObserver ();

		private TalkEventSystem _System;
		private TextAsset _Csv;
		private SpriteRenderer _GuideRenderer;
		private GuideSpritesWithPosition _GuideSprite;
		private TimeSpan _DelaySec;

		public MoveTutorial (TalkEventSystem system, TextAsset csv, float delay, SpriteRenderer renderer, GuideSpritesWithPosition guideInfo)
		{
			_System = system;
			_Csv = csv;
			_DelaySec = TimeSpan.FromSeconds (delay);
			_GuideSprite = guideInfo;
			_GuideRenderer = renderer;
		}

		public void InputObsavationStart ()
		{
			GameManager.Player.CanControl = false;
			_System.EventDoneAsObservable ()
				.First ()
				.Subscribe (_ =>
				 {
					 GameManager.Player.CanControl = true;
					 Func<bool> up = () => Input.GetAxis (AxisName.VERTICAL) > 0;
					 Func<bool> down = () => Input.GetAxis (AxisName.VERTICAL) < 0;
					 Func<bool> left = () => Input.GetAxis (AxisName.HORIZONTAL) < 0;
					 Func<bool> right = () => Input.GetAxis (AxisName.HORIZONTAL) > 0;
					 _GuideRenderer.transform.position = _GuideSprite.Position;
					 _GuideRenderer.sprite = _GuideSprite.Sprite;
					 _InputObserver.ObsavationCompleteAsObsavable ()
						 .First ()
						 .Subscribe (__ => _GuideRenderer.enabled = false).AddTo (_System);
					 _InputObserver.ObsavationStart (up, down, left, right);
				 }).AddTo (_System);
			_System.TalkStart (TalkMessageReader.GetTalkData (_Csv));
		}

		public IObservable<Unit> ProcessDoneAsObservable ()
		{
			return _InputObserver.ObsavationCompleteAsObsavable ().Delay (_DelaySec);
		}
	}
}