using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using GameUnits;
	using Managers;

	[Serializable]
	public class PlayerLifeController
	{
		[SerializeField]
		private int _MaxLife;

		[SerializeField]
		private int _MaxRemainingUnit;

		[SerializeField]
		private float _RegenerateStartTime = 2.0f;

		[SerializeField]
		private float _RegenerateSpeed = 0.3f;

		private ReactiveProperty<float> _PlayerLife;
		private ReactiveProperty<int> _RemainingUnit;
		private Subject<Unit> _OnDamageObserver;

		public PlayerLifeController ()
		{
		}

		public void Awake (Player player)
		{
			_PlayerLife = new ReactiveProperty<float> (20);
			PlayerLife = _PlayerLife.ToReadOnlyReactiveProperty ();
			_RemainingUnit = new ReactiveProperty<int> (10);
			RemainingUnit = _RemainingUnit;
			DeadAsObservable = PlayerLife.Where (l => l <= 0)
										.Publish ()
										.RefCount ()
										.AsUnitObservable ();
			_OnDamageObserver = new Subject<Unit> ();
			OnDamageAsObservable = _OnDamageObserver;

			//TODO:死亡後のイベントなどを追加しないと駄目
			DeadAsObservable
				.Throttle (TimeSpan.FromSeconds (0.5f))
				.Subscribe (_ =>
				{
					_RemainingUnit.Value--;

					//_PlayerLife.Value = _MaxLife;
				});

			DeadAsObservable.Subscribe (_ => Debug.Log ("Dead"));
			_PlayerLife.Value = _MaxLife;
			_RemainingUnit.Value = _MaxRemainingUnit;
			player.OnTriggerEnter2DAsObservable ()
				.ThrottleFirst (TimeSpan.FromSeconds (0.05))
				.Subscribe (c =>
				{
					var d = c.GetComponent<IDamage> ();
					if (d == null) { return; }
					_PlayerLife.Value -= d.Damage;

					if (_PlayerLife.Value <= 0)
					{
						return;
					}
					_OnDamageObserver.OnNext (Unit.Default);
				});
			DeadAsObservable.Subscribe (_ => AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.DeadSound));

			var currentLife = (float)MaxLife;
			var deltaTime = 0.0f;

			LifeRegenerateStartAsObservable = OnDamageAsObservable
				.Throttle (TimeSpan.FromSeconds (_RegenerateStartTime)).Publish ()
				.RefCount ();

			LifeRegenerateStartAsObservable
				.Subscribe (_ =>
				{
					currentLife = _PlayerLife.Value;
					var ratio = currentLife / _MaxLife;
					deltaTime = 0.0f;
					player.UpdateAsObservable ()
					.TakeUntil (OnDamageAsObservable)
					.TakeUntil (DeadAsObservable)
					.Where (__ => _PlayerLife.Value > 0)
					.Subscribe (__ =>
					 {
						 _PlayerLife.Value = Mathf.Lerp (0, _MaxLife, ratio + deltaTime * _RegenerateSpeed);
						 deltaTime += TimeManager.PlayerDeltaTime;
					 });
				});
		}

		public int MaxLife
		{
			get { return _MaxLife; }
		}

		public ReadOnlyReactiveProperty<float> PlayerLife
		{
			get;
			private set;
		}

		public ReactiveProperty<int> RemainingUnit
		{
			get;
			private set;
		}

		public IObservable<Unit> OnDamageAsObservable
		{
			get;
			private set;
		}

		public IObservable<Unit> DeadAsObservable
		{
			get;
			private set;
		}

		public IObservable<Unit> LifeRegenerateStartAsObservable
		{
			get;
			private set;
		}
	}
}