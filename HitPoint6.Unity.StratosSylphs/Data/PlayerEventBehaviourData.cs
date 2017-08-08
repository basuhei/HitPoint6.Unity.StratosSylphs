using System;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	using Library.Method;

	[Serializable]
	public class PlayerEventParams
	{
		[SerializeField]
		private Vector2 _Direction;

		[SerializeField]
		private float _OparatingTime;

		[SerializeField]
		private AnimationCurve _Easing;

		public Vector2 Direction
		{
			get { return _Direction; }
		}

		public float OparatingTime
		{
			get
			{
				return _OparatingTime;
			}
		}

		public AnimationCurve Easing
		{
			get { return _Easing; }
		}
	}

	[CreateAssetMenu]
	public class PlayerEventBehaviourData : ScriptableObject
	{
		[SerializeField]
		private PlayerEventParams[] _EventData;

		public PlayerEventParams[] EventData
		{
			get { return _EventData; }
		}

		private void OnValidate ()
		{
			foreach (var e in _EventData)
			{
				e.Easing.Clamp01KyeTime ();
			}
		}
	}
}