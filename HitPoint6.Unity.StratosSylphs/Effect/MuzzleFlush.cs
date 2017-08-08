using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	using GameUnits;
	using Managers;

	public class MuzzleFlush : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem _System;

		[SerializeField]
		private Player _Player;

		private void Start ()
		{
			_Player.Controller.FiringController.ShotAsObservable ()
				.Subscribe (_ =>
				 {
					 _System.Clear (true);
					 var shootPoint = _Player.Controller.StateController.ShootPoint.transform.position;

					 var direction = (UIManager.TargetSight.transform.position - shootPoint).normalized;
					 transform.position = shootPoint;
					 transform.rotation = Quaternion.FromToRotation (Vector3.right, direction);
					 _System.Play (true);
				 });
		}
	}
}