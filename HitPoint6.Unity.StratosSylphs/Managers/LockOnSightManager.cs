using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using UI;

	public class LockOnSightManager : MonoBehaviour
	{
		[SerializeField]
		private LockOnSight _LockOnSight;

		private List<LockOnSight> _LockOnSightList;

		private void Awake ()
		{
			_LockOnSightList = new List<LockOnSight> ();
		}

		private void Start ()
		{
			for (int i = 0; i < GameManager.Player.Controller.BombController.LockOnLimit; i++)
			{
				var lockOnSight = Instantiate<LockOnSight> (_LockOnSight);
				lockOnSight.GetComponent<RectTransform> ().SetParent (transform);
				lockOnSight.GetComponent<RectTransform> ().localScale = _LockOnSight.GetComponent<RectTransform> ().localScale;
				_LockOnSightList.Add (lockOnSight);
				lockOnSight.gameObject.SetActive (false);
			}

			GameManager.Player.Controller.BombController.FirstLockOnAsObservable ()
				.Merge (GameManager.Player.Controller.BombController.MultiLockAsObservable ())
				.Subscribe (c =>
				 {
					 AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.LockOnSound);
					 var lockOnSight = _LockOnSightList
					 .FirstOrDefault (l => !l.gameObject.activeInHierarchy);

					 if (lockOnSight != null)
					 {
						 lockOnSight.Activate (c);
					 }
				 });
		}
	}
}