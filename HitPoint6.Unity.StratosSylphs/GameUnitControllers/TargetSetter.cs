using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	using Managers;

	public class TargetSetter : MonoBehaviour
	{
		private void Start ()
		{
			var CCD = GetComponent<SimpleCCD> ();
			CCD.target = UIManager.TargetSight.transform;
		}
	}
}