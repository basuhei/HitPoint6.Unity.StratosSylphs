using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class Result : MonoBehaviour
	{
		[SerializeField]
		private Text _ValueText;

		private void Awake ()
		{
			this.ObserveEveryValueChanged (_ => UIManager.Score)
				.First (scoreUI => scoreUI != null)
				.Subscribe (scoreUI =>
				{
					scoreUI.ScoreValue
					.SubscribeToText (_ValueText);
				});
		}
	}
}