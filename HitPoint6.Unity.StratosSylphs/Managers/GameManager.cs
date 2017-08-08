using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using GameUnits;
	using InitializeData;
	using Library.CustomizedMonoBehavior;
	using Library.DeviceInput;

	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		[SerializeField]
		private PlayerEventBehaviourData _MoveOnPlayerDeath;

		[SerializeField]
		private int _ContinuCount;

		private Subject<Player> _PlayerInstatiateObserver = new Subject<Player> ();

		public bool Pause
		{
			get
			{
				return TimeManager.IsPause;
			}
		}

		public static Player Player
		{
			get;
			private set;
		}

		protected override void Awake ()
		{
			base.Awake ();
			IDisposable disposable = null;
			_PlayerInstatiateObserver
				.Subscribe (player =>
				{
					if (disposable != null)
					{
						disposable.Dispose ();
					}

					player.UpdateAsObservable ()
					.First ()
					.Subscribe (_ =>
					{
						disposable = player.Controller.LifeController
						 .DeadAsObservable
						 .Subscribe
						 (__ =>
						   {
							   player.Controller.MoveController.EventMove (_MoveOnPlayerDeath.EventData);
							   player.Controller.MoveController.EventMoveDoneAsObservable ()
							   .Subscribe (___ =>
								{
									player.gameObject.SetActive (false);
									Destroy (player.gameObject);
								});
						   });
					});

					//_DefinePauseProcess ();
				});
		}

		public void RegisterPlayer (Player player)
		{
			Player = player;
			_PlayerInstatiateObserver.OnNext (player);
		}

		public IObservable<Player> OnRegisterPlayerAsObservable ()
		{
			return _PlayerInstatiateObserver;
		}

		private void _DefinePauseProcess ()
		{
			var publish = InputObservable.Instance.OnKeyDownAsObservable (KeyCode.Escape)
				.Where (_ => !Pause)
				.First ()
				.Publish ()
				.RefCount ();

			publish
				.Subscribe (_ =>
				{
					TimeManager.Pause ();
				});
			publish
				.ThrottleFrame (1)
				.Subscribe (_ => _DefinePauseRelease ());
		}

		private void _DefinePauseRelease ()
		{
			var publish = InputObservable.Instance.OnKeyDownAsObservable (KeyCode.Escape)
				 .Where (_ => Pause)
				 .First ()
				 .Publish ()
				 .RefCount ();
			publish
				 .Subscribe (_ =>
				  {
					  TimeManager.PauseRelease ();
				  });
			publish
				.ThrottleFrame (1)
				.Subscribe (_ => _DefinePauseProcess ());
		}
	}
}