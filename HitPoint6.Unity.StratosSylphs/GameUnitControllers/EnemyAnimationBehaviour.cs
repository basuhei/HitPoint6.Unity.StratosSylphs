using UniRx;
using UniRx.Triggers;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	public class EnemyAnimationBehaviour : ObservableStateMachineTrigger
	{
		private void Awake ()
		{
			this.OnStateExitAsObservable ()
				.Subscribe (info =>
				 {
					 Destroy (info.Animator.gameObject);
				 });
		}
	}
}