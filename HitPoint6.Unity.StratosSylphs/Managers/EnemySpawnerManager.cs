using System;
using System.Collections;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using GameUnits;

	[Serializable]
	public class EnemySpawn
	{
		[SerializeField]
		private Vector2 _Position;

		[SerializeField]
		private float _Interval;

		[SerializeField]
		private float _StartTime;

		[SerializeField]
		private int _Count;

		[SerializeField]
		private Enemy _EnemyPrefab;

		[SerializeField]
		private float _SpeedRatio = 1f;

		public Vector2 Position
		{
			get { return _Position; }
		}

		public float Interval
		{
			get { return _Interval; }
		}

		public float StartTime
		{
			get { return _StartTime; }
		}

		public int Count
		{
			get { return _Count; }
		}

		public Enemy EnemyPrefab
		{
			get { return _EnemyPrefab; }
		}

		public float SpeedRatio
		{
			get { return _SpeedRatio; }
		}
	}

	public class EnemySpawnerManager : MonoBehaviour
	{
		[SerializeField]
		private EnemySpawn[] _EnemySpawnList;

		[SerializeField]
		private float BaseSpawnDelay;

		private Subject<Enemy> _EnemySpawnObserver = new Subject<Enemy> ();

		private Subject<Unit> _SpawnCompleteObserver = new Subject<Unit> ();

		private static float _StopFlag = 1.0f;

		public IObservable<Unit> SpawnCompleteAsObservable ()
		{
			return _SpawnCompleteObserver;
		}

		public IObservable<Unit> AllEnemyDestroiedAsObservable { get; private set; }

		public IObservable<Enemy> EnemySpawnAsObservable ()
		{
			return _EnemySpawnObserver;
		}

		private void Awake ()
		{
			var reactiveEnemyList = new ReactiveCollection<Enemy> ();

			AllEnemyDestroiedAsObservable = reactiveEnemyList.ObserveCountChanged ()
				.Where (count => count == 0)
				.SkipUntil (_SpawnCompleteObserver)
				.AsUnitObservable ();

			EnemySpawnAsObservable ()
				.Subscribe (enemy =>
				{
					reactiveEnemyList.Add (enemy);
					enemy.OnDestroyAsObservable ()
					.Subscribe (_ =>
					 {
						 reactiveEnemyList.Remove (enemy);
					 });
				});
		}

		private void Start ()
		{
			Subject<Unit> completeObserver = new Subject<Unit> ();
			IObservable<Unit> completeZipper = completeObserver;
			completeZipper.First ()
				.Subscribe (_ => _SpawnCompleteObserver.OnNext (_));

			Observable.NextFrame ()
				.Subscribe (_ =>
				 {
					 StartCoroutine (SpawnCore (_EnemySpawnList[0], completeObserver));

					 for (int i = 1; i < _EnemySpawnList.Length; i++)
					 {
						 var subject = new Subject<Unit> ();
						 var spawnData = _EnemySpawnList[i];
						 completeZipper = completeZipper.Zip (subject, (l, r) => r);
						 if (spawnData.EnemyPrefab)
						 {
							 StartCoroutine (SpawnCore (spawnData, subject));
						 }
					 }
				 });
		}

		public static void StartProcess ()
		{
			_StopFlag = 1.0f;
		}

		public static void StopProcess ()
		{
			_StopFlag = 0.0f;
		}

		private IEnumerator SpawnCore (EnemySpawn spawnData, Subject<Unit> completeObserver)
		{
			var deltaTime = 0.0f;
			while (deltaTime < spawnData.StartTime + BaseSpawnDelay)
			{
				deltaTime += TimeManager.EnemyDeltaTime * _StopFlag;
				yield return null;
			}

			var enemy = Instantiate (spawnData.EnemyPrefab, spawnData.Position, Quaternion.identity) as Enemy;
			_EnemySpawnObserver.OnNext (enemy);
			enemy.SpeedRatio = spawnData.SpeedRatio;

			for (int i = 0; i < spawnData.Count - 1; i++)
			{
				deltaTime = 0;
				while (deltaTime < spawnData.Interval)
				{
					deltaTime += TimeManager.EnemyDeltaTime * _StopFlag;
					yield return null;
				}
				enemy = Instantiate (spawnData.EnemyPrefab, spawnData.Position, Quaternion.identity) as Enemy;
				_EnemySpawnObserver.OnNext (enemy);
				enemy.SpeedRatio = spawnData.SpeedRatio;
			}
			completeObserver.OnNext (Unit.Default);
		}

		private void OnDestroy ()
		{
			StartProcess ();
		}
	}
}