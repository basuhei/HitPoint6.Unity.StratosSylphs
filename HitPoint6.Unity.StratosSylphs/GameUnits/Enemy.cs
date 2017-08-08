using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using Constants;
	using GameUnitControllers;
	using Managers;

	public class Enemy : MonoBehaviour, IDamage
	{
		private NwayGun _Shooter;

		[SerializeField]
		private Animator _Animator;

		private float _SpeedRatio = 1;

		[SerializeField]
		private int _InitLife = 100;

		[SerializeField]
		private GameObject _ShootPoint;

		private Rigidbody2D _RigidBody2D;

		private ReactiveProperty<int> _Life;

		[SerializeField]
		private int _Damage = 100;

		[SerializeField]
		private int _Score = 100;

		[SerializeField]
		private VisibleChecker _VisibleChecker;

		[SerializeField]
		private LaserManager _LaserManager;

		public int MaxLife
		{
			get { return _InitLife; }
		}

		public IObservable<int> LifeAsObservable ()
		{
			return _Life;
		}

		public ReadOnlyReactiveProperty<int> Life
		{
			get;
			private set;
		}

		public bool Visible
		{
			get { throw new NotImplementedException (); }
		}

		public float SpeedRatio
		{
			get { return _SpeedRatio; }
			set { _SpeedRatio = value; }
		}

		public Vector2 ShootPoint { get { return _ShootPoint.transform.position; } }

		public float Sign { get; private set; }

		public int Damage
		{
			get
			{
				return _Damage;
			}
		}

		public int Score
		{
			get { return _Score; }
		}

		private void OnDestroy ()
		{
			_LaserManager = null;
			_VisibleChecker = null;
			_RigidBody2D = null;
			_ShootPoint = null;
			_Shooter = null;
			_Animator = null;
			_Life.Dispose ();
			_Life = null;
		}

		private void Awake ()
		{
			if (_ShootPoint == null)
			{
				_ShootPoint = gameObject;
			}
			Sign = Mathf.Sign (_ShootPoint.transform.position.x - transform.position.x);

			if (!GetComponent<AnimatorSpeedController> ())
			{
				gameObject.AddComponent<AnimatorSpeedController> ();
			}

			var colliders = GetComponentsInChildren<Collider2D> (true);
			foreach (var c in colliders)
			{
				c.isTrigger = true;
				var damageComponent = c.gameObject.GetComponent<DamageComponent> ();
				if (damageComponent == null)
				{
					damageComponent = c.gameObject.AddComponent<DamageComponent> ();
					damageComponent.Damage = Damage;
				}
			}
			_RigidBody2D = GetComponent<Rigidbody2D> ();
			_RigidBody2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
			_Life = new ReactiveProperty<int> (_InitLife);

			Life = _Life.ToReadOnlyReactiveProperty ();

			if (!_Animator) _Animator = GetComponent<Animator> ();
			_VisibleChecker.Init ();
		}

		private void Start ()
		{
			_Life.Where (life => life <= 0)
				.ThrottleFirst (TimeSpan.FromSeconds (1))
				.Subscribe (_ =>
				{
					if (!GetComponent<Boss> ())
					{
						AudioManager.SoundEmitter.PlaySE (AudioManager.EnemySound.DestroySound);
					}
					_Animator.SetBool (AnimationParams.Dead, true);
					UIManager.Score.AddScore (_Score);
				});

			_Shooter = new NwayGun (_ShootPoint);

			var rendere = GetComponent<SpriteRenderer> ();

			//TODO:ダメージ処理を別のクラスに
			this.OnTriggerEnter2DAsObservable ()
				.Where (_ => new Rect (0, 0, 1, 1).Contains (Camera.main.WorldToViewportPoint (rendere.transform.position)))
				.ThrottleFirstFrame (1)
				.Subscribe (collision =>
				{
					var damage = collision.GetComponent<IDamage> ();
					if (collision.GetComponent<IDamage> () != null)
					{
						_Animator.SetBool (AnimationParams.Damage, true);
						AddDamage (damage.Damage);
					}
				});

			this.FixedUpdateAsObservable ()
				.Subscribe (_ =>
				 {
					 _RigidBody2D.MovePosition (_RigidBody2D.position + _Velocity * _SpeedRatio * TimeManager.EnemyFixedDeltaTime);
				 });
			this.LateUpdateAsObservable ()
				.Where (_ => _DirectionChange)
				.Subscribe (_ =>
				 {
					 if (_Velocity.x > 0)
					 {
						 gameObject.transform.localScale = new Vector3 (-1, 1, 1);
					 }
					 else if (_Velocity.x < 0)
					 {
						 gameObject.transform.localScale = new Vector3 (1, 1, 1);
					 }
				 });

			Life.Where (value => value <= 0)
				.Subscribe (_ => _Velocity = Vector2.zero);
		}

		[SerializeField]
		private bool _DirectionChange = false;

		private Vector2 _Velocity;

		public void Move (Vector2 velocity)
		{
			if (velocity == new Vector2 (float.NaN, float.NaN))
			{
				velocity = Vector2.zero;
			}
			_Velocity = velocity;
			if (_Life.Value <= 0)
			{
				_Velocity = Vector2.zero;
			}
		}

		public void MoveDirectory (Vector2 position)
		{
			_Velocity = Vector2.zero;
			_RigidBody2D.MovePosition (position);
		}

		public void Shot (Bullet bullet, Vector2 direction)
		{
			if (!_VisibleChecker.Visible) { return; }
			_Shooter.ShootOut (direction, bullet);
		}

		public void ShotLaser (Laser laser, Vector2 direction)
		{
			_LaserManager.ShotLaser (laser, direction);
		}

		public void RotationLaser (Laser laser, Vector2 direction)
		{
			_LaserManager.RotationLaser (laser, direction);
		}

		public void StopLaser (Laser laser)
		{
			_LaserManager.StopLaser (laser);
		}

		public void AnimationStateChange (string message)
		{
			if (_Animator == null || string.IsNullOrEmpty (message))
			{
				return;
			}
			_Animator.SetTrigger (message);
		}

		public void AddDamage (int value)
		{
			_Life.Value -= value;
		}
	}
}