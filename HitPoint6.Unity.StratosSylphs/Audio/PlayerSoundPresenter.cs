using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Audio
{
	using Managers;

	public class PlayerSoundPresenter : MonoBehaviour
	{
		private void Start ()
		{
			GameManager.Player.Controller.FiringController.ShotAsObservable ()
				.Subscribe (_ =>
				 {
					 var clip = AudioManager.PlayerSound.ShotSound;
					 AudioManager.SoundEmitter.PlaySE (clip);
				 });
			GameManager.Player.Controller.BombController.LaunchMissileAsObservable ()
				.Subscribe (_ =>
				 {
					 AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.MissileShotSound);
				 });
			GameManager.Player.Controller.FiringController.ReloadAsObservable ()
				.Subscribe (_ => AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.ReloadSound));
			GameManager.Player.Controller.LifeController.OnDamageAsObservable
				.Subscribe (_ => AudioManager.SoundEmitter.PlaySE (AudioManager.PlayerSound.DamageSound));
		}
	}
}