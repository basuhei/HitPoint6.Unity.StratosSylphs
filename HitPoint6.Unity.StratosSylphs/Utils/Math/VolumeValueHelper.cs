namespace HitPoint6.Unity.StratosSylphs.Utils.Math
{
	public static class VolumeValueHelper
	{
		public static float NormalizedValueToVolume (float normalizeValue)
		{
			return 100 * normalizeValue - 80;
		}
	}
}