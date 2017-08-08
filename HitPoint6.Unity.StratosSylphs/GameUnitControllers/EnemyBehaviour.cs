using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using GameUnits;
	using InitializeData;

	public interface IProcesser
	{
		IObservable<Unit> CompleteAsObservable ();

		void Process ();

		void Initialize ();
	}

	public class EnemyBehaviour : MonoBehaviour
	{
		[SerializeField]
		private EnemyActionBehaviourData _Data;

		private void Start ()
		{
			var enemy = GetComponent<Enemy> ();

			var processer = new EnemyProcesser (enemy, _Data);

			this.UpdateAsObservable ()
				.TakeUntil (enemy.LifeAsObservable ().Where (life => life <= 0))
				.Subscribe (_ => processer.Update ());

			#region Hidden

			/*			_ManagerList = new List<IProcesserManager> ();

				//_ManagerList.Add (new _EnemyProcesserManager (enemy, _Data.ActionData));

				for (int i = 0; i < _Data.EnemyActionData.Count(); i++)
				{
					var data = _Data.EnemyActionData.ElementAt (i);
					var manager = new EnemyProcesserManager (enemy, _Data.EnemyActionData.Skip (i).Take (1).Select (_data => _data.ActionData));
					_ManagerList.Add (manager);
					if(data.LoopIndex.LoopCount != 0)
					{
						if (data.LoopIndex.InfinitLoop)
						{
							var infManager = new InfinitLoopEnemyProcesserManager (enemy, _Data.EnemyActionData
																					.Skip ((int)data.LoopIndex.LoopStartIndex)
																					.Take (1 + (int)(data.LoopIndex.LoopEndIndex - data.LoopIndex.LoopStartIndex))
																					.Select (_data => _data.ActionData));
							_ManagerList.Add (infManager);
							break;
						}
						var repManager = new RepeateEnemyProcesserManager ((int)data.LoopIndex.LoopCount, enemy, _Data.EnemyActionData
																											.Skip ((int)data.LoopIndex.LoopStartIndex)
																											.Take (1 + (int)(data.LoopIndex.LoopEndIndex - data.LoopIndex.LoopStartIndex))
																											.Select (_data => _data.ActionData));
						_ManagerList.Add (repManager);
						continue;
					}
				}
				/*LoopIndex loopData;

				if (_Data.LoopIndex.Count () != 0)
				{
					for (int i = 0; i < _Data.LoopIndex.Count (); i++)
					{
						loopData = _Data.LoopIndex.ElementAt (i);
						if (loopData.InfinitLoop)
						{
							_ManagerList.Add (new _InfinitLoopEnemyProcesserManager (enemy, _Data.ActionData
																						.Skip ((int)loopData.LoopStartIndex)
																						.Take (1+(int)(loopData.LoopEndIndex - loopData.LoopStartIndex))));
							break;
						}
						_ManagerList.Add (new _RepeateEnemyProcesserManager ((int)loopData.LoopCount-1, enemy, _Data.ActionData
																												.Skip ((int)loopData.LoopStartIndex)
																												.Take (1+(int)(loopData.LoopEndIndex - loopData.LoopStartIndex))));
					}
				}*/
			/*	_Manager = _ManagerList.GetEnumerator ();
				_Manager.MoveNext ();
				_OnComlete ();
				this.UpdateAsObservable ()
					.TakeUntil(enemy.LifeAsObservable().Where(life => life <= 0))
					.Subscribe (_ =>
					 {
						 _Manager.Current.Update ();
					 });*/

			#endregion Hidden
		}

		private void OnDestroy ()
		{
			_Data = null;
		}
	}

	public class EnemyProcesser
	{
		private List<IProcesserManager> _ManagerList;
		private IEnumerator<IProcesserManager> _Manager;

		public EnemyProcesser (Enemy enemy, EnemyActionBehaviourData actionData)
		{
			_ManagerList = new List<IProcesserManager> ();

			for (int i = 0; i < actionData.EnemyActionData.Count (); i++)
			{
				var data = actionData.EnemyActionData.ElementAt (i);
				var manager = new EnemyProcesserManager (enemy, actionData.EnemyActionData.Skip (i).Take (1).Select (_data => _data.ActionData));
				_ManagerList.Add (manager);
				if (data.LoopIndex.LoopCount != 0)
				{
					if (data.LoopIndex.InfinitLoop)
					{
						var infManager = new InfiniteLoopManager (enemy, data.LoopIndex, actionData);
						_ManagerList.Add (infManager);
						break;
					}
					var repManager = new RepeatManager (enemy, data.LoopIndex, actionData);
					_ManagerList.Add (repManager);
					continue;
				}
			}
			_Manager = _ManagerList.GetEnumerator ();
			_Manager.MoveNext ();
			_OnComlete (enemy);
		}

		public void Update ()
		{
			_Manager.Current.Update ();
		}

		private void _OnComlete (Enemy enemy)
		{
			_Manager.Current.CompleteAsObservable ()
				.First ()
				.Subscribe (_ =>
				{
					if (_Manager.MoveNext ())
					{
						_OnComlete (enemy);
					}
					else
					{
						GameObject.Destroy (enemy.gameObject);
					}
				});
		}

		public class NullAttackProcesser : IProcesser
		{
			private Subject<Unit> _CompleteObserver;

			public NullAttackProcesser ()
			{
				_CompleteObserver = new Subject<Unit> ();
				Observable.NextFrame ()
					.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public void Initialize ()
			{
				Observable.NextFrame ()
					.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));
			}

			public void Process ()
			{
			}
		}

		public class EnemyActionProcesser : IProcesser
		{
			private Subject<Unit> _CompleteObserver;
			private EnemyActionData _Data;
			private Enemy _Enemy;
			private Action _Process;
			private IProcesser _AttackProcesser;
			private IProcesser _MoveProcesser;

			public EnemyActionProcesser (Enemy enemy, EnemyActionData data)
			{
				_Enemy = enemy;
				_Data = data;
				if (data.AttackData.Count () == 0)
				{
					_AttackProcesser = new NullAttackProcesser ();
				}
				else
				{
					_AttackProcesser = new EnemyAttackProcesser (data.AttackData, enemy);
				}
				_MoveProcesser = new EnemyMoveProcesser (enemy, data.MoveData);
				_CompleteObserver = new Subject<Unit> ();
				_AttackProcesser.CompleteAsObservable ()
					.Zip (_MoveProcesser.CompleteAsObservable (), (l, r) => r)
					.First ()
					.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));

				_CompleteObserver.Subscribe (_ => _Process = _FirstProcess);

				_Process = _FirstProcess;
			}

			private void _FirstProcess ()
			{
				_Enemy.AnimationStateChange (_Data.AnimationStateMessage);
				_AttackProcesser.Process ();
				_MoveProcesser.Process ();
				_Process = _ProcessCore;
			}

			private void _ProcessCore ()
			{
				_AttackProcesser.Process ();
				_MoveProcesser.Process ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public void Process ()
			{
				_Process ();
			}

			public override string ToString ()
			{
				return "Processing : " + _Data.name;
			}

			public void Initialize ()
			{
				_Process = _FirstProcess;
				_AttackProcesser.Initialize ();
				_MoveProcesser.Initialize ();
				_AttackProcesser.CompleteAsObservable ()
					.Zip (_MoveProcesser.CompleteAsObservable (), (l, r) => r)
					.First ()
					.Subscribe (_ => _CompleteObserver.OnNext (Unit.Default));
			}
		}

		public interface IProcesserManager
		{
			IObservable<Unit> CompleteAsObservable ();

			void Update ();
		}

		public class EnemyProcesserManager : IProcesserManager
		{
			private IEnumerator<EnemyActionProcesser> _Processers;
			private Subject<Unit> _OnCompleteObserver;

			public EnemyProcesserManager (Enemy enemy, IEnumerable<EnemyActionData> data)
			{
				_OnCompleteObserver = new Subject<Unit> ();
				var processerArray = new EnemyActionProcesser[data.Count ()];
				for (int i = 0; i < processerArray.Length; i++)
				{
					processerArray[i] = new EnemyActionProcesser (enemy, data.ElementAt (i));
				}

				_Processers = processerArray.AsEnumerable ().GetEnumerator ();
				_Processers.MoveNext ();
				_OnComplete ();
			}

			private void _OnComplete ()
			{
				_Processers.Current.CompleteAsObservable ()
					.First ()
					.Subscribe (_ =>
					{
						//二個ストリームを作るとたぶん二回MoveNextが動くのでwhereは使わない
						if (!_Processers.MoveNext ())
						{
							_OnCompleteObserver.OnNext (Unit.Default);
						}
						else
						{
							_OnComplete ();
						}
					});
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _OnCompleteObserver;
			}

			public void Update ()
			{
				_Processers.Current.Process ();
			}
		}

		public class RepeatManager : IProcesserManager
		{
			private IEnumerator<IProcesser> _Processers;
			private Subject<Unit> _CompleteObserver;

			private EnemyActionBehaviourData _Data;

			public RepeatManager (Enemy enemy, LoopIndex loopData, EnemyActionBehaviourData originData)
			{
				_CompleteObserver = new Subject<Unit> ();
				var container = new List<IProcesser> ();
				_SetUp (enemy, loopData, originData, container, 0);
				_Processers = container.GetEnumerator ();
				_Processers.MoveNext ();
				_OnComplete ();
			}

			private void _PutOutError (Enemy enemy, LoopIndex index, EnemyActionBehaviourData data)
			{
				EnemyActionDataWithLoop putData = null;
				for (int i = 0; i < data.EnemyActionData.Count (); i++)
				{
					putData = data.EnemyActionData.ElementAt (i);
					if (putData.LoopIndex == index)
					{
						Debug.LogError (string.Format ("無限ループに陥っていますデータを見直してください :  ObjectName → {0} : DataName → {1} : Index → {2}", enemy.name, putData.ActionData.name, i));
						return;
					}
				}
			}

			private void _SetUp (Enemy enemy, LoopIndex dataIndex, EnemyActionBehaviourData originData, List<IProcesser> container, int loopCount)
			{
				if (loopCount > 1000)
				{
					_PutOutError (enemy, dataIndex, originData);
					return;
				}
				for (int i = 0; i < dataIndex.LoopCount; i++)
				{
					loopCount++;
					if (loopCount > 1000)
					{
						_PutOutError (enemy, dataIndex, originData);
						break;
					}
					var data = originData.EnemyActionData.Skip ((int)dataIndex.LoopStartIndex)
														.Take (1 + (int)(dataIndex.LoopEndIndex - dataIndex.LoopStartIndex));

					container.Add (new EnemyActionProcesser (enemy, data.First ().ActionData));

					for (int y = 1; y < data.Count (); y++)
					{
						loopCount++;
						if (loopCount > 1000)
						{
							_PutOutError (enemy, dataIndex, originData);
							break;
						}

						var loopData = data.ElementAt (y);

						container.Add (new EnemyActionProcesser (enemy, loopData.ActionData));
						if (loopData.LoopIndex.LoopCount != 0)
						{
							_SetUp (enemy, loopData.LoopIndex, originData, container, loopCount);
							continue;
						}
					}
				}
			}

			private void _OnComplete ()
			{
				_Processers.Current.CompleteAsObservable ()
					.First ()
					.Subscribe (_ =>
					 {
						 if (_Processers.MoveNext ())
						 {
							 Debug.Log (_Processers.Current.ToString ());
							 _OnComplete ();
							 return;
						 }
						 else
						 {
							 _CompleteObserver.OnNext (Unit.Default);
						 }
					 });
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public void Update ()
			{
				_Processers.Current.Process ();
			}
		}

		public class RepeatEnemyProcesserManager : IProcesserManager
		{
			private int _CurrentLoopCount = 0;
			private int _LoopCount;
			private IEnumerator<EnemyActionProcesser> _Processers;
			private Subject<Unit> _CompleteObserver = new Subject<Unit> ();
			private Enemy _Enemy;
			private IEnumerable<EnemyActionData> _Data;

			public RepeatEnemyProcesserManager (int loopCount, Enemy enemy, IEnumerable<EnemyActionData> data)
			{
				if (loopCount < 0)
				{
					throw new InvalidOperationException ("LoopCountの値は1以上に設定してください");
				}
				_Enemy = enemy;
				_Data = data;
				_LoopCount = loopCount;
				var array = new EnemyActionProcesser[data.Count ()];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new EnemyActionProcesser (enemy, data.ElementAt (i));
				}
				_Processers = array.AsEnumerable ().GetEnumerator ();
				_Processers.MoveNext ();
				_OnComplete ();
			}

			private void _OnComplete ()
			{
				Debug.Log (_CurrentLoopCount);
				_Processers.Current.CompleteAsObservable ()
					.First ()
					.Subscribe (_ =>
					{
						if (_Processers.MoveNext ())
						{
							_OnComplete ();
							return;
						}
						else if (_LoopCount - 1 > _CurrentLoopCount)
						{
							_CurrentLoopCount++;
							var array = new EnemyActionProcesser[_Data.Count ()];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = new EnemyActionProcesser (_Enemy, _Data.ElementAt (i));
							}
							_Processers = array.AsEnumerable ().GetEnumerator ();
							_Processers.MoveNext ();

							_OnComplete ();

							return;
						}
						else
						{
							_CompleteObserver.OnNext (Unit.Default);
						}
					});
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public void Update ()
			{
				_Processers.Current.Process ();
			}
		}

		public class InfiniteLoopManager : IProcesserManager
		{
			private LoopIndex _Index;
			private RepeatManager _Manager;
			private EnemyActionBehaviourData _Data;
			private Enemy _Enemy;
			private Subject<Unit> _CompleteObserver;

			public InfiniteLoopManager (Enemy enemy, LoopIndex index, EnemyActionBehaviourData data)
			{
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
				_Index = index;
				_Data = data;
				_Manager = new RepeatManager (enemy, index, data);
				_OnComplete ();
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			private void _OnComplete ()
			{
				_Manager.CompleteAsObservable ()
					.Subscribe (_ =>
					 {
						 _Manager = new RepeatManager (_Enemy, _Index, _Data);
						 _OnComplete ();
					 });
			}

			public void Update ()
			{
				_Manager.Update ();
			}
		}

		public class InfinitLoopEnemyProcesserManager : IProcesserManager
		{
			private Subject<Unit> _CompleteObserver;
			private Enemy _Enemy;
			private IEnumerable<EnemyActionData> _Data;
			private IEnumerable<EnemyActionProcesser> _Processer;
			private IEnumerator<EnemyActionProcesser> _ProcesserEnumRator;

			public InfinitLoopEnemyProcesserManager (Enemy enemy, IEnumerable<EnemyActionData> data)
			{
				_Data = data;
				_Enemy = enemy;
				_CompleteObserver = new Subject<Unit> ();
				var processerArray = new EnemyActionProcesser[data.Count ()];
				for (int i = 0; i < processerArray.Length; i++)
				{
					processerArray[i] = new EnemyActionProcesser (enemy, data.ElementAt (i));
				}
				_Processer = processerArray.AsEnumerable ();
				_ProcesserEnumRator = _Processer.GetEnumerator ();
				_ProcesserEnumRator.MoveNext ();
				_OnComplete ();
			}

			private void _OnComplete ()
			{
				_ProcesserEnumRator.Current.CompleteAsObservable ()
					.First ()
					.Subscribe (_ =>
					{
						if (_ProcesserEnumRator.MoveNext ())
						{
							_OnComplete ();
						}
						else
						{
							var processerArray = new EnemyActionProcesser[_Data.Count ()];
							for (int i = 0; i < processerArray.Length; i++)
							{
								processerArray[i] = new EnemyActionProcesser (_Enemy, _Data.ElementAt (i));
							}
							_Processer = processerArray.AsEnumerable ();
							_ProcesserEnumRator = _Processer.GetEnumerator ();
							_ProcesserEnumRator.MoveNext ();
							_OnComplete ();
						}
					});
			}

			public IObservable<Unit> CompleteAsObservable ()
			{
				return _CompleteObserver;
			}

			public void Update ()
			{
				_ProcesserEnumRator.Current.Process ();
			}
		}
	}
}