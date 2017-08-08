using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Data;
	using TalkEvent;

	public enum BehaveOn
	{
		Enter,
		Exit
	}

	public class TalkStartFromAnimationStateBehaviour : ObservableStateMachineTrigger
	{
		[SerializeField]
		private BehaveOn _BehaveOn;

		[SerializeField]
		private TalkEvent _TalkData;

		private IDisposable _TalkStream;

		private void Awake ()
		{
			switch (_BehaveOn)
			{
				case BehaveOn.Enter:
					_TalkStream = this.OnStateEnterAsObservable ()
						.Subscribe (_ =>
						 {
							 TalkEventSystem.Instance.TalkStart (_TalkData.Message);
						 });
					break;

				case BehaveOn.Exit:
					_TalkStream = this.OnStateExitAsObservable ()
						.Subscribe (_ =>
						 {
							 TalkEventSystem.Instance.TalkStart (_TalkData.Message);
						 });
					break;

				default:
					break;
			}
		}

		private void OnDestroy ()
		{
			if (_TalkStream != null) _TalkStream.Dispose ();
		}
	}
}