using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Effect
{
	using Managers;

	public class DeathEffect : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem _Effect;

		private void Start ()
		{
			GameManager.Player.Controller.LifeController.DeadAsObservable
				.Subscribe (_ => EffectStart ());
		}

		public void EffectStart ()
		{
			_Effect.Play ();
		}
	}
}