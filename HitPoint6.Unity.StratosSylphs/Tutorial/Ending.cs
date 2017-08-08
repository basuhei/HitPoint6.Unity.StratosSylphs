using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using Managers;
	using Scene;
	using TalkEvent;
	using Utils.CSV;

	public class Ending : ITutorial
	{
		private TextAsset _BeforeChoice;
		private TextAsset _AfterChoice;

		private TalkEventSystem _System;

		public Ending (TalkEventSystem system, TextAsset before, TextAsset after)
		{
			_System = system;
			_BeforeChoice = before;
			_AfterChoice = after;
		}

		public void InputObsavationStart ()
		{
			GameManager.Player.CanControl = false;
			_System.DialogResultAsObservable ()
				.First ()
				.Where (agree => agree)
				.Subscribe (_ =>
				 {
					 _System.EventDoneAsObservable ()
					 .Skip (1)
					 .First ()
					 .Subscribe (__ =>
					  {
						  SceneChanger.SceneChange (SceneType.StageOne);
					  }).AddTo (_System);
					 _System.EventDoneAsObservable ()
					 .DelayFrame (10)
					 .First ()
					 .Subscribe (__ =>
					  {
						  _System.TalkStart (TalkMessageReader.GetTalkData (_AfterChoice));
					  }).AddTo (_System);
				 }).AddTo (_System);

			_System.DialogResultAsObservable ()
				.First ()
				.Where (agree => !agree)
				.Subscribe (_ =>
				 {
					 _System.EventDoneAsObservable ()
					 .First ()
					 .Subscribe (__ => SceneChanger.SceneChange (SceneType.Title)).AddTo (_System);
				 }).AddTo (_System);
			_System.TalkStart (TalkMessageReader.GetTalkData (_BeforeChoice));
		}

		public IObservable<Unit> ProcessDoneAsObservable ()
		{
			return null;
		}
	}
}