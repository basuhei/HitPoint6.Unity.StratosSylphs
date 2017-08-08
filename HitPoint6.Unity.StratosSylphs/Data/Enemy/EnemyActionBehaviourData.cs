using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[Serializable]
	public class LoopIndex
	{
		[SerializeField]
		private uint _LoopStartIndex;

		[SerializeField]
		private uint _LoopEndIndex;

		[SerializeField]
		private uint _LoopCount;

		[SerializeField, Header ("無限ループ、ここから先には行かないから注意してね")]
		private bool _InfinitLoop;

		public uint LoopStartIndex
		{
			get { return _LoopStartIndex; }
		}

		public uint LoopEndIndex
		{
			get { return _LoopEndIndex; }
		}

		public uint LoopCount
		{
			get { return _LoopCount; }
		}

		public bool InfinitLoop { get { return _InfinitLoop; } }
	}

	[Serializable]
	public class EnemyActionDataWithLoop
	{
		[SerializeField]
		private LoopIndex _LoopIndex;

		[SerializeField]
		private EnemyActionData _ActionData;

		public LoopIndex LoopIndex { get { return _LoopIndex; } }

		public EnemyActionData ActionData { get { return _ActionData; } }
	}

	[Serializable]
	public class EnemyActionBehaviourData
	{
		[SerializeField]
		private EnemyActionDataWithLoop[] _EnemyActionData;

		public IEnumerable<EnemyActionDataWithLoop> EnemyActionData { get { return _EnemyActionData; } }
	}
}