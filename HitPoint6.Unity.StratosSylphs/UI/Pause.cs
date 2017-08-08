using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class Pause : MonoBehaviour
	{
		[SerializeField]
		private StageManager _Manager;

		private void Start ()
		{
			_Manager.State
				.Where (state => state == StageState.Pause)
				.Subscribe (_ =>
				 {
					 gameObject.SetActive (true);
				 });
			_Manager.State
				.Where (state => state == StageState.Play)
				.Subscribe (_ => gameObject.SetActive (false));
			gameObject.SetActive (false);
		}
	}
}