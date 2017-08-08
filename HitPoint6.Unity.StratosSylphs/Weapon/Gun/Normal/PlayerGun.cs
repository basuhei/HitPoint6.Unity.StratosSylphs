using System.Collections;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using InitializeData;
	using Managers;

	public class PlayerGun : IObservableShooter
	{
		#region サポートクラス

		private class _ShotArgsTuple
		{
			private Vector2 _Position;
			private Vector2 _Direction;

			public Vector2 Position
			{
				get { return _Position; }
			}

			public Vector2 Direction
			{
				get { return _Direction; }
			}

			public void SetTuple (Vector2 position, Vector2 direction)
			{
				_Position = position;
				_Direction = direction;
			}
		}

		#endregion サポートクラス

		private GunData _GunData;

		private float _ReloadTime;

		private IReloadableContainer _Resource;

		private Subject<Unit> _ReLoadObserver;

		private Subject<_ShotArgsTuple> _ShotOrderObserver;
		private Subject<Unit> _ShotObserver;

		private _ShotArgsTuple _ShotArgs;

		public ReadOnlyReactiveProperty<uint> ReminingResourceCount ()
		{
			return _Resource.RemainingResouceAsObservable ();
		}

		public IObservable<Unit> ReloadAsObservable ()
		{
			return _ReLoadObserver;
		}

		public IObservable<Unit> ShootAsObservable ()
		{
			return _ShotObserver;
		}

		public float ShotPerSec
		{
			get { return _GunData.ShotPerSecond; }
		}

		private bool _Reloading = false;

		private bool _CanShot = true;

		public PlayerGun (GunData data)

		{
			_ShotArgs = new _ShotArgsTuple ();
			_ReLoadObserver = new Subject<Unit> ();
			_ShotOrderObserver = new Subject<_ShotArgsTuple> ();
			_ShotObserver = new Subject<Unit> ();
			_Resource = new Magazine (data);
			_GunData = data;

			_ShotOrderObserver
				.Where (_ => !_Reloading)
				.Where (_ => _CanShot)
				.Subscribe (tuple =>
				{
					IShootable bullet;
					if (_Resource.Load (out bullet))
					{
						bullet.Shoot (tuple.Position, tuple.Direction);
						_ShotObserver.OnNext (Unit.Default);
						Observable.FromCoroutine<Unit> (observer => DeltaTimer (observer))
								.Subscribe (time => { });
						return;
					}

					//_ReloadAuto ();
				});
		}

		private IEnumerator DeltaTimer (IObserver<Unit> observer)
		{
			if (!_CanShot) { yield break; }
			_CanShot = false;
			var shotpersec = 1f / ShotPerSec;
			var deltaTime = 0.0f;
			while (deltaTime < shotpersec)
			{
				deltaTime += TimeManager.PlayerDeltaTime;
				observer.OnNext (Unit.Default);
				yield return null;
			}
			_CanShot = true;
			observer.OnCompleted ();
		}

		private IEnumerator _ReloadTimer (IObserver<Unit> observer, float reloadTime)
		{
			if (_Resource.RemainingResouceAsObservable ().Value == _GunData.ReminingBulletCount)
			{
				yield break;
			}
			_Reloading = true;
			_ReLoadObserver.OnNext (Unit.Default);
			var deltaTime = 0.0f;
			while (reloadTime > deltaTime)
			{
				deltaTime += TimeManager.PlayerDeltaTime;
				yield return null;
			}
			observer.OnNext (Unit.Default);
			_Reloading = false;
			observer.OnCompleted ();
		}

		public void ShootOut (Vector2 direction)
		{
			_ShotArgs.SetTuple (GameManager.Player.Controller.StateController.ShootPoint.transform.position, direction);
			_ShotOrderObserver.OnNext (_ShotArgs);
		}

		private void _ReloadManual ()
		{
			if (_Reloading) { return; }
			Observable.FromCoroutine<Unit> (_ => _ReloadTimer (_, _GunData.ReloadTime))
				.Subscribe (_ =>
				{
					_Resource.Reload ();
				});
		}

		private void _ReloadAuto ()
		{
			if (_Reloading) { return; }
			Observable.FromCoroutine<Unit> (_ => _ReloadTimer (_, _GunData.ReloadTimeAuto))
				.Subscribe (_ =>
				{
					_Resource.Reload ();
				});
		}

		public void Reload ()
		{
			_ReloadManual ();
		}
	}
}