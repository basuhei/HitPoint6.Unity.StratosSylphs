using System;
using System.Collections;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using GameUnits;
	using InitializeData;
	using Library.DeviceInput;
	using Managers;

	[Serializable]
	public class PlayerMoveController
	{
		[SerializeField]
		private float _Speed = 20f;

		private Subject<Unit> _EventMoveObserver = new Subject<Unit> ();

		private Vector2 _Velocity;
		private MoveClamper _MoveClamp;
		private Player _Player;

		private IEnumerator _LateFixiedUpdate (Action action)
		{
			while (true)
			{
				action ();
				yield return new WaitForFixedUpdate ();
			}
		}

		public IObservable<Unit> EventMoveDoneAsObservable ()
		{
			return _EventMoveObserver;
		}

		public void Start (Player player)
		{
			_Player = player;
			DefineInputStream (player);
			DefineMoveStream (player);
		}

		private void DefineMoveStream (Player player)
		{
			var rigidBody2D = player.GetComponent<Rigidbody2D> ();
			_MoveClamp = new MoveClamper (new Vector2 (3.8f, 3), rigidBody2D, new Vector2 (0.3f, 0.3f));

			player.FixedUpdateAsObservable ()
				.Where (_ => !player.Controller.StateController.IsAimMode.Value)
				.Subscribe (_ =>
				{
					rigidBody2D.MovePosition (rigidBody2D.position + _Velocity * _Speed * TimeManager.PlayerFixedDeltaTime);
				});

			player.FixedUpdateAsObservable ()
				.Where (_ => player.Controller.StateController.IsAimMode.Value)
				.Subscribe (_ => rigidBody2D.MovePosition (rigidBody2D.position + _Velocity * _Speed * 0.2f * TimeManager.PlayerFixedDeltaTime));

			player.StartCoroutine (_LateFixiedUpdate (() =>
									{
										if (player.CanControl) _MoveClamp.Clamp ();
									}));

			//死んだ後
			//player.Controller.LifeController
			//	.DeadAsObservable
			//	.Subscribe (_ => rigidBody2D.position = Camera.main.ViewportToWorldPoint (new Vector3 (-0.1f, 0.5f, 0.5f)));
		}

		private void DefineInputStream (Player player)
		{
			player.UpdateAsObservable ()
				.First ()
				.Subscribe (__ =>
				 {
					 InputObservable.Instance.HorizontalRawAsObservable ()
						 .Where (_ => player.CanControl)
						 .Subscribe (x =>
						 {
							 _Velocity.x = x;
						 })
						 .AddTo (player);

					 InputObservable.Instance.VerticalRawAsObservable ()
						 .Where (_ => player.CanControl)
						 .Subscribe (y =>
						 {
							 _Velocity.y = y;
						 })
						 .AddTo (player);

					 player.ObserveEveryValueChanged (pl => pl.CanControl)
					 .Where (controlable => !controlable)
					 .Subscribe (_ => _Velocity = Vector2.zero);
				 });
		}

		private IEnumerator _EventMoveCore (PlayerEventParams[] behaviour, Player player)
		{
			player.CanControl = false;
			float deltaTime;
			Vector2 direction;
			for (int i = 0; i < behaviour.Length; i++)
			{
				deltaTime = 0.0f;
				direction = behaviour[i].Direction.normalized;
				while (behaviour[i].OparatingTime >= deltaTime)
				{
					_Velocity = direction.normalized * behaviour[i].Easing.Evaluate (deltaTime / behaviour[i].OparatingTime);
					deltaTime += TimeManager.PlayerDeltaTime;
					yield return null;
				}
			}
			_EventMoveObserver.OnNext (Unit.Default);
			player.CanControl = true;
		}

		public void EventMove (PlayerEventParams[] behaviour)
		{
			if (!_Player.CanControl) { return; }
			_Player.StartCoroutine (_EventMoveCore (behaviour, _Player));
		}
	}
}