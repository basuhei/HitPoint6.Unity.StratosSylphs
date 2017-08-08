using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using Managers;

	public class MiddleBoss : MonoBehaviour
	{
		[SerializeField]
		private AudioClip _BGM;

		private void Start ()
		{
			EnemySpawnerManager.StopProcess ();
			if (_BGM != null) AudioManager.Instance.ChangeMusic (_BGM);
		}

		private void OnDestroy ()
		{
			if (_BGM != null) AudioManager.Instance.PlayDefaultMusic ();
			EnemySpawnerManager.StartProcess ();
		}
	}
}