using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using IO;
	using Utils.Math;

	public class VolumeSlider : MonoBehaviour
	{
		private enum VolumeType
		{
			BGM,
			SE
		}

		[SerializeField]
		private Slider _VolumeBar;

		[SerializeField]
		private AudioMixer _Mixer;

		[SerializeField]
		private VolumeType _VolumeType;

		private void Awake ()
		{
			if (_VolumeType == VolumeType.BGM)
			{
				_VolumeBar.OnSelectAsObservable ()
					.Subscribe (_ => Debug.Log ("BGMを流す"));

				_Mixer.SetFloat ("BGM", VolumeValueHelper.NormalizedValueToVolume (AudioVolumeSave.GetBGMVolume ()));
				_VolumeBar.value = AudioVolumeSave.GetBGMVolume ();
				_VolumeBar.OnValueChangedAsObservable ()
					.Subscribe (volume =>
					 {
						 AudioVolumeSave.SetBGMVolume (volume);
						 _Mixer.SetFloat ("BGM", VolumeValueHelper.NormalizedValueToVolume (volume));
						 AudioVolumeSave.Save ();
					 });
			}
			if (_VolumeType == VolumeType.SE)
			{
				_VolumeBar.OnSelectAsObservable ()
					.ThrottleFirst (TimeSpan.FromSeconds (0.7))
					.Subscribe (_ => Debug.Log ("SEを鳴らす"));
				_Mixer.SetFloat ("SE", VolumeValueHelper.NormalizedValueToVolume (AudioVolumeSave.GetBGMVolume ()));
				_VolumeBar.value = AudioVolumeSave.GetSEVolume ();
				_VolumeBar.OnValueChangedAsObservable ()
					.Subscribe (volume =>
					{
						AudioVolumeSave.SetSEVolume (volume);
						_Mixer.SetFloat ("SE", VolumeValueHelper.NormalizedValueToVolume (volume));
						AudioVolumeSave.Save ();
					});
			}
		}

		[ContextMenu ("LoadDefaultValue")]
		private void LoadDefaultValue ()
		{
			AudioVolumeSave.SaveErase ();
			_VolumeBar.value = AudioVolumeSave.GetBGMVolume ();
		}
	}
}