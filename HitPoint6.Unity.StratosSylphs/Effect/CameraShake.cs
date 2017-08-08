using System.Collections;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	public class CameraShake : MonoBehaviour
	{
		[SerializeField]
		private float _ShakeAmount;

		[SerializeField, Range (0.0001f, 0.9999f)]
		private float _AttenuationRate;

		private Vector3 _OriginPosition;

		private void Awake ()
		{
			_OriginPosition = transform.position;
		}

		public void Shake ()
		{
			StopAllCoroutines ();
			StartCoroutine (_ShakeCore ());
		}

		private IEnumerator _ShakeCore ()
		{
			var shakeAmount = _ShakeAmount;
			while (shakeAmount > 0.0001f)
			{
				var range = (Vector2)_OriginPosition + Random.insideUnitCircle * shakeAmount;
				transform.position = new Vector3 (range.x, range.y, _OriginPosition.z);
				shakeAmount *= _AttenuationRate;
				yield return null;
			}
			transform.position = _OriginPosition;
		}
	}
}