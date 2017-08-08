using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Data;
	using GameUnits;
	using InitializeData;
	using Managers;
	using TalkEvent;

	[Serializable]
	public class EscapeData
	{
		[SerializeField]
		private float _LimitTime;

		[SerializeField]
		private string _EscapeAnimationMessage;

		[SerializeField]
		private bool _IsEscape;

		public float LimitTime { get { return _LimitTime; } }

		public string EscapeAnimationMessage { get { return _EscapeAnimationMessage; } }

		public bool IsEscape { get { return _IsEscape; } }
	}

	[Serializable]
	public class BehaviourSwitchData
	{
		[SerializeField, Range (0, 1)]
		private float _HPRatio;

		[SerializeField]
		private EnemyActionBehaviourData _Data;

		[SerializeField]
		private EscapeData _EscapeData;

		public float HPRatio { get { return _HPRatio; } }

		public EnemyActionBehaviourData Data { get { return _Data; } }

		public EscapeData Escape { get { return _EscapeData; } }
	}

	public class BossBehaviour : MonoBehaviour
	{
		[SerializeField]
		private EnemyActionBehaviourData _FirstProcessData;

		[SerializeField]
		private BehaviourSwitchData[] _SwitchData;

		private Dictionary<BehaviourSwitchData, EnemyProcesser> _ProcesserDictionary;

		private EnemyProcesser _CurrentProcesser;

		private TalkEventSystem _TalkEventSystem;

		[SerializeField]
		private GameObject[] _EnemyPopPoint;

		[SerializeField]
		private TalkEvent _EventData;

		[SerializeField]
		private TalkEvent _EscapeEventData;

		public IEnumerable<GameObject> EnemyPopPoint { get { return _EnemyPopPoint; } }

		private void Start ()
		{
			_TalkEventSystem = TalkEventSystem.Instance;

			_ProcesserDictionary = new Dictionary<BehaviourSwitchData, EnemyProcesser> ();
			var orderedSwitchData = _SwitchData.OrderByDescending (data =>
			 {
				 return data.HPRatio;
			 });

			var enemy = GetComponent<Enemy> ();

			_CurrentProcesser = new EnemyProcesser (enemy, _FirstProcessData);

			foreach (var s in orderedSwitchData.Take (orderedSwitchData.Count () - 1))
			{
				//ラムダキャプチャ対策
				var switchData = s;
				enemy.Life
					.Where (life => ((float)life / (float)enemy.MaxLife) < switchData.HPRatio)
					.First ()
					.Subscribe (_ =>
					 {
						 _CurrentProcesser = _ProcesserDictionary[switchData];
						 if (switchData.Escape.IsEscape)
						 {
							 enemy.StartCoroutine (_EscapeTimer (enemy, switchData));
						 }
					 });
				_ProcesserDictionary.Add (switchData, new EnemyProcesser (enemy, switchData.Data));
			}

			var lastSwitchData = orderedSwitchData.Last ();

			//死亡時の処理
			enemy.Life
				.Where (life => (float)life / (float)enemy.MaxLife < lastSwitchData.HPRatio)
				.First ()
				.Subscribe (life =>
				 {
					 if(life > 0)UIManager.Score.AddScore (GetComponent<Enemy> ().Score);
					 _CurrentProcesser = _ProcesserDictionary[lastSwitchData];
					 if (lastSwitchData.Escape.IsEscape)
					 {
						 enemy.StartCoroutine (_EscapeTimer (enemy, lastSwitchData));
					 }

					 foreach (var c in transform.root.GetComponentsInChildren<AnimatorSpeedController> (true))
					 {
						 c.ControllStop ();
					 }

					 //_TalkEventSystem.TalkStart (_EventData.Message); アニメーションからにした
				 });
			_ProcesserDictionary.Add (lastSwitchData, new EnemyProcesser (enemy, lastSwitchData.Data));

			this.UpdateAsObservable ()
				.TakeUntil (enemy.Life.Where (life => life < 0))
				.Subscribe (_ => _CurrentProcesser.Update ());
		}

		private IEnumerator _EscapeTimer (Enemy enemy, BehaviourSwitchData data)
		{
			var deltaTime = 0.0f;
			while (data.Escape.LimitTime > deltaTime)
			{
				yield return null;
				deltaTime += TimeManager.EnemyDeltaTime;
			}
			enemy.AnimationStateChange (data.Escape.EscapeAnimationMessage);

			//_TalkEventSystem.TalkStart (_EscapeEventData.Message);アニメーションステートビヘイビアに移した
		}
	}
}