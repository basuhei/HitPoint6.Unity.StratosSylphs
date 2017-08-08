using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using TalkEvent;

	public class EscapementFromAnimation : ObservableStateMachineTrigger
	{
		[SerializeField]
		private BehaveOn _BehaveType;

		private void Awake ()
		{
			switch (_BehaveType)
			{
				case BehaveOn.Enter:
					this.OnStateEnterAsObservable ()
						.Subscribe (_ =>
						 {
							 TalkEventSystem.Instance.Escapement ();
						 });
					break;

				case BehaveOn.Exit:
					this.OnStateExitAsObservable ()
						.Subscribe (_ =>
						 {
							 TalkEventSystem.Instance.Escapement ();
						 });
					break;

				default:
					break;
			}
		}
	}
}