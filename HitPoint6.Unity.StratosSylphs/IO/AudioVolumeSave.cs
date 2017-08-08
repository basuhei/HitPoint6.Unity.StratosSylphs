using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.IO
{
	public static class AudioVolumeSave
	{
		private const string BGM_VOLUME = "StratosSylphs_BGM_Volume";
		private const string SE_VOLUME = "StratosSylphs_SE_Volume";

		public static void SetBGMVolume (float value)
		{
			PlayerPrefs.SetFloat (BGM_VOLUME, value);
		}

		public static float GetBGMVolume ()
		{
			return PlayerPrefs.GetFloat (BGM_VOLUME, 0.7f);
		}

		public static float GetSEVolume ()
		{
			return PlayerPrefs.GetFloat (SE_VOLUME, 0.7f);
		}

		public static void SetSEVolume (float value)
		{
			PlayerPrefs.SetFloat (SE_VOLUME, value);
		}

		public static void Save ()
		{
			PlayerPrefs.Save ();
		}

		public static void SaveErase ()
		{
			PlayerPrefs.DeleteAll ();
		}
	}
}