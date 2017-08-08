using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public interface IShootable : IDamage
	{
		float LifeTime { get; }

		bool IsActive { get; }

		GameObject GameObject { get; }

		void Destroy ();

		void Shoot (Vector2 position, Vector2 direction);
	}
}