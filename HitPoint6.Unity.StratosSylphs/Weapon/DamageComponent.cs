using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	public class DamageComponent : MonoBehaviour, IDamage
	{
		[SerializeField]
		private int _Damage;

		public int Damage
		{
			get
			{
				return _Damage;
			}

			set
			{
				_Damage = value;
			}
		}
	}
}