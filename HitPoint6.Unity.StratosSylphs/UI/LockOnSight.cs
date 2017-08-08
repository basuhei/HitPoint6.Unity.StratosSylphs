using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class LockOnSight : MonoBehaviour
	{
		private RectTransform _RectTransform;
		private List<IDisposable> _InitDisposeList;

		public GameObject test;

		private void Awake ()
		{
			_RectTransform = GetComponent<RectTransform> ();
			_InitDisposeList = new List<IDisposable> ();
		}

		private void _Init ()
		{
			foreach (var d in _InitDisposeList)
			{
				d.Dispose ();
			}
			_InitDisposeList.Clear ();
		}

		public void Activate (Collider2D enemey)
		{
			_Init ();
			test = enemey.gameObject;
			_InitDisposeList.Add (
			GameManager.Player.Controller.BombController
				.LaunchMissileAsObservable ()
				.Where (t => t.Item2 == enemey)
				.Subscribe (t =>
				{
					_InitDisposeList.Add (
					t.Item1.gameObject.OnDisableAsObservable ()
					.Subscribe (_ =>
					{
						gameObject.SetActive (false);
						_Init ();
					}));
				}));

			_InitDisposeList.Add (
				this.UpdateAsObservable ()
				.TakeUntil (enemey.OnDestroyAsObservable ())
				.TakeUntil (enemey.OnDisableAsObservable ())
				.Subscribe (_ =>
				{
					_RectTransform.position = RectTransformUtility.WorldToScreenPoint (Camera.main, enemey.transform.position);
				}));

			_InitDisposeList.Add (
				enemey.OnDisableAsObservable ()
				.Subscribe (_ => gameObject.SetActive (false)));

			_InitDisposeList.Add (
				enemey.OnDestroyAsObservable ()
				.Subscribe (_ => gameObject.SetActive (false)));

			gameObject.SetActive (true);
		}
	}
}