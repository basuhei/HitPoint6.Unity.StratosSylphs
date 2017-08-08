using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	[Serializable]
	public class RemainingBulletAnimatorMessager
	{
		[SerializeField]
		private Animator _Animator;

		[SerializeField]
		private string _BulletCountParamName;

		[SerializeField]
		private string _ReloadTriggerName;

		public Animator Animator { get { return _Animator; } }

		public string BulletCountParamName { get { return _BulletCountParamName; } }

		public string ReloadTriggerName { get { return _ReloadTriggerName; } }
	}

	public class RemaningBulletGauge : MonoBehaviour
	{
		[SerializeField]
		private Image _Image;

		[SerializeField]
		private RemainingBulletAnimatorMessager _AnimationMassenger;

		private uint _MaxBullet;

		private void Start ()
		{
			GameManager.Player.Controller.FiringController.ReminingBulletCount ()
				.First ()
				.Subscribe (count => _MaxBullet = count);

			GameManager.Player.Controller.FiringController.ReminingBulletCount ()
				.AsObservable ()
				.Subscribe (count =>
				{
					_AnimationMassenger.Animator.SetInteger (_AnimationMassenger.BulletCountParamName, (int)count);
					_Image.fillAmount = (float)count / (float)_MaxBullet;
				});

			GameManager.Player.Controller.FiringController.ReminingBulletCount ()
				.Skip (1)
				.Where (count => _MaxBullet == count)
				.Subscribe (_ =>
				{
					_AnimationMassenger.Animator.SetTrigger (_AnimationMassenger.ReloadTriggerName);
				});
		}
	}
}