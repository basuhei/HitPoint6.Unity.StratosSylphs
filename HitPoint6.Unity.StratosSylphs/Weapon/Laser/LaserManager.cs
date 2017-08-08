using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	[Serializable]
	public class LaserManager
	{
		private Dictionary<Laser, Laser> _LaserDictionary = new Dictionary<Laser, Laser> ();

		[SerializeField]
		private GameObject _ShootPoint;

		private Laser _KeyCheckAndGetValue (Laser key)
		{
			Laser laser;
			if (_LaserDictionary.TryGetValue (key, out laser))
			{
				return laser;
			}
			else
			{
				laser = GameObject.Instantiate (key);
				laser.SetFollowObject (_ShootPoint);
				_LaserDictionary.Add (key, laser);
				return laser;
			}
		}

		public void ShotLaser (Laser laser, Vector2 direction)
		{
			_KeyCheckAndGetValue (laser).Shot (direction);
		}

		public void StopLaser (Laser laser)
		{
			_KeyCheckAndGetValue (laser).Stop ();
		}

		public void RotationLaser (Laser laser, Vector2 direction)
		{
			_KeyCheckAndGetValue (laser).Rotate (direction);
		}
	}
}