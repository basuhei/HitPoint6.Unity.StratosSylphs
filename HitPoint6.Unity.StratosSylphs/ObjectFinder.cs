using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	using Constants;
	using GameUnits;
	using UI;

	//ロケーターパターン？
	public static class ObjectFinder
	{
		public static TargetSightCore FindTargetSight ()
		{
			return GameObject.FindGameObjectWithTag (Tags.TargetSight).GetComponentInChildren<TargetSightCore> (true);
		}

		public static Player FindPlayer ()
		{
			return GameObject.FindGameObjectWithTag (Tags.Player).GetComponent<Player> ();
		}
	}
}