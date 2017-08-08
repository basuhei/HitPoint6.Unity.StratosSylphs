using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public interface IShooter
	{
		float ShotPerSec { get; }

		void ShootOut (Vector2 direction);
	}
}