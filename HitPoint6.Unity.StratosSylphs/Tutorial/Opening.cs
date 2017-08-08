using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	using Effect;
	using GameUnits;
	using TalkEvent;
	using Utils.CSV;

	public class Opening : ITutorial
	{
		private TalkEventSystem _System;
		private TextAsset _CSV1;
		private TextAsset _CSV2;
		private GameObject _Player;
		private Sprite _AirBackGroundImage;
		private Sprite _BaseBackGroundImage;
		private SpriteRenderer _Rendere;
		private GameObject _CloudPrefab;

		public Opening (TalkEventSystem system, TextAsset csv1, TextAsset csv2, GameObject player, Sprite airBackGround, GameObject cloudPrefab, Sprite baseBackGround, SpriteRenderer rendere)
		{
			_System = system;
			_CSV1 = csv1;
			_CSV2 = csv2;
			_Player = player;
			_AirBackGroundImage = airBackGround;
			_BaseBackGroundImage = baseBackGround;
			_Rendere = rendere;
			_CloudPrefab = cloudPrefab;
		}

		public void InputObsavationStart ()
		{
			_System.EventDoneAsObservable ()
				.First ()
				.Subscribe (_ =>
				 {
					 FadeInOut.Instance.OnFadeCompleteAsObservable ()
					 .First (fade => fade == Fade.Out)
					 .Subscribe (__ =>
					 {
						 _Player.SetActive (true);
						 _Player.GetComponent<Player> ().CanControl = false;
						 _Player.GetComponent<Player> ().Controller.BombController.ReminingBombCount.Value = 0;
						 _Rendere.sprite = _AirBackGroundImage;
						 _CloudPrefab.SetActive (true);
						 FadeInOut.Instance.OnFadeCompleteAsObservable ()
						 .First (fade => fade == Fade.In)
						 .Subscribe (___ =>
						  {
							  _System.TalkStart (TalkMessageReader.GetTalkData (_CSV2));
						  }).AddTo (_System);
						 FadeInOut.Instance.FadeIn (1f);
					 }).AddTo (_System);

					 FadeInOut.Instance.FadeOut (1f);
				 }).AddTo (_System);

			FadeInOut.Instance.OnFadeCompleteAsObservable ()
				.First (fade => fade == Fade.Out)
				.Subscribe (__ =>
				{
					FadeInOut.Instance.OnFadeCompleteAsObservable ()
						.First (fade => fade == Fade.In)
						.Subscribe (___ =>
						{
							_System.TalkStart (TalkMessageReader.GetTalkData (_CSV1));
						}).AddTo (_System);
					_Rendere.sprite = _BaseBackGroundImage;
					FadeInOut.Instance.FadeIn (1f);
				}).AddTo(_System);
			Observable.NextFrame ()
				.Delay (TimeSpan.FromSeconds (3f))
				.Subscribe (_ =>
				 {
					 FadeInOut.Instance.FadeOut (1f);
				 }).AddTo (_System);
		}

		public IObservable<Unit> ProcessDoneAsObservable ()
		{
			return _System.EventDoneAsObservable ().Skip (1).First ().AsUnitObservable ();
		}
	}
}