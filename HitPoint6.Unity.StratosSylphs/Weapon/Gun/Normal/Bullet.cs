using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using InitializeData;
	using Managers;

	[RequireComponent (typeof (Rigidbody2D), typeof (Collider2D))]
	public class Bullet : MonoBehaviour, IShootable, IDamage
	{
		[SerializeField]
		protected BulletData _Data;

		protected Rigidbody2D _Rigidbody2D;
		protected Vector2 _Velocity;

		private bool _IsActive;

		public float LifeTime
		{
			get { return _Data.LifeTime; }
		}

		public int Damage
		{
			get { return _Data.Damage; }
		}

		public bool IsActive
		{
			get
			{
				return _IsActive;
			}
		}

		public GameObject GameObject
		{
			get
			{
				return gameObject;
			}
		}

		private void Awake ()
		{
			_Rigidbody2D = GetComponent<Rigidbody2D> ();
			_IsActive = false;

			//gameObject.hideFlags = HideFlags.HideInHierarchy;
			gameObject.SetActive (false);

			gameObject.OnDestroyAsObservable ()
				.Subscribe (_ => _IsActive = false);

			gameObject.OnDisableAsObservable ()
				.Subscribe (_ => _IsActive = false);

			this.UpdateAsObservable ()
				.Subscribe (_ => _IsActive = gameObject.activeInHierarchy);
		}

		private void Start ()
		{
			//画面外にでたら消える
			this.OnBecameInvisibleAsObservable ()
				.Subscribe (_ => gameObject.SetActive (false));

			//動き
			this.FixedUpdateAsObservable ()
				.Subscribe (_ => _Rigidbody2D.position += _Velocity * _Data.Speed * TimeManager.PlayerBulletFixedDeltaTime);

			//あたった時
			this.OnTriggerEnter2DAsObservable ()
				.Subscribe (_ => gameObject.SetActive (false));
		}

		private IEnumerator _LifeTime ()
		{
			var deltaTime = 0.0f - TimeManager.BulletDeltaTime;
			while (deltaTime < _Data.LifeTime)
			{
				deltaTime += TimeManager.BulletDeltaTime;
				yield return null;
			}
			gameObject.SetActive (false);
		}

		public void Shoot (Vector2 position, Vector2 direction)
		{
			_IsActive = true;
			transform.position = position;
			_Velocity = direction.normalized;
			var targetRotation = Quaternion.FromToRotation (Vector3.right, _Velocity);
			transform.rotation = targetRotation;
			gameObject.SetActive (true);
			StartCoroutine (_LifeTime ());
		}

		public void Destroy ()
		{
			if (gameObject != null) Destroy (gameObject);
		}
	}
}