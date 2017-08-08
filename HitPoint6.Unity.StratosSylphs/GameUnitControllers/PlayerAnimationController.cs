using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Constants;
	using GameUnits;
	using Library.Method;

	[Serializable]
	public class PlayerAnimationController
	{
		private enum _Horizontal
		{
			Left = -1,
			Stop = 0,
			Right = 1
		}

		private Animator _SpeedModeAnimator;
		private List<Animator> _ReloadAnimator;

		[SerializeField, Header ("飛行機雲")]
		private GameObject[] _Contrail;

		[SerializeField, Header ("左にいくとオフ右に行くとオンに今のところはしている")]
		private GameObject[] _Effects;

		[SerializeField]
		private GameObject _Magazine;

		public void Start (Player player)
		{
			_SpeedModeAnimator = player.GetComponentsInChildren<Animator> (true)
							   .FirstOrDefault (animatior => animatior.gameObject.name == GameObjectPath.PlayerLowerBody);
			_ReloadAnimator = player.GetComponentsInChildren<Animator> (true)
							   .Where (animatior => animatior.gameObject.name == GameObjectPath.RightAimMode || animatior.gameObject.name == GameObjectPath.LeftAimMode).ToList ();

			player.FixedUpdateAsObservable ()
				.Where (_ => _SpeedModeAnimator.gameObject.activeInHierarchy)
				.Select (_ => player.transform.position)
				.Buffer (2, 1)
				.Where (b => b.Count == 2)
				.Select (b => b[1] - b[0])
				.Subscribe (moveAmount =>
				{
					_SpeedModeAnimator.SetInteger (AnimationParams.Vertical, (int)MathfExtentions.Sign0 (moveAmount.y));
					switch ((_Horizontal)(int)MathfExtentions.Sign0 (moveAmount.x))
					{
						case _Horizontal.Left:
							for (int i = 0; i < _Effects.Length; i++)
							{
								_Effects[i].SetActive (false);
							}
							if (moveAmount.y > 0)
							{
								for (int i = 0; i < _Contrail.Length; i++)
								{
									_Contrail[i].SetActive (true);
								}
								break;
							}
							for (int i = 0; i < _Contrail.Length; i++)
							{
								_Contrail[i].SetActive (false);
							}
							break;

						case _Horizontal.Stop:
							for (int i = 0; i < _Effects.Length; i++)
							{
								_Effects[i].SetActive (true);
							}
							for (int i = 0; i < _Contrail.Length; i++)
							{
								_Contrail[i].SetActive (true);
							}
							break;

						case _Horizontal.Right:
							for (int i = 0; i < _Effects.Length; i++)
							{
								_Effects[i].SetActive (true);
							}
							for (int i = 0; i < _Contrail.Length; i++)
							{
								_Contrail[i].SetActive (true);
							}
							break;

						default:
							break;
					}
				});

			var leftMagazinePosition = player.GetComponentsInChildren<Transform> (true).FirstOrDefault (@object => @object.name == "MagazinePositionLeft");
			var rightMagazinePosition = player.GetComponentsInChildren<Transform> (true).FirstOrDefault (@object => @object.name == "MagazinePositionRight");

			player.Controller.FiringController.ReloadAsObservable ()
				.ThrottleFrame (1)
				.Subscribe (_ =>
				{
					for (int i = 0; i < _ReloadAnimator.Count; i++)
					{
						if (!_ReloadAnimator[i].gameObject.activeInHierarchy) { continue; }
						_ReloadAnimator[i].SetTrigger ("Reload");
					}

					foreach (var animator in _ReloadAnimator
											.Where (animator => animator.gameObject.activeInHierarchy))
					{
						animator.SetTrigger ("Reload");
					}

					if (player.Controller.StateController.BodyDirection == BodyDirection.Left)
					{
						MonoBehaviour.Instantiate (_Magazine, leftMagazinePosition.transform.position, Quaternion.identity);
					}
					else
					{
						MonoBehaviour.Instantiate (_Magazine, rightMagazinePosition.transform.position, Quaternion.identity);
					}
				});

			player.UpdateAsObservable ()
				.First ()
				.Subscribe (__ =>
				{
					var spriteRenderes = player.GetComponentsInChildren<SpriteRenderer> (true);
					Coroutine coroutine = null;
					player.Controller.ColliderController
						.OnInvincibleAsObservable
						.Subscribe (_ =>
						 {
							 /*_SpeedModeAnimator.SetBool ("Damage", true);
							 _ReloadAnimator.ToList ()
							 .ForEach (animator => animator.SetBool ("Damage", true));*/
							 coroutine = player.StartCoroutine (_SpriteFlashing (spriteRenderes));
						 });

					player.Controller.ColliderController
					.OnInvincibleEndAsObservable
					.Subscribe (_ =>
					 {
						 /*_SpeedModeAnimator.SetBool ("Damage", false);
						 _ReloadAnimator
						 .ForEach (animator => animator.SetBool ("Damage", false));*/
						 if (coroutine != null) player.StopCoroutine (coroutine);
						 foreach (var s in spriteRenderes)
						 {
							 s.enabled = true;
							 s.color = Color.white;
						 }
					 });
				});
		}

		private IEnumerator _SpriteFlashing (SpriteRenderer[] sprites)
		{
			foreach (var s in sprites)
			{
				s.color = new Color (1, 1, 1, 0.6f);
			}

			while (true)
			{
				if (Time.frameCount % 30 == 0)
				{
					foreach (var s in sprites)
					{
						s.color = s.color.a == 0.6f ? new Color (1, 1, 1, 1.0f) : new Color (1, 1, 1, 0.6f);

						//s.enabled = !s.enabled;
					}
				}
				yield return null;
			}
		}
	}
}