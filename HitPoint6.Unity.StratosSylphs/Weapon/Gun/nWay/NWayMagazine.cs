using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public class NWayMagazine
	{
		private Dictionary<Bullet, List<Bullet>> _BulletDictionary = new Dictionary<Bullet, List<Bullet>> ();
		private bool SceneChanged = false;

		public NWayMagazine (GameObject enemy)
		{
			enemy.OnDestroyAsObservable ()
				.Subscribe (_ => DestroyResouce (enemy));
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void DestroyResouce (GameObject enemy)
		{
			var updateStream = Observable.EveryUpdate ().Publish ().RefCount ();
			for (int i = 0; i < _BulletDictionary.Keys.Count; i++)
			{
				var bulletList = _BulletDictionary[_BulletDictionary.Keys.ElementAt (i)];
				updateStream
					.TakeWhile (_ => !SceneChanged)
					.TakeWhile (_ => bulletList.Count > 0)
					.Subscribe (_ =>
					{
						Bullet bullet = null;
						for (int x = 0; x < bulletList.Count; x++)
						{
							if (bulletList[x].IsActive)
							{
								continue;
							}
							bullet = bulletList[x];
						}
						if (bullet == null)
						{
							return;
						}
						bulletList.Remove (bullet);
						bullet.Destroy ();
					});
			}
		}

		private void SceneManager_sceneLoaded (UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
		{
			SceneChanged = true;
		}

		private void _CheckKey (Bullet key)
		{
			if (_BulletDictionary.ContainsKey (key))
			{
				return;
			}
			_BulletDictionary.Add (key, new List<Bullet> ());
			for (int i = 0; i < 10; i++)
			{
				_BulletDictionary[key].Add (GameObject.Instantiate (key));
			}
		}

		public bool Load (Bullet key, out Bullet bullet)
		{
			_CheckKey (key);

			for (int i = 0; i < _BulletDictionary[key].Count; i++)
			{
				if (_BulletDictionary[key][i].IsActive) { continue; }

				bullet = _BulletDictionary[key][i];
				return true;
			}

			for (int i = 0; i < 9; i++)
			{
				_BulletDictionary[key].Add (GameObject.Instantiate (key));
			}
			bullet = GameObject.Instantiate (key);
			_BulletDictionary[key].Add (bullet);
			return true;
		}
	}
}