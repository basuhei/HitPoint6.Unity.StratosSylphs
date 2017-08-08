using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.TalkEvent
{
	public enum TalkEventState
	{
		Start,
		End
	}

	public class AnimationCompleteObserver : StateMachineBehaviour
	{
		[SerializeField]
		private TalkEventState _State;

		private TalkEventSystem _System;

		private void Awake ()
		{
		}

		public override void OnStateExit (Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
		{
			if (_System == null)
			{
				_System = animator.transform.root.GetComponentInChildren<TalkEventSystem> ();
			}
			_System.UIAnimationCallBack (_State);
		}
	}
}