using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class GameOver : MonoBehaviour
	{
		private void Start ()
		{
			GameManager.Player.Controller.LifeController.DeadAsObservable
				.First ()
				.Subscribe (_ =>
				{
					gameObject.SetActive (true);
				});
			gameObject.SetActive (false);
		}
	}
}