using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using GameUnits;
	using Managers;

	public class TargetSightCore : MonoBehaviour
	{
		[SerializeField]
		private int _MoveSpeed = 200;

		private MoveClamper _Clamp;

		private Rigidbody2D _Rigidbody2D;
		private Vector2 _Velocity;
		private Vector2 _Position;

		private void Awake ()
		{
			UIManager.TargetSight = this;
			_Rigidbody2D = GetComponent<Rigidbody2D> ();
		}

		public void Start ()
		{
			var player = GameManager.Player;
			_Clamp = new MoveClamper (Vector2.zero, _Rigidbody2D);
			DefineMouseAiming (player);
			DefineFixingTarget (player);

			this.FixedUpdateAsObservable ()
				.Subscribe (_ => _Clamp.Clamp ());
		}

		private void DefineRightStickAiming (Player player)
		{
			//右joyStickで照準する。

			/*this.FixedUpdateAsObservable ()
				.Where(_ => player.Controller.StateController.IsAimMode.Value)
				.Subscribe (_ => _Rigidbody2D.position += _Velocity * _MoveSpeed * Time.fixedDeltaTime);*/
		}

		private void DefineFixingTarget (Player player)
		{
			this.FixedUpdateAsObservable ()
				.Where (_ => TimeManager.TimeScale != 0.0f)
				.Where (_ => !player.Controller.StateController.IsAimMode.Value)
				.Where (_ => !player.Controller.BombController.LockOnMode)
				.Subscribe (_ =>
				{
					_Position.x = player.transform.position.x + 6;
					_Position.y = player.Controller.StateController.ShootPoint.transform.position.y;
					_Rigidbody2D.position = _Position;
				}).AddTo (player);
		}

		private void DefineMouseAiming (Player player)
		{
			this.FixedUpdateAsObservable ()
				.Where (_ => TimeManager.TimeScale != 0.0f)
				.Where (_ => player.Controller.StateController.IsAimMode.Value || player.Controller.BombController.LockOnMode)
				.Select (_ => (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition))

				//.DistinctUntilChanged ()
				.Subscribe (position =>
				{
					_Rigidbody2D.position = transform.parent.transform.position;
				}).AddTo (player);
		}
	}
}