using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Managers;

	public class BossHeadController : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _Head;

		private void Start ()
		{
			foreach (var c in GetComponents<SimpleCCD> ())
			{
				c.target = GameManager.Player.transform;
			}
			var positionStream = this.UpdateAsObservable ()
				.Select (_ => GameManager.Player.transform.position.x - transform.position.x);

			//右向き
			positionStream.Where (gap => gap > 0)
				.Subscribe (_ =>
				{
					_Head[1].SetActive (true);
					_Head[0].SetActive (false);
				});

			//左向き
			positionStream.Where (gap => gap < 0)
				.Subscribe (_ =>
				 {
					 _Head[1].SetActive (false);
					 _Head[0].SetActive (true);
				 });
		}
	}
}