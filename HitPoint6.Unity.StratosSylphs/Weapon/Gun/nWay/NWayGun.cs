using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public class NwayGun
	{
		private NWayMagazine _Magazine;
		private Bullet _Bullet;
		private GameObject _ShootPoint;

		public NwayGun (GameObject shootPoint)
		{
			_ShootPoint = shootPoint;
			_Magazine = new NWayMagazine (shootPoint);
		}

		public float ShotPerSec
		{
			get
			{
				return 0f;
			}
		}

		public void ShootOut (Vector2 direction, Bullet bullet)
		{
			if (_Magazine.Load (bullet, out _Bullet))
			{
				_Bullet.Shoot (_ShootPoint.transform.position, direction);
			}
		}
	}
}