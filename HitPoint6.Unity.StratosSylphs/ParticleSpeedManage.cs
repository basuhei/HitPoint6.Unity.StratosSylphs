using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	using Managers;

	public class ParticleSpeedManage : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem _Particle;

		private void Awake ()
		{
			var main = _Particle.main;
			this.UpdateAsObservable ()
				.Subscribe (_ => main.simulationSpeed = TimeManager.PlayerTimeScale);
		}
	}
}