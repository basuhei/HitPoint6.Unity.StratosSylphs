using UniRx;
using UniRx.Triggers;

namespace HitPoint6.Unity.StratosSylphs.Weapon.Missile
{
	public class MissileStateBehaviour : ObservableStateMachineTrigger
	{
		private void Awake ()
		{
			this.OnStateExitAsObservable ()
				.Subscribe (info =>
				 {
					 info.Animator.gameObject.SetActive (false);
				 });
		}
	}
}