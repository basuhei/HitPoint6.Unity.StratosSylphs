using HitPoint6.Unity.StratosSylphs.GameUnits;
using System;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Managers;

	[Serializable]
	public class PlayerColliderController
	{
		[SerializeField]
		private float _InvincibleTimeOnDamage;

		private Subject<Unit> _OnInvincibleObserver;
		private Subject<Unit> _OnInvincibleEndObserver;

		private bool _Invincible = false;

		public IObservable<Unit> OnInvincibleAsObservable
		{
			get;
			private set;
		}

		public IObservable<Unit> OnInvincibleEndAsObservable
		{
			get;
			private set;
		}

		public PlayerColliderController ()
		{
			_OnInvincibleObserver = new Subject<Unit> ();
			OnInvincibleAsObservable = _OnInvincibleObserver;

			_OnInvincibleEndObserver = new Subject<Unit> ();
			OnInvincibleEndAsObservable = _OnInvincibleEndObserver;
		}

		public void Start (Player player)
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

			//TODO:DEBUG COMMAND 無敵:M
			Observable.EveryUpdate ()
			   .Where (__ => Input.GetKeyDown (KeyCode.M))
			   .Subscribe (__ =>
			   {
				   var collider = player.GetComponentsInChildren<Collider2D> (true).ToList ();
				   collider.ForEach (c => c.enabled = false);
			   })
			   .AddTo (player);
#endif
			Observable.NextFrame ()
				.Subscribe (_ =>
				 {
					 var colliders = player.GetComponentsInChildren<Collider2D> (true);
					 player.Controller.LifeController
						 .OnDamageAsObservable
						 .Subscribe (__ => player.StartCoroutine (_ColliderControl (colliders, _InvincibleTimeOnDamage)));

					 player.Controller.LifeController
						 .DeadAsObservable
						 .Subscribe (__ => player.StartCoroutine (_ColliderControl (colliders, 100)));
				 });
		}

		private IEnumerator _ColliderControl (Collider2D[] colliders, float invincibleTime)
		{
			float deltaTime = 0.0f;
			if (_Invincible) { yield break; }
			_Invincible = true;
			foreach (var c in colliders)
			{
				c.enabled = false;
			}
			_OnInvincibleObserver.OnNext (Unit.Default);
			while (invincibleTime > deltaTime)
			{
				deltaTime += TimeManager.PlayerDeltaTime;
				yield return null;
			}
			foreach (var c in colliders)
			{
				c.enabled = true;
			}
			_Invincible = false;
			_OnInvincibleEndObserver.OnNext (Unit.Default);
		}
	}
}