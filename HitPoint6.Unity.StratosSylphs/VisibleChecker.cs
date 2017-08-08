using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	[Serializable]
	public class VisibleChecker
	{
		private enum _CheckType
		{
			AllInCamera,
			AnyInCamera
		}

		[SerializeField]
		private GameObject _CheckObject;

		[SerializeField]
		private SpriteRenderer _CheckRenderer;

		[SerializeField]
		private _CheckType _Type;

		public bool Visible
		{
			get;
			private set;
		}

		private bool OutRight
		{
			get
			{
				return _CheckRenderer.transform.position.x + _CheckRenderer.bounds.extents.x > Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, 0)).x;
			}
		}

		private bool OutLeft
		{
			get
			{
				return _CheckRenderer.transform.position.x - _CheckRenderer.bounds.extents.x < Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 0)).x;
			}
		}

		private bool OutTop
		{
			get
			{
				return _CheckRenderer.transform.position.y + _CheckRenderer.bounds.extents.y > Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, 0)).y;
			}
		}

		private bool OutBottom
		{
			get
			{
				return _CheckRenderer.transform.position.y - _CheckRenderer.bounds.extents.y < Camera.main.ViewportToWorldPoint (Vector3.zero).y;
			}
		}

		private void _IsAllInCamera ()
		{
			_CheckRenderer.UpdateAsObservable ()
				.Where (_ => !OutBottom)
				.Where (_ => !OutLeft)
				.Where (_ => !OutRight)
				.Where (_ => !OutTop)
				.Subscribe (_ => Visible = true);
			_CheckRenderer.UpdateAsObservable ()
				.Where (_ => OutTop
							 || OutBottom
							 || OutLeft
							 || OutRight)
				.Subscribe (_ => Visible = false);
		}

		private void _IsAnyInCamera ()
		{
			_CheckRenderer.OnBecameVisibleAsObservable ()
				.Subscribe (_ => Visible = true);
			_CheckRenderer.OnBecameInvisibleAsObservable ()
				.Subscribe (_ => Visible = false);
		}

		public void Init ()
		{
			if (_CheckObject && _CheckRenderer)
			{
				Debug.LogAssertion ("ObjectとRendereが両方アタッチされています");
				Debug.LogAssertion ("Rendererが優先で判定に使われます");
				_DefineRenderCheckStream ();
				return;
			}

			if (_CheckRenderer)
			{
				_DefineRenderCheckStream ();
				return;
			}

			if (_CheckObject)
			{
				_CheckObject.UpdateAsObservable ()
					.Where (_ => Camera.main.rect.Contains (Camera.main.WorldToViewportPoint ((Vector2)_CheckObject.transform.position)))
					.Subscribe (_ => Visible = true);
				_CheckObject.UpdateAsObservable ()
					.Where (_ => !Camera.main.rect.Contains (Camera.main.WorldToViewportPoint ((Vector2)_CheckObject.transform.position)))
					.Subscribe (_ => Visible = false);
				return;
			}

			Debug.LogAssertion ("オブジェクトがアタッチされていません常に画面に映っているとみなされます");
			Visible = true;
		}

		private void _DefineRenderCheckStream ()
		{
			switch (_Type)
			{
				case _CheckType.AllInCamera:
					_IsAllInCamera ();
					break;

				case _CheckType.AnyInCamera:
					_IsAnyInCamera ();
					break;

				default:
					break;
			}
		}
	}
}