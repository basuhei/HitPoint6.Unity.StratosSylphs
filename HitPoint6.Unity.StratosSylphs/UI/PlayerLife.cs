using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Effect;
	using Managers;

	public class PlayerLife : MonoBehaviour
	{
		private Image _Gauge;
		private float _StartTime;
		private float _Amount;

		private void Awake ()
		{
			_Gauge = GetComponent<Image> ();
		}

		private void Start ()
		{
			var lifeContoller = GameManager.Player.Controller.LifeController;

			var onDamageStream = lifeContoller.OnDamageAsObservable.Publish ().RefCount ();

			lifeContoller.LifeRegenerateStartAsObservable
				.Subscribe (_ =>
				 {
					 lifeContoller.PlayerLife
								 .TakeUntil (onDamageStream)
								 .Buffer (2, 1)
								 .Where (life => life.Count == 2)
								 .Where (life => life[1] - life[0] > 0)
								 .Subscribe (life =>
								 {
									 var color = _Gauge.color;
									 _Gauge.color = new Color (color.r, color.g, color.b, (lifeContoller.MaxLife - life[0]) / lifeContoller.MaxLife);
								 });
				 });

			onDamageStream
				.Subscribe (_ =>
				 {
					 _Amount = _Gauge.color.a;
					 _StartTime = Time.time;
					 this.UpdateAsObservable ()
						.TakeUntil (onDamageStream)
						.TakeUntil (lifeContoller.LifeRegenerateStartAsObservable)
						 .Subscribe (__ =>
						  {
							  var color = _Gauge.color;
							  color.a = Mathf.SmoothStep (_Amount, (lifeContoller.MaxLife - lifeContoller.PlayerLife.Value) / lifeContoller.MaxLife, Time.time - _StartTime);
							  _Gauge.color = color;
						  });
				 });

			var shake = Camera.main.GetComponent<CameraShake> ();
			if (shake) GameManager.Player.Controller.LifeController.PlayerLife
																  .Buffer (2, 1)
																  .Where (life => life[0] - life[1] > 0)
																  .Subscribe (_ => shake.Shake ());
		}
	}
}