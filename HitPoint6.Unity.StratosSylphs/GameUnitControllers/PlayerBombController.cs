using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Constants;
	using GameUnits;
	using Library.DeviceInput;
	using Managers;

	[Serializable]
	public class PlayerBombController
	{
		[SerializeField]
		private GameObject[] _LaunchPad;

		[SerializeField]
		private int _LockOnLimit;

		[SerializeField]
		private float _ConcentrateLimit;

		[SerializeField]
		private Missile _Missile;

		[SerializeField]
		private float _TargetTimeScale = 0.1f;

		[SerializeField]
		private float _SecondLockDelay = 0.5f;

		[SerializeField]
		private float _MultiLockDelay = 0.3f;

		[SerializeField]
		private int _ReminingMissileCount = 2;

		[SerializeField]
		private int _LeastMissileLaunchCount = 6;

		private List<Collider2D> _LockOnEnemyQueue;
		private List<Missile> _MissileList;
		private bool _LockOnMode = false;
		private HashSet<Collider2D> _FirstLockedOnEnemy;
		private List<Collider2D> _MultiLockOnEnemy;
		private Subject<Collider2D> _FirstLockObserver;
		private Subject<Collider2D> _MultiLockObserver;
		private Subject<Unit> _LockOnProcessFinishObserver;
		private Subject<Tuple<Missile, Collider2D>> _MissileLaunchObserver;
		private ReactiveProperty<int> _ReminingBombCount;

		public IObservable<Collider2D> FirstLockOnAsObservable ()
		{
			return _FirstLockObserver;
		}

		public IObservable<Tuple<Missile, Collider2D>> LaunchMissileAsObservable ()
		{
			return _MissileLaunchObserver;
		}

		public IObservable<Collider2D> MultiLockAsObservable ()
		{
			return _MultiLockObserver;
		}

		public int LockOnLimit
		{
			get { return _LockOnLimit; }
		}

		public bool LockOnMode
		{
			get { return _LockOnMode; }
		}

		public ReactiveProperty<int> ReminingBombCount
		{
			get;
			private set;
		}

		public void Awake ()
		{
			if (_LaunchPad.Length != 4)
			{
				throw new InvalidOperationException ("LaunchPadのSizeには4を指定してつかあさい");
			}
			for (int i = 0; i < _LaunchPad.Length; i++)
			{
				if (_LaunchPad[i] == null)
				{
					throw new InvalidOperationException (string.Format ("LaunchPadに発射場所となるGameObjectを追加してください配列{0}番目", i));
				}
			}
			for (int i = 0; i < LockOnLimit; i++)
			{
				_MissileList.Add (UnityEngine.Object.Instantiate<Missile> (_Missile));
			}
			_ReminingBombCount = new ReactiveProperty<int> (_ReminingMissileCount); //ボムの数
			ReminingBombCount = _ReminingBombCount;
		}

		public IObservable<Unit> LockOnProcessFinishAsObservable ()
		{
			return _LockOnProcessFinishObserver;
		}

		public PlayerBombController ()
		{
			_FirstLockObserver = new Subject<Collider2D> ();
			_MultiLockObserver = new Subject<Collider2D> ();
			_LockOnProcessFinishObserver = new Subject<Unit> ();
			_FirstLockedOnEnemy = new HashSet<Collider2D> ();
			_MultiLockOnEnemy = new List<Collider2D> ();
			_MissileList = new List<Missile> (LockOnLimit);
			_MissileLaunchObserver = new Subject<Tuple<Missile, Collider2D>> ();
			_LockOnMode = false;
		}

		public void Start (Player player)
		{
			_LockOnProcessFinishObserver
			.Subscribe (_ =>
			 {
				 Missile missile;
				 Collider2D target = null;
				 _LockOnEnemyQueue = _FirstLockedOnEnemy
				 .Concat (_MultiLockOnEnemy).ToList ();

				 Observable
				 .Timer (TimeSpan.FromSeconds (0.1), TimeSpan.FromSeconds (0.1))
				 .TakeUntil (player.Controller.LifeController.DeadAsObservable)
				 .Take (Mathf.Max (_LockOnEnemyQueue.Count, _LeastMissileLaunchCount)) // ミサイルの最低発射数
				 .DoOnCompleted (() =>
				  {
					  _LockOnMode = false;
					  _LockOnEnemyQueue.Clear ();
				  })
				 .Subscribe (__ =>
				 {
					 missile = _MissileList.FirstOrDefault (m => !m.gameObject.activeInHierarchy);
					 for (int i = 0; i < _LockOnEnemyQueue.Count; i++)
					 {
						 if (_LockOnEnemyQueue[i] == null)
						 {
							 _LockOnEnemyQueue.RemoveAt (i);
							 continue;
						 }
						 target = _LockOnEnemyQueue[i];
						 _LockOnEnemyQueue.Remove (target);
						 break;
					 }
					 if (missile != null)
					 {
						 _MissileLaunchObserver.OnNext (new Tuple<Missile, Collider2D> (missile, target));
						 missile.Launch (_LaunchPad[UnityEngine.Random.Range (0, 3)].transform.position, target);
					 }
				 });
			 });

			InputObservable.Instance.OnButtonDownAsObservable (ButtonName.Bomb)
				.Where (_ => _ReminingBombCount.Value > 0)
				.Where (_ => player.CanControl)
				.Where (_ => _MissileList.Where (m => m.gameObject.activeInHierarchy).Count () < 1)
				.Subscribe (__ => player.StartCoroutine (_LockOnProcess (player)))
				.AddTo (player);
		}

		private IEnumerator _LockOnProcess (Player player)
		{
			if (_LockOnMode)
			{
				yield break;
			}
			_LockOnMode = true;
			_ReminingBombCount.Value--;
			List<IDisposable> lockOnEnemyStream = new List<IDisposable> ();

			List<IDisposable> multilock = new List<IDisposable> ();

			var multiLockStream = _FirstLockObserver
				.Delay (TimeSpan.FromSeconds (_SecondLockDelay))
				.Subscribe (e =>
				{
					Debug.Log ("マルチロックスタート");
					var mulloc = e.OnTriggerStay2DAsObservable ()
					 .Where (_ => _LockOnLimit >= _FirstLockedOnEnemy.Count + _MultiLockOnEnemy.Count)
					 .Where (c => c.tag == Tags.TargetSight)
					 .ThrottleFirst (TimeSpan.FromSeconds (_MultiLockDelay))
					 .Subscribe (_ =>
					 {
						 Debug.Log ("マルチロック");
						 _MultiLockObserver.OnNext (e);
						 _MultiLockOnEnemy.Add (e);
					 });
					multilock.Add (mulloc);
				});

			var firstLockStream = UIManager.TargetSight
											.OnTriggerStay2DAsObservable ()
											.Where (_ => _LockOnLimit >= _FirstLockedOnEnemy.Count + _MultiLockOnEnemy.Count)
											.Subscribe (c =>
											{
												var tform = c.transform;
												while (tform.parent != null)
												{
													tform = tform.parent;
												}
												var enemy = tform.GetComponentInChildren<Enemy> (true);
												if (enemy == null) { return; }

												if (!_FirstLockedOnEnemy.Contains (c))
												{
													_FirstLockObserver.OnNext (c);
												}
												_FirstLockedOnEnemy.Add (c);
											});

			TimeManager.TimeScale = _TargetTimeScale;
			var deltaTime = 0.0f;
			while (player.CanControl && deltaTime < _ConcentrateLimit && Input.GetButton (ButtonName.Bomb) || GameManager.Instance.Pause)
			{
				if (GameManager.Instance.Pause) { yield return null; }
				deltaTime += Time.deltaTime;
				yield return null;
			}
			_LockOnProcessFinishObserver.OnNext (Unit.Default);

			if (player.CanControl) TimeManager.SmoothStepTimeScale (1, 1);

			firstLockStream.Dispose ();
			multiLockStream.Dispose ();
			multilock.ForEach (d => d.Dispose ());
			lockOnEnemyStream.ForEach (d => d.Dispose ());
			lockOnEnemyStream.Clear ();
			_MultiLockOnEnemy.Clear ();
			_FirstLockedOnEnemy.Clear ();
		}
	}
}