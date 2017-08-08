using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Constants;
	using GameUnits;
	using Library.DeviceInput.Constants;
	using Library.Method;
	using Managers;

	public enum BodyDirection
	{
		Left,
		Right
	}

	[Serializable]
	public class PlayerStateController
	{
		[SerializeField]
		private GameObject _AimModeObject;

		[SerializeField]
		private GameObject _SpeedModeObject;

		public GameObject ShootPoint
		{
			get;
			private set;
		}

		public BodyDirection BodyDirection { get; private set; }

		private ReactiveProperty<bool> _IsAimMode;

		private bool _Reloading = false;

		public ReadOnlyReactiveProperty<bool> IsAimMode
		{
			get;
			private set;
		}

		public PlayerStateController ()
		{
			_IsAimMode = new ReactiveProperty<bool> (false);
			IsAimMode = _IsAimMode.ToReadOnlyReactiveProperty ();
		}

		public void Awake (Player player)
		{
			_AimModeObject = UnityEngine.Object.Instantiate (_AimModeObject, player.transform.position, Quaternion.identity) as GameObject;
			player.AddChild (_AimModeObject);
			ShootPoint = _SpeedModeObject.GetComponentsInChildren<Transform> ()
													.First (t => t.name == GameObjectPath.SpeedModeGun).gameObject;
		}

		public void Start (Player player)
		{
			var children = _AimModeObject.GetComponentsInChildren<Transform> ();

			var leftAim = children.First (t => t.name == GameObjectPath.LeftAimMode).gameObject;

			var rightAim = children.First (t => t.name == GameObjectPath.RightAimMode).gameObject;

			var leftGun = children.First (t => t.name == GameObjectPath.LeftGun).gameObject;

			var rightGun = children.First (t => t.name == GameObjectPath.RightGun).gameObject;

			var shootPointOrigin = _SpeedModeObject.GetComponentsInChildren<Transform> ()
													.First (t => t.name == GameObjectPath.SpeedModeGun).gameObject;

			_AimModeObject.SetActive (false);
			leftAim.SetActive (true);
			rightAim.SetActive (false);

			DefineInput (player);
			DefineSwitchAimMode (player, _AimModeObject, _SpeedModeObject, leftGun, rightGun, shootPointOrigin);
			DefineSwitchLeftAndRight (player, rightAim, leftAim, leftGun, rightGun);
		}

		private void DefineSwitchAimMode (Player player, GameObject aimModeObj, GameObject speedMode, GameObject leftGun, GameObject rightGun, GameObject speedGun)
		{
			player.UpdateAsObservable ()
				.Where (_ => !_Reloading)
				.Where (_ => IsAimMode.Value)
				.Subscribe (_ =>
				{
					speedMode.SetActive (false);
					aimModeObj.SetActive (true);
					if (UIManager.TargetSight.transform.position.x - player.transform.position.x < 0f)
					{
						ShootPoint = leftGun;
					}
					else
					{
						ShootPoint = rightGun;
					}
				});

			player.UpdateAsObservable ()
				.Where (_ => !_Reloading)
				.Where (_ => !IsAimMode.Value)
				.Subscribe (_ =>
				{
					speedMode.SetActive (true);
					aimModeObj.SetActive (false);
					ShootPoint = speedGun;
				});

			player.LateUpdateAsObservable ()
				.Where (_ => !player.CanControl)
				.Subscribe (_ =>
				{
					_IsAimMode.Value = false;
				});

			player.LateUpdateAsObservable ()
				.Where (_ => player.Controller.BombController.LockOnMode)
				.Subscribe (_ =>
				 {
					 _IsAimMode.Value = false;
				 });
		}

		private void DefineSwitchLeftAndRight (Player player, GameObject rightAim, GameObject leftAim, GameObject leftGun, GameObject rightGun)
		{
			var updateStream = player.UpdateAsObservable ().Publish ().RefCount ();

			updateStream
				.Where (_ => rightAim.activeInHierarchy)
				.Where (_ => UIManager.TargetSight.transform.position.x - player.transform.position.x < -0.4f)
				.Subscribe (_ =>
				{
					rightAim.SetActive (false);
					leftAim.SetActive (true);
					ShootPoint = leftGun;
					BodyDirection = BodyDirection.Left;
				});

			updateStream
				.Where (_ => leftAim.activeInHierarchy)
				.Where (_ => UIManager.TargetSight.transform.position.x - player.transform.position.x > 0.15f)
				.Subscribe (_ =>
				{
					leftAim.SetActive (false);
					rightAim.SetActive (true);
					ShootPoint = rightGun;
					BodyDirection = BodyDirection.Right;
				});

			player.Controller.FiringController.ReloadAsObservable ()
				.Subscribe (_ =>
				{
					_Reloading = true;
					player.LateUpdateAsObservable ()
						.Where (__ => _Reloading)
						.TakeUntil (player.Controller.FiringController.ReminingBulletCount ().Skip (1))
						.Subscribe (__ =>
						{
							_IsAimMode.Value = true;
							_SpeedModeObject.SetActive (false);
							_AimModeObject.SetActive (true);
						});
					player.Controller.FiringController.ReminingBulletCount ()
					.Skip (1)
					.First ()
					.Subscribe (__ =>
					{
						_Reloading = false;
					});
				});
		}

		private void DefineInput (Player player)
		{
			/*InputObservable.Instance.OnButtonHoldUpAsObservable (ButtonName.Aim)
				.Where(_ => !_Reloading)
				.Where(__ => !player.Controller.BombController.LockOnMode)
				.Where (__ => player.CanControl)
				.Subscribe (__ =>
				{
					_IsAimMode.Value = true;
				})
				.AddTo (player);

			InputObservable.Instance.OnButtonDownAsObservable (ButtonName.Aim)
				.Where (_ => !_Reloading)
				.Where (__ => player.CanControl)
				.Subscribe (__ =>
				{
					_IsAimMode.Value = false;
				})
				.AddTo (player);*/

			var moveStream = player.UpdateAsObservable ()
				.Select (_ => new Vector2 (Input.GetAxisRaw (AxisName.HORIZONTAL), Input.GetAxisRaw (AxisName.VERTICAL)))
				.Publish ().RefCount ();

			moveStream
				.Where (input => input == Vector2.zero)
				.Subscribe (_ => _IsAimMode.Value = true);

			moveStream
				.Where (input => input != Vector2.zero)
				.Subscribe (_ => _IsAimMode.Value = false);
		}
	}
}