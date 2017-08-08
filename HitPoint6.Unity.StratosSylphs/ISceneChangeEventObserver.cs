using System;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	public class ISceneChangeEventObserver : MonoBehaviour
	{
		[SerializeField]
		private string _ScenePath;

		protected string ScenePath
		{
			get { return _ScenePath; }
		}

		public virtual IObservable<string> SceneChangeEventAsObservable ()
		{
			throw new NotImplementedException ();
		}
	}
}