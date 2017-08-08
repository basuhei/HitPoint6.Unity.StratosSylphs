using UnityEngine;
using UnityEngine.Audio;

namespace HitPoint6.Unity.StratosSylphs.IO
{
	using Utils.Math;

	public class VolumeLoadOnTitle : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer _Mixier;

		private void Awake ()
		{
			_Mixier.SetFloat ("BGM", VolumeValueHelper.NormalizedValueToVolume (AudioVolumeSave.GetBGMVolume ()));
			_Mixier.SetFloat ("SE", VolumeValueHelper.NormalizedValueToVolume (AudioVolumeSave.GetSEVolume ()));
		}
	}
}