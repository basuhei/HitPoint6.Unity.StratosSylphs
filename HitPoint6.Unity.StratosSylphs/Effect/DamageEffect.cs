using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	using Managers;

	public class DamageEffect : MonoBehaviour
	{
		[SerializeField]
		private Image _Image;

		[SerializeField]
		private CameraShake _Shake;

		private void Start ()
		{
			GameManager.Player.Controller.LifeController.PlayerLife.Select (life =>
			 {
				 var maxLife = (float)GameManager.Player.Controller.LifeController.MaxLife;
				 return (maxLife - life) / maxLife;
			 })
			 .Subscribe (lifeRatio =>
			  {
				  var color = _Image.color;
				  _Image.color = new Color (color.r, color.g, color.b, lifeRatio);
				  Debug.Log (lifeRatio);
				  Debug.Log (_Image.color);
			  });
		}
	}
}