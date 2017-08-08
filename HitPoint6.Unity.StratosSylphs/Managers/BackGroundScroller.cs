using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	public class BackGroundScroller : MonoBehaviour
	{
		[Serializable]
		private class _BackGroundParams
		{
			[SerializeField]
			private GameObject _BackGroundObject;

			[SerializeField]
			private float _MoveSpeed;

			[SerializeField]
			private bool _Loop;

			[SerializeField]
			private GameObject _LinkageBackGround;

			public GameObject Object
			{
				get { return _BackGroundObject; }
			}

			public GameObject LinkageBackGround
			{
				get { return _LinkageBackGround; }
			}

			public bool Loop
			{
				get { return _Loop; }
			}

			public float MoveSpeed
			{
				get { return _MoveSpeed; }
			}
		}

		private interface _IBackGround
		{
			void Move (float speed);
		}

		private class _LoopBackGround : _IBackGround
		{
			private GameObject _LinkageBackGround;
			private SpriteRenderer _LinkageRenderer;
			private GameObject _MyObject;
			private SpriteRenderer _MyRenderer;

			private Vector3 _Direction = new Vector3 (-1f, 0, 0);
			private float _Sign;

			public _LoopBackGround (GameObject linkageBackGround, GameObject myObject)
			{
				//TODO:クラス分けたい
				_MyObject = myObject;
				_MyRenderer = _MyObject.GetComponent<SpriteRenderer> ();
				if (linkageBackGround != null)
				{
					_LinkageBackGround = linkageBackGround;
					_LinkageRenderer = _LinkageBackGround.GetComponent<SpriteRenderer> ();

					_LinkageBackGround.OnBecameVisibleAsObservable ()
						.Subscribe (_ =>
						 {
							 ReturnBack ();
						 })
						 .AddTo (_LinkageBackGround);
				}
				else
				{
					_MyObject.OnBecameInvisibleAsObservable ()
						.Subscribe (_ =>
						 {
							 var viewRight = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0.5f));
							 viewRight.x += _MyRenderer.bounds.size.x;
							 viewRight.z = 0;
							 _MyObject.transform.position = viewRight;
						 });
				}
			}

			public void Move (float speed)
			{
				_MyObject.transform.position += _Direction * speed * TimeManager.DeltaTime;
				_Sign = Mathf.Sign (speed);
			}

			private void ReturnBack ()
			{
				if (_Sign > 0)
				{
					_MyObject.transform.position = new Vector3 (_LinkageBackGround.transform.position.x + _LinkageRenderer.bounds.size.x / 2f + _MyRenderer.bounds.size.x / 2, 0);
				}
				if (_Sign < 0)
				{
					_MyObject.transform.position = new Vector3 (_LinkageBackGround.transform.position.x - _LinkageRenderer.bounds.size.x / 2f - _MyRenderer.bounds.size.x / 2, 0);
				}
			}
		}

		private class _NonLoopBackGround : _IBackGround
		{
			private GameObject _MyObject;
			private Vector3 _Direction = new Vector3 (-1, 0, 0);

			public _NonLoopBackGround (GameObject myObject)
			{
				_MyObject = myObject;
				myObject.OnBecameInvisibleAsObservable ()
					.Subscribe (_ => Destroy (myObject));
			}

			public void Move (float speed)
			{
				_MyObject.transform.position += _Direction * speed * TimeManager.DeltaTime;
			}
		}

		[SerializeField]
		private _BackGroundParams[] _BackGrounds;

		[SerializeField]
		private float _BaseSpeed;

		private bool _IsFliping;

		public void FlipSpeed ()
		{
			if (!_IsFliping)
			{
				StartCoroutine (_FlipSpeed ());
			}
		}

		private IEnumerator _FlipSpeed ()
		{
			_IsFliping = true;
			var flipedSpeed = -_BaseSpeed;
			var startTime = Time.time;
			while (Mathf.Abs (flipedSpeed) <= Mathf.Abs (_BaseSpeed))
			{
				_BaseSpeed = Mathf.Lerp (_BaseSpeed, flipedSpeed, Time.time - startTime);
				yield return null;
			}
			_IsFliping = false;
		}

		private void Awake ()
		{
			//memo transform.position.x + (SpriteRendere.bounds.size.x / 2) 端の位置の取得
			transform.position = Vector3.zero;
			var updateStream = this.UpdateAsObservable ().Publish ().RefCount ();
			foreach (var b in _BackGrounds)
			{
				var bparams = b;
				_IBackGround backGround;
				if (b.Loop)
				{
					backGround = new _LoopBackGround (bparams.LinkageBackGround, bparams.Object);
				}
				else
				{
					backGround = new _NonLoopBackGround (bparams.Object);
				}
				updateStream.Subscribe (_ =>
				{
					backGround.Move (bparams.MoveSpeed * _BaseSpeed);
				}).AddTo (bparams.Object);
			}
		}
	}
}