using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class PlayerRemainingUnit : MonoBehaviour
	{
		private void Start ()
		{
			var text = GetComponent<Text> ();

			this.UpdateAsObservable ()
				.First ()
				.Subscribe (_ =>
				 {
					 GameManager.Player.Controller.LifeController.RemainingUnit
						 .SubscribeToText (text);
				 });
		}
	}
}