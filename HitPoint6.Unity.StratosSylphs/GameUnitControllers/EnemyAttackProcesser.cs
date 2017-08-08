using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using GameUnits;
	using InitializeData;
	using Managers;

	public class EnemyAttackProcesser : IProcesser
	{
		private interface IAttackProcesser
		{
			IObservable<Unit> CompleteAsObservable ();

			IEnumerator Process ();
		}

		private class nWayShot : IAttackProcesser
		{
			private Subject<Unit> _CompleteObserver;
			private nWayShotData _Data;

			private Enemy _Enemy;

			public nWayShot (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as nWayShotData;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;

				while (_Data.Duration > deltaTime)
				{
					Vector2 direction = Quaternion.AngleAxis (_Data.OriginAngle, Vector3.forward) * new Vector2 (_Enemy.Sign, 0);
					for (int i = 0; i < _Data.WayCount; i++)
					{
						_Enemy.Shot (_Data.Bullet, direction);
						direction = Quaternion.AngleAxis (_Data.GapAngle, Vector3.forward) * direction;
					}

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
						deltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					shotDeltaTime = 0.0f;
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class nWayShotRotation : IAttackProcesser
		{
			private Subject<Unit> _CompleteObserver;
			private nWayShotRotationData _Data;
			private Enemy _Enemy;

			public nWayShotRotation (EnemyAttackDataBase data, Enemy enemy)
			{
				_Enemy = enemy;
				_Data = data as nWayShotRotationData;
				_CompleteObserver = new Subject<Unit> ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;
				var originAngle = _Data.OriginAngle;

				while (_Data.Duration > deltaTime)
				{
					Vector2 direction = Quaternion.AngleAxis (originAngle, Vector3.forward) * new Vector2 (_Enemy.Sign, 0);
					for (int i = 0; i < _Data.WayCount; i++)
					{
						_Enemy.Shot (_Data.Bullet, direction);
						direction = Quaternion.AngleAxis (_Data.GapAngle, Vector3.forward) * direction;
					}

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
						deltaTime += TimeManager.EnemyDeltaTime;
						originAngle += 1 * _Data.RotationSpeed.Evaluate (deltaTime) * TimeManager.EnemyDeltaTime;
						yield return null;
					}
					shotDeltaTime = 0.0f;
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class nWayShotTargetPlayer : IAttackProcesser
		{
			private Subject<Unit> _CompleteObserver;
			private nWayShotTargetPlayerData _Data;

			private Enemy _Enemy;

			public nWayShotTargetPlayer (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as nWayShotTargetPlayerData;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;

				while (_Data.Duration > deltaTime)
				{
					Quaternion rotation = _Data.WayCount % 2 == 0 ? Quaternion.AngleAxis ((-_Data.GapAngle * (1f - (1f / _Data.WayCount))) * (_Data.WayCount / 2), Vector3.forward)
																	: Quaternion.AngleAxis (-_Data.GapAngle * (_Data.WayCount / 2), Vector3.forward);
					Vector2 direction = rotation * (GameManager.Player.transform.position - (Vector3)_Enemy.ShootPoint);
					for (int i = 0; i < _Data.WayCount; i++)
					{
						_Enemy.Shot (_Data.Bullet, direction);
						direction = Quaternion.AngleAxis (_Data.GapAngle, Vector3.forward) * direction;
					}

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
						deltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					shotDeltaTime = 0.0f;
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class Laser : IAttackProcesser
		{
			private LaserData _Data;
			private Enemy _Enemy;
			private Subject<Unit> _CompleteObserver;

			public Laser (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as LaserData;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;
				var shotCount = 0;
				while (_Data.ShotCount > shotCount)
				{
					Vector2 direction = Quaternion.AngleAxis (_Data.OriginAngle, Vector3.forward) * new Vector2 (_Enemy.Sign, 0);

					_Enemy.ShotLaser (_Data.Laser, direction);

					while (_Data.Laser.Duration > deltaTime)
					{
						deltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					deltaTime = 0.0f;
					_Enemy.StopLaser (_Data.Laser);

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					shotDeltaTime = 0.0f;
					shotCount++;
					yield return null;
				}

				_Enemy.StopLaser (_Data.Laser);
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class LaserRotation : IAttackProcesser
		{
			private LaserRotationData _Data;
			private Enemy _Enemy;
			private Subject<Unit> _CompleteObserver;

			public LaserRotation (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as LaserRotationData;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;
				var shotCount = 0;
				var originAngle = _Data.OriginAngle;
				while (_Data.ShotCount > shotCount)
				{
					originAngle = _Data.OriginAngle;
					Vector2 direction = Quaternion.AngleAxis (originAngle, Vector3.forward) * new Vector2 (_Enemy.Sign, 0);

					_Enemy.ShotLaser (_Data.Laser, direction);

					while (_Data.Laser.Duration > deltaTime)
					{
						originAngle += 1 * _Data.RotationSpeed.Evaluate (deltaTime) * TimeManager.EnemyDeltaTime;
						direction = Quaternion.AngleAxis (originAngle, Vector3.forward) * new Vector2 (_Enemy.Sign, 0);
						deltaTime += TimeManager.EnemyDeltaTime;
						_Enemy.RotationLaser (_Data.Laser, direction);
						yield return null;
					}
					deltaTime = 0.0f;
					_Enemy.StopLaser (_Data.Laser);

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
					}
					shotDeltaTime = 0.0f;
					shotCount++;
					yield return null;
				}
				_Enemy.StopLaser (_Data.Laser);

				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class LaserTargetPlayer : IAttackProcesser
		{
			private LaserTargetPlayerData _Data;
			private Subject<Unit> _CompleteObserver;
			private Enemy _Enemy;

			public LaserTargetPlayer (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as LaserTargetPlayerData;
				_CompleteObserver = new Subject<Unit> ();
				_Enemy = enemy;
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var shotDeltaTime = 0.0f;
				var shotCount = 0;
				while (_Data.ShotCount > shotCount)
				{
					_Enemy.ShotLaser (_Data.Laser, GameManager.Player.transform.position - _Enemy.transform.position);

					while (_Data.Laser.Duration > deltaTime)
					{
						deltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					deltaTime = 0.0f;
					_Enemy.StopLaser (_Data.Laser);

					while (_Data.ShotInterval > shotDeltaTime)
					{
						shotDeltaTime += TimeManager.EnemyDeltaTime;
					}
					shotDeltaTime = 0.0f;
					shotCount++;
					yield return null;
				}
				_Enemy.StopLaser (_Data.Laser);

				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class EnemyPop : IAttackProcesser
		{
			private Subject<Unit> _CompleteObserver;
			private EnemyPopData _Data;
			private Enemy _Enemy;
			private IEnumerable<GameObject> _PopPoint;

			public EnemyPop (EnemyAttackDataBase data, Enemy enemy)
			{
				_Data = data as EnemyPopData;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();

				_PopPoint = _Enemy.GetComponent<BossBehaviour> ().EnemyPopPoint;
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (_Data.StartTime > deltaTime)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}

				deltaTime = 0.0f;
				var popDeltaTime = 0.0f;
				var count = 0;
				while (_Data.Duration > deltaTime
						|| _Data.Infinit
						|| count < _Data.Count)
				{
					var enemy = GameObject.Instantiate<Enemy> (_Data.PopEnemy, _PopPoint.ElementAt (_Data.PopPoint).transform.position, Quaternion.identity);
					enemy.SpeedRatio = _Data.SpeedRatio;
					count++;
					while (_Data.ShotInterval > popDeltaTime)
					{
						popDeltaTime += TimeManager.EnemyDeltaTime;
						deltaTime += TimeManager.EnemyDeltaTime;
						yield return null;
					}
					popDeltaTime = 0.0f;
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private IEnumerator[] _ProcessList;
		private IAttackProcesser[] _ProcesserList;
		private Subject<Unit> _CompleteObserver;
		private IObservable<Unit> _CompleteStream;

		public EnemyAttackProcesser (IEnumerable<EnemyAttackDataBase> data, Enemy enemy)
		{
			_ProcessList = new IEnumerator[data.Count ()];
			_ProcesserList = new IAttackProcesser[data.Count ()];
			_CompleteObserver = new Subject<Unit> ();
			IAttackProcesser processer;
			for (int i = 0; i < data.Count (); i++)
			{
				switch (data.ElementAt (i).AttackType)
				{
					case AttackType.nWayShot:
						processer = new nWayShot (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.nWayShotTargetPlayer:
						processer = new nWayShotTargetPlayer (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.nWayShotRotation:
						processer = new nWayShotRotation (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.Laser:
						processer = new Laser (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.LaserTargetPlayer:
						processer = new LaserTargetPlayer (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.LaserRotation:
						processer = new LaserRotation (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case AttackType.EnemyPop:
						processer = new EnemyPop (data.ElementAt (i), enemy);
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					default:
						break;
				}
			}

			_CompleteStream = _ProcesserList[0].CompleteAsObservable ();
			for (int i = 1; i < _ProcesserList.Length; i++)
			{
				_CompleteStream = _CompleteStream.Zip (_ProcesserList[i].CompleteAsObservable (), (r, l) => { return l; });
			}
			_CompleteStream
				.First ()
				.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));
		}

		public IObservable<Unit> CompleteAsObservable ()
		{
			return _CompleteObserver;
		}

		public void Process ()
		{
			for (int i = 0; i < _ProcessList.Length; i++)
			{
				_ProcessList[i].MoveNext ();
			}
		}

		public void Initialize ()
		{
			for (int i = 0; i < _ProcesserList.Length; i++)
			{
				_ProcessList[i] = _ProcesserList[i].Process ();
			}
			_CompleteStream = _ProcesserList[0].CompleteAsObservable ();
			for (int i = 1; i < _ProcesserList.Length; i++)
			{
				_CompleteStream = _CompleteStream.Zip (_ProcesserList[i].CompleteAsObservable (), (r, l) => { return l; });
			}
			_CompleteStream
				.First ()
				.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));
		}
	}
}