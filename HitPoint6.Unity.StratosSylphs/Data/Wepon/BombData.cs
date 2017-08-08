using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu]
	public class BombData : ScriptableObject
	{
		[SerializeField]
		private int _Damage;

		public int Damage { get { return _Damage; } }
	}
}