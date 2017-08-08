using System;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Managers;

	[DisallowMultipleComponent]
	public class AnimatorSpeedController : MonoBehaviour
	{
		private IDisposable _SpeedControllStream;

		private void Start ()
		{
			var animator = transform.root.GetComponentsInChildren<Animator> (true);
			_SpeedControllStream = this.ObserveEveryValueChanged (_ => TimeManager.EnemyTimeScale)
				   .Subscribe (timeScale =>
					{
						for (int i = 0; i < animator.Length; i++)
						{
							animator[i].speed = timeScale;
						}
					});
		}

		public void ControllStart ()
		{
			var animator = transform.root.GetComponentsInChildren<Animator> (true);

			_SpeedControllStream = this.ObserveEveryValueChanged (_ => TimeManager.EnemyTimeScale)
				.Subscribe (timeScale =>
				 {
					 for (int i = 0; i < animator.Length; i++)
					 {
						 animator[i].speed = timeScale;
					 }
				 });
		}

		public void ControllStop ()
		{
			_SpeedControllStream.Dispose ();

			var animator = transform.root.GetComponentsInChildren<Animator> (true);

			for (int i = 0; i < animator.Length; i++)
			{
				animator[i].speed = 1.0f;
			}
		}
	}
}