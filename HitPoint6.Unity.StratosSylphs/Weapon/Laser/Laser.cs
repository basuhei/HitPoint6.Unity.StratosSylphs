using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public class Laser : MonoBehaviour
	{
		[SerializeField, Header ("Laserとrigidbodyはこのゲームオブジェクトのプレファブからリンクさせてね")]
		private ParticleSystem _Laser;

		[SerializeField]
		private Rigidbody2D _Rigidbody;

		[SerializeField]
		private Animator _Animator;

		[SerializeField, Header ("角度は赤いやじるしが右のときが0度だよ")]
		private float _AngleOffset;

		private float _Angle;

		public float Duration
		{
			get
			{
				return _Laser.duration;
			}
		}

		private void Awake ()
		{
			this.FixedUpdateAsObservable ()
				.Subscribe (_ => _Rigidbody.MoveRotation (_Angle));
			_Animator.gameObject.SetActive (false);
		}

		public void SetFollowObject (GameObject obj)
		{
			this.FixedUpdateAsObservable ()
				.Subscribe (_ => _Rigidbody.MovePosition (obj.transform.position))
				.AddTo (obj);
		}

		public void Rotate (Vector2 direction)
		{
			_Angle = _CulcAngle (direction);
		}

		private float _CulcAngle (Vector2 direction)
		{
			return Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg + _AngleOffset;
		}

		public void Stop ()
		{
			_Laser.Stop (true);
		}

		public void Shot (Vector2 direction)
		{
			if (_Laser.isPlaying) { return; }
			_Animator.gameObject.SetActive (true);
			Rotate (direction);
			_Laser.Play ();
			_Animator.SetTrigger ("ShotLaser");
			this.UpdateAsObservable ()
				.SkipWhile (_ => _Laser.isPlaying)
				.First ()
				.Subscribe (_ => _Animator.gameObject.SetActive (false));
		}
	}
}