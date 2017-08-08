using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class MissileCount : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _MissileIcon;

		private Vector2 _OriginSize;

		private void Start ()
		{
			_OriginSize = _MissileIcon.sizeDelta;
			GameManager.Player.Controller.BombController
				.ReminingBombCount
				.Subscribe (count => _MissileIcon.sizeDelta = new Vector2 (_OriginSize.x * count, _OriginSize.y));
		}
	}
}