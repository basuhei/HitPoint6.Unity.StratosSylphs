using System;
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

	public class EnemyMoveProcesser : IProcesser
	{
		private interface IMoveProcesser
		{
			IObservable<Unit> CompleteAsObservable ();

			IEnumerator Process ();
		}

		private interface ISwing
		{
			Vector2 MoveOffset { get; }

			void Update ();
		}

		private abstract class _MoveProcesserBase : IMoveProcesser
		{
			protected Subject<Unit> _CompleteObserver { get; private set; }

			protected Enemy _Enemy { get; private set; }

			protected ISwing _Swing { get; private set; }

			public _MoveProcesserBase (Enemy enemy, EnemyMoveDataBase data)
			{
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
				if (data.SwingData == null)
				{
					_Swing = new _NullSwing ();
				}
				else
				{
					_Swing = new _Swing (data.SwingData);
				}
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public abstract IEnumerator Process ();
		}

		private class _Swing : ISwing
		{
			private float _TotalTime;
			private SwingData _Data;
			private Vector2 _NormalizedDirection;

			public _Swing (SwingData data)
			{
				_Data = data;
				MoveOffset = Vector2.zero;
				_NormalizedDirection = _Data.SwingDirection.normalized;
			}

			public Vector2 MoveOffset
			{
				get;
				private set;
			}

			public void Update ()
			{
				_TotalTime += _Data.SwingSpeed * TimeManager.EnemyDeltaTime;
				MoveOffset = _NormalizedDirection * Mathf.Sin (_TotalTime) * _Data.SwingWidth * _Data.SwingSpeed;
			}
		}

		private class _NullSwing : ISwing
		{
			public Vector2 MoveOffset
			{
				get
				{
					return Vector2.zero;
				}
			}

			public void Update ()
			{
			}
		}

		private class _Enter : _MoveProcesserBase
		{
			private EnterData _Data;
			private Vector2 _TargetDirection;

			public _Enter (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as EnterData;
				_TargetDirection = Vector2.zero;
				var enemyPosition = (Vector2)Camera.main.WorldToViewportPoint (enemy.transform.position);
				if (enemyPosition.x < 0)
				{
					_TargetDirection.x = 1;
				}
				if (enemyPosition.y > 1)
				{
					_TargetDirection.y = -1;
				}
				if (enemyPosition.y < 0)
				{
					_TargetDirection.y = 1;
				}
				if (enemyPosition.x > 1)
				{
					_TargetDirection.x = -1;
				}

				if (_TargetDirection.x != 0 && _TargetDirection.y != 0)
				{
					_TargetDirection = -enemy.transform.position.normalized;
				}
			}

			public override IEnumerator Process ()
			{
				var deltaTime = 0.0f;

				while (deltaTime < _Data.Duration)
				{
					_Swing.Update ();
					_Enemy.Move (_Swing.MoveOffset + _TargetDirection * _Data.Speed * _Data.Easing.Evaluate (deltaTime / _Data.Duration) * _Enemy.SpeedRatio / Mathf.Abs (_Enemy.SpeedRatio));
					deltaTime += TimeManager.EnemyDeltaTime;
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _MoveToPoint : _MoveProcesserBase
		{
			private MoveToPositionData _Data;

			public _MoveToPoint (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as MoveToPositionData;
			}

			public override IEnumerator Process ()
			{
				Vector2 targetDirection = _Data.TargetPosition - (Vector2)_Enemy.transform.position;
				float initialMagnitude = targetDirection.sqrMagnitude;
				float currentMagnitude = initialMagnitude;
				float lastMagnitude = currentMagnitude + 1;
				while (targetDirection.sqrMagnitude > 0.2f && lastMagnitude - currentMagnitude >= 0)
				{
					lastMagnitude = currentMagnitude;
					_Swing.Update ();
					targetDirection = _Data.TargetPosition - (Vector2)_Enemy.transform.position;
					_Enemy.Move (_Swing.MoveOffset + targetDirection.normalized * _Data.Easing.Evaluate ((initialMagnitude - targetDirection.sqrMagnitude) / initialMagnitude));
					currentMagnitude = targetDirection.sqrMagnitude;
					yield return null;
				}
				_Enemy.MoveDirectory (_Data.TargetPosition);
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _Linear : _MoveProcesserBase
		{
			private LinearMoveData _Data;

			public _Linear (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as LinearMoveData;
			}

			public override IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (deltaTime < _Data.Duration)
				{
					_Swing.Update ();
					deltaTime += TimeManager.EnemyDeltaTime;
					_Enemy.Move (_Swing.MoveOffset + _Data.Direction * _Data.Easing.Evaluate (deltaTime / _Data.Duration));
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _Exit : _MoveProcesserBase
		{
			private ExitData _Data;

			public _Exit (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as ExitData;
			}

			private bool IsOutOfBoder
			{
				get
				{
					var viewportPositon = (Vector2)Camera.main.WorldToViewportPoint (_Enemy.transform.position);
					return viewportPositon.x > 1.2f
						|| viewportPositon.y > 1.2f
						|| viewportPositon.x < -0.2f
						|| viewportPositon.y < -0.2f;
				}
			}

			private Vector2 _ToLeaveCenter
			{
				get
				{
					if (((Vector2)_Enemy.transform.position) == Vector2.zero)
					{
						return new Vector2 (0, 1);
					}
					return -(Vector2.zero - (Vector2)_Enemy.transform.position).normalized;
				}
			}

			public override IEnumerator Process ()
			{
				while (!IsOutOfBoder)
				{
					_Swing.Update ();
					_Enemy.Move (_Swing.MoveOffset + _ToLeaveCenter * _Data.Speed * _Enemy.SpeedRatio / Mathf.Abs (_Enemy.SpeedRatio));
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _ApproachToPlayer : _MoveProcesserBase
		{
			private ApproachPlayerData _Data;

			private float _SqrDistance
			{
				get
				{
					return (GameManager.Player.transform.position - _Enemy.transform.position).sqrMagnitude;
				}
			}

			private Vector2 ToPlayer
			{
				get
				{
					return GameManager.Player.transform.position - _Enemy.transform.position;
				}
			}

			private bool _Approached
			{
				get
				{
					return _SqrDistance < _Data.AproachDistance * _Data.AproachDistance;
				}
			}

			public _ApproachToPlayer (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as ApproachPlayerData;
			}

			public override IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				while (deltaTime < _Data.Duration && !_Approached)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					_Swing.Update ();
					_Enemy.Move (_Swing.MoveOffset + ToPlayer.normalized * _Data.Easing.Evaluate (deltaTime / _Data.Duration));
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _AssaultToPlayer : _MoveProcesserBase
		{
			private AssaultToPlayerData _Data;

			public _AssaultToPlayer (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as AssaultToPlayerData;
			}

			public override IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				Vector2 velocity = GameManager.Player.transform.position - _Enemy.transform.position;
				velocity.Normalize ();
				while (deltaTime < _Data.Duration)
				{
					deltaTime += TimeManager.EnemyDeltaTime;
					_Swing.Update ();
					_Enemy.Move (_Swing.MoveOffset + velocity * _Data.Easing.Evaluate (deltaTime / _Data.Duration));
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private class _RandomLinear : _MoveProcesserBase
		{
			private RandomLinearData _Data;

			public _RandomLinear (Enemy enemy, EnemyMoveDataBase data) : base (enemy, data)
			{
				_Data = data as RandomLinearData;
			}

			public override IEnumerator Process ()
			{
				var deltaTime = 0.0f;
				var range = _Data.Range;
				var x = UnityEngine.Random.Range (range.LowX, range.HighX);
				var y = UnityEngine.Random.Range (range.LowY, range.HighY);
				var velocity = new Vector2 (x, y);
				var speed = UnityEngine.Random.Range (range.LowSpeed, range.HighSpeed);
				velocity.Normalize ();

				while (deltaTime < _Data.Duration)
				{
					_Swing.Update ();
					deltaTime += TimeManager.EnemyDeltaTime;
					_Enemy.Move (_Swing.MoveOffset + velocity * speed * _Data.Easing.Evaluate (deltaTime / _Data.Duration));
					yield return null;
				}
				_CompleteObserver.OnNext (Unit.Default);
			}
		}

		private IEnumerator[] _ProcessList;
		private IMoveProcesser[] _ProcesserList;
		private int _CurrentIndex;//うーん
		private Subject<Unit> _CompleteObserver;
		private IObservable<Unit> _CompleteStream;

		public EnemyMoveProcesser (Enemy enemy, IEnumerable<EnemyMoveDataBase> data)
		{
			_ProcessList = new IEnumerator[data.Count ()];
			_ProcesserList = new IMoveProcesser[data.Count ()];
			IMoveProcesser processer;

			for (int i = 0; i < data.Count (); i++)
			{
				switch (data.ElementAt (i).Type)
				{
					case MoveType.Linear:
						processer = new _Linear (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.MoveToPoint:
						processer = new _MoveToPoint (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.AssaultToPlayer:
						processer = new _AssaultToPlayer (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.ApproachPlayer:
						processer = new _ApproachToPlayer (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.Exit:
						processer = new _Exit (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.Enter:
						processer = new _Enter (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					case MoveType.RandomLinear:
						processer = new _RandomLinear (enemy, data.ElementAt (i));
						_ProcessList[i] = processer.Process ();
						_ProcesserList[i] = processer;
						break;

					default:
						break;
				}
			}

			_CompleteObserver = new Subject<Unit> ();
			_CompleteStream = _ProcesserList[0].CompleteAsObservable ();
			for (int i = 1; i < _ProcesserList.Length; i++)
			{
				_CompleteStream = _CompleteStream.Zip (_ProcesserList[i].CompleteAsObservable (), (r, l) => { return l; });
			}
			_CompleteStream
				.First ()
				.Subscribe (_ =>
				{
					_CompleteObserver.OnNext (Unit.Default);
				});
		}

		public IObservable<Unit> CompleteAsObservable ()
		{
			return _CompleteObserver;
		}

		public void Process ()
		{
			if (!_ProcessList[_CurrentIndex].MoveNext ())
			{
				_CurrentIndex++;
				_CurrentIndex = Math.Min (_CurrentIndex, _ProcessList.Count () - 1);
			}
		}

		public void Initialize ()
		{
			_CurrentIndex = 0;

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
				.Subscribe (_ =>
				{
					_CompleteObserver.OnNext (Unit.Default);
				});
		}
	}
}