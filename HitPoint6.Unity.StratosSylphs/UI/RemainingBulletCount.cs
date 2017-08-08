using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class RemainingBulletCount : MonoBehaviour
	{
		private void Start ()
		{
			var text = GetComponent<Text> ();
			GameManager.Player.Controller.FiringController.ReminingBulletCount ()
				.SubscribeToText (text);
		}
	}
}