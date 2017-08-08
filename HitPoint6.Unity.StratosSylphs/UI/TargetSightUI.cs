using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Library.DeviceInput;
	using Managers;

	public class TargetSightUI : MonoBehaviour
	{
		[SerializeField]
		private float _Sensitivity = 10f;

		private Vector2 _Velocity;

		private MoveClamper _Clamp;

		private void Start ()
		{
			_Clamp = new MoveClamper (Vector2.zero, transform);
			DefineRightStickAiming ();
			DefineMouseAiming ();
			this.UpdateAsObservable ()
				.Subscribe (_ => _Clamp.Clamp ());
		}

		private void DefineRightStickAiming ()
		{
			InputObservable.Instance
				.HorizontalSubRawAsObservable ()

				//.Where (_ => player.AimMode)
				.Subscribe (x =>
				{
					var position = transform.position;
					position.x += x * _Sensitivity * Time.deltaTime;
					transform.position = position;

					//transform.Translate (new Vector2 (x, 0) * _MoveSpeed * Time.deltaTime);
				})
				.AddTo (gameObject);

			InputObservable.Instance
				.VerticalSubRawAsObservable ()

				//.Where (_ => player.AimMode)
				.Subscribe (y =>
				{
					var position = transform.position;
					position.y += y * _Sensitivity * Time.deltaTime;
					transform.position = position;

					//transform.Translate (new Vector2 (0, y) * _MoveSpeed * Time.deltaTime);
				})
				.AddTo (gameObject);
		}

		private void DefineMouseAiming ()
		{
			this.UpdateAsObservable ()
				.Where (_ => TimeManager.TimeScale != 0.0f)
				.Select (_ => (Vector2)Camera.main.ScreenToWorldPoint (Input.mousePosition))
				.DistinctUntilChanged ()
				.Subscribe (position => transform.position = position);
		}
	}
}