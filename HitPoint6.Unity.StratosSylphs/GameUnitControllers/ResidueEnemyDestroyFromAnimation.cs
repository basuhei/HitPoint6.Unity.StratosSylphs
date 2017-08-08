using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Constants;

	public class ResidueEnemyDestroyFromAnimation : ObservableStateMachineTrigger
	{
		[SerializeField]
		private BehaveOn _BehaveOn;

		[SerializeField]
		private string _EnemyTag;

		[SerializeField]
		private string _DestroyTrigger;

		private void Awake ()
		{
			Debug.Log ("Awake");
			switch (_BehaveOn)
			{
				case BehaveOn.Enter:
					this.OnStateEnterAsObservable ()
						.Subscribe (_ =>
						 {
							 EnemiesDestroyProcess ();
						 });

					break;

				case BehaveOn.Exit:
					this.OnStateExitAsObservable ()
						.Subscribe (_ =>
						{
							EnemiesDestroyProcess ();
						});
					break;

				default:
					break;
			}
		}

		private void EnemiesDestroyProcess ()
		{
			Debug.Log ("destoryProcessStart");
			var tag = string.IsNullOrEmpty (_EnemyTag) ? Tags.Enemies : _EnemyTag;
			var enemies = GameObject.FindGameObjectsWithTag (tag);
			var destroyTrigger = string.IsNullOrEmpty (_DestroyTrigger) ? AnimationParams.Dead : _DestroyTrigger;
			foreach (var a in enemies.Select (g => g.GetComponent<Animator> ()))
			{
				a.gameObject.GetComponent<AnimatorSpeedController> ().ControllStop ();
				a.SetTrigger (_DestroyTrigger);
			}
		}
	}
}