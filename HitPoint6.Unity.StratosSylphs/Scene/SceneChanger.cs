using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Scene
{
	using Managers;

	public static class SceneChanger
	{
		private static TransitionManager _TransitionManager;
		private static readonly string _TransitionManagerPrefabPath = "Manager/TransitionManager";

		private static TransitionManager TransitionManager
		{
			get
			{
				if (_TransitionManager != null)
				{
					return _TransitionManager;
				}
				if (TransitionManager.Instance != null)
				{
					_TransitionManager = TransitionManager.Instance;
					return _TransitionManager;
				}
				var transitionManager = Resources.Load (_TransitionManagerPrefabPath);
				UnityEngine.Object.Instantiate (transitionManager);
				_TransitionManager = TransitionManager.Instance;
				return _TransitionManager;
			}
		}

		public static void SceneChange (SceneType scene)
		{
			TransitionManager.TranstionStart (scene);
		}
	}
}