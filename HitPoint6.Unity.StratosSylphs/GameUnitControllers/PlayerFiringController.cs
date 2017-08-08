using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Constants;
	using GameUnits;
	using InitializeData;
	using Library.DeviceInput;
	using Managers;

	[Serializable]
	public class PlayerFiringController
	{
		private IObservableShooter _MainWepon;

		[SerializeField]
		private GunData _GunData;

		private Bounds _InsidePlayer;

		private Vector3 _BoundsOffset;

		public void Awake (Player player)
		{
			var collider = player.GetComponent<BoxCollider2D> ();
			_InsidePlayer = collider.bounds;
			_BoundsOffset = collider.offset;
			_InsidePlayer.size = new Vector3 (_InsidePlayer.size.x, _InsidePlayer.size.y, 10);
			_MainWepon = new PlayerGun (_GunData);
			MonoBehaviour.Destroy (collider);
		}

		public void Start (Player player)
		{
			DefineFireringObservable (player);
		}

		public IObservable<Unit> ShotAsObservable ()
		{
			return _MainWepon.ShootAsObservable ();
		}

		public ReadOnlyReactiveProperty<uint> ReminingBulletCount ()
		{
			return _MainWepon.ReminingResourceCount ();
		}

		public IObservable<Unit> ReloadAsObservable ()
		{
			return _MainWepon.ReloadAsObservable ();
		}

		public PlayerBombController BombController
		{
			get;
			private set;
		}

		/*private void DefineChangeWeponObservable ()
		{
			int loopCount = 0;
			for (int i = 49; i < 59; i++)
			{
				//キーコードの48が0なのでキーボードの並び順とあわせるため
				if (i == 58) { i = 48; }

				//loopCount直接突っ込むとキャプチャされて結果が変わるので
				var x = loopCount;
				InputObservable.Instance.OnKeyDownAsObservable ((KeyCode)i)
					.TakeUntilDestroy (gameObject)
					.Subscribe (_ => _Player.ChangeWepon (x));

				if (i == 48) { break; }
				loopCount++;
			}
		}*/

		private void DefineFireringObservable (Player player)
		{
			DefineShotObservable (player);
			DefineReloadStream (player);
		}

		private void DefineReloadStream (Player player)
		{
			InputObservable.Instance.OnButtonDownAsObservable (ButtonName.ReLoad)
				.Where (__ => player.CanControl)
				.Subscribe (__ =>
				{
					_MainWepon.Reload ();
				});
		}

		private void DefineShotObservable (Player player)
		{
			player.UpdateAsObservable ()
				.Subscribe (_ => _InsidePlayer.center = player.transform.position + _BoundsOffset);
			InputObservable.Instance.OnButtonHoldAsObservable (ButtonName.Shot)
				.Merge (player.UpdateAsObservable ()
							 .Where (_ => Input.GetAxis (ButtonName.Shot) != 0f))
				.Where (_ => !player.Controller.BombController.LockOnMode)
				.Where (_ => player.CanControl)
				.Where (_ => !_InsidePlayer.Contains (UIManager.TargetSight.transform.position))
				.Subscribe (_ =>
				{
					var direction = (Vector2)(UIManager.TargetSight.transform.position - player.Controller.StateController.ShootPoint.transform.position);
					_MainWepon.ShootOut (direction.normalized);
				})
				.AddTo (player);
		}
	}
}