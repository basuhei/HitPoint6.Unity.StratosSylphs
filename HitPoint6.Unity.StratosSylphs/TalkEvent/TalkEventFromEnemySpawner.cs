using System;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.TalkEvent
{
	using Data;
	using Managers;

	[Serializable]
	public class TalkEventSet
	{
		[SerializeField]
		private EnemySpawnerManager _Spawner;

		[SerializeField]
		private TalkEvent _Event;

		[SerializeField]
		private float _Delay;

		public EnemySpawnerManager Spawner { get { return _Spawner; } }

		public TalkEvent Event { get { return _Event; } }

		public float Delay { get { return _Delay; } }
	}

	public class TalkEventFromEnemySpawner : MonoBehaviour
	{
		[SerializeField]
		private TalkEventSystem _System;

		[SerializeField]
		private TalkEventSet[] _Event;

		private void Awake ()
		{
			if (_System == null)
			{
				_System = FindObjectOfType<TalkEventSystem> ();
				if (_System == null)
				{
					Debug.LogWarning ("TalkEventSystemがシーン上にも存在しません");
				}
				Debug.LogWarning ("TalkEventSystemがアタッチされていません");
			}
		}

		private void Start ()
		{
			foreach (var set in _Event)
			{
				var talk = set.Event;
				set.Spawner
					.AllEnemyDestroiedAsObservable
					.Delay (TimeSpan.FromSeconds (set.Delay))
					.Subscribe (_ => _System.TalkStart (talk.Message));
			}
		}
	}
}