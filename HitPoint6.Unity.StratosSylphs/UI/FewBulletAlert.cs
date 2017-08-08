using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class FewBulletAlert : MonoBehaviour
	{
		[SerializeField]
		private Text _Text;

		[SerializeField]
		private int _AlertCount;

		[SerializeField]
		private float _PingPongSpeed = 0.3f;

		[SerializeField]
		private float _PingPongHight = 0.2f;

		private void Start ()
		{
			GameManager.Player
				.Controller.FiringController.ReminingBulletCount ()
				.Where (count => count <= _AlertCount)
				.Subscribe (_ => _Text.enabled = true);

			GameManager.Player
				.Controller.FiringController.ReloadAsObservable ()
				.Subscribe (_ => _Text.enabled = false);

			this.UpdateAsObservable ()
				.Subscribe (_ => _Text.rectTransform.position = RectTransformUtility.WorldToScreenPoint (Camera.main, GameManager.Player.transform.position + new Vector3 (0, 2 + Mathf.PingPong (Time.time * _PingPongSpeed, _PingPongHight))));

			_Text.enabled = false;
		}
	}
}