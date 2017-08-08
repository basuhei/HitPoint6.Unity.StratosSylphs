using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using InitializeData;
	using Managers;

	public class Magazine : IReloadableContainer
	{
		private GunData _Data;
		private List<IShootable> _BulletList;
		private ReactiveProperty<uint> _RemainingBullet;
		private ReadOnlyReactiveProperty<uint> _ReadOnlyRemaningBullet;

		[Conditional ("UNITY_EDITOR")
		, Conditional ("DEVELOPMENT_BUILD")]
		private void DefineDebugCommand ()
		{
			//TODO:DEBUG COMMAND　弾無限:N
			Observable.EveryUpdate ()
				.Where (_ => Input.GetKeyDown (KeyCode.N))
				.Subscribe (_ => _RemainingBullet.Value = int.MaxValue)
				.AddTo (GameManager.Player.gameObject);
		}

		public Magazine (GunData data)
		{
			_Data = data;
			_RemainingBullet = new ReactiveProperty<uint> (_Data.ReminingBulletCount);
			_BulletList = new List<IShootable> ();

			_ReadOnlyRemaningBullet = _RemainingBullet.ToReadOnlyReactiveProperty ();

			DefineDebugCommand ();

			Observable.FromCoroutine<GameObject> (_ => InstantiateBullet (_))
				.Subscribe (bullet =>
				{
					_BulletList.Add (UnityEngine.Object.Instantiate (bullet).GetComponent<IShootable> ());
				});

			//１フレーム中にデストロイを集中させないためにコルーチンモドキを外部に生成
			/*Observable.EveryUpdate () //多分いらない
				.SkipUntil (GameManager.Player.OnDestroyAsObservable ())
				.TakeWhile(_ => _BulletList.Count > 0)
				.Subscribe (_ =>
				{
					var bullet = _BulletList
									.Where (b => !b.IsActive)
									.FirstOrDefault ();
					if (bullet != null)
					{
						_BulletList.Remove (bullet);
						bullet.Destroy ();
					}
				}).AddTo();*/
		}

		private IEnumerator InstantiateBullet (IObserver<GameObject> observer)
		{
			while (_BulletList.Count < _Data.ReminingBulletCount)
			{
				observer.OnNext (_Data.Shootable);
				yield return null;
			}
			observer.OnCompleted ();
		}

		public ReadOnlyReactiveProperty<uint> RemainingResouceAsObservable ()
		{
			return _ReadOnlyRemaningBullet;
		}

		public bool Load (out IShootable shootable)
		{
			if (_RemainingBullet.Value <= 0)
			{
				shootable = null;
				return false;
			}

			shootable = _BulletList.FirstOrDefault (b => !b.IsActive);
			if (shootable == null)
			{
				shootable = UnityEngine.Object.Instantiate (_Data.Shootable).GetComponent<IShootable> ();
			}
			_RemainingBullet.Value--;
			return true;
		}

		public void Reload ()
		{
			_RemainingBullet.Value = _Data.ReminingBulletCount;
		}
	}
}