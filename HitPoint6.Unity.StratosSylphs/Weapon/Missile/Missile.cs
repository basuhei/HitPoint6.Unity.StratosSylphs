using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using Constants;
	using InitializeData;
	using Managers;

	public class Missile : MonoBehaviour, IDamage
	{
		[SerializeField]
		private BombData _Data;

		private Rigidbody2D _Rigidbody2D;
		private Vector2 _Velocity;
		private TrailRenderer _TrailRendere;
		private Animator _Animator;
		private Collider2D _Collider;

		public int Damage
		{
			get { return _Data.Damage; }
		}

		private void Awake ()
		{
			_Rigidbody2D = GetComponent<Rigidbody2D> ();
			_TrailRendere = GetComponent<TrailRenderer> ();
			_TrailRendere.enabled = false;
			_Animator = GetComponent<Animator> ();
			_Collider = GetComponent<Collider2D> ();
		}

		private void Start ()
		{
			var collider = GetComponent<Collider2D> ();
			this.OnTriggerEnter2DAsObservable ()
				.Subscribe (_ =>
				{
					_Collider.enabled = false;
					StopAllCoroutines ();
					_Velocity = Vector2.zero;
					AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.MissileHitSound);
					_TrailRendere.time = -1f;
					_TrailRendere.enabled = false;
					_Animator.SetTrigger (AnimationParams.Hit);
				});
			this.FixedUpdateAsObservable ()
				.Subscribe (_ =>
				{
					_Rigidbody2D.MovePosition (_Rigidbody2D.position + _Velocity);
				});

			//this.OnBecameInvisibleAsObservable ()
			//	.Subscribe (_ => gameObject.SetActive (false));

			gameObject.SetActive (false);
		}

		public void Launch (Vector2 position, Collider2D target)
		{
			transform.position = position;
			_Collider.enabled = true;
			gameObject.SetActive (true);
			_TrailRendere.enabled = true;
			StartCoroutine (_LaunchCore (target));
		}

		private IEnumerator _LaunchCore (Collider2D target)
		{
			float rotationPerSec = 180f;
			Vector2 targetVector = Vector2.zero;
			float angleRotation;
			float angleGap;
			Quaternion targetRotation;
			float speed = 5;
			float terminalSpeed = 30f;
			float deltaTime = -TimeManager.PlayerBulletFixedDeltaTime;
			while (gameObject.activeInHierarchy)
			{
				if (deltaTime > 1f)
				{
					gameObject.SetActive (false);
					_TrailRendere.enabled = false;
					yield break;
				}
				if (target == null)
				{
					_Velocity = (Vector2)(transform.TransformDirection (Vector3.right) * speed * TimeManager.PlayerBulletFixedDeltaTime);
					yield return null;
				}
				deltaTime += TimeManager.PlayerDeltaTime;
				if (target != null)
				{ targetVector = target.transform.position - transform.position; }
				targetVector.Normalize ();

				angleRotation = rotationPerSec * TimeManager.PlayerBulletFixedDeltaTime;
				targetRotation = Quaternion.FromToRotation (Vector3.right, targetVector);
				angleGap = Quaternion.Angle (transform.rotation, targetRotation);

				if (angleGap <= angleRotation)
				{
					transform.rotation = targetRotation;
				}
				else
				{
					float t = angleRotation / angleGap;
					transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, t);
				}

				_Velocity = (Vector2)(transform.TransformDirection (Vector3.right) * speed * TimeManager.PlayerBulletFixedDeltaTime);
				speed = Mathf.LerpUnclamped (1, terminalSpeed, (deltaTime * 2) * (deltaTime * 2));
				rotationPerSec = Mathf.LerpUnclamped (180, 2000, deltaTime * deltaTime);
				yield return null;
				_TrailRendere.time = Mathf.Lerp (-1f, 0.7f, deltaTime * 2);
			}
			Debug.Log ("終了");
		}
	}
}