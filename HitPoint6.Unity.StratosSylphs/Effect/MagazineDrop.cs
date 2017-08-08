using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	using Managers;

	public class MagazineDrop : MonoBehaviour
	{
		private void Awake ()
		{
			var rigidbody2D = GetComponent<Rigidbody2D> ();
			var torque = UnityEngine.Random.Range (-1, 1) > 0 ? -10 : 10;
			this.FixedUpdateAsObservable ()
				.Subscribe (_ =>
				 {
					 rigidbody2D.AddForce (new Vector2 (-10 * TimeManager.PlayerFixedDeltaTime, 0), ForceMode2D.Impulse);
					 rigidbody2D.AddTorque (torque, ForceMode2D.Force);
				 });
			this.OnBecameInvisibleAsObservable ()
				.Subscribe (_ => Destroy (this.gameObject));
		}
	}
}