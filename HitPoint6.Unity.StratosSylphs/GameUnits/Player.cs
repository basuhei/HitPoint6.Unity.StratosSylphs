using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	using GameUnitControllers;
	using Managers;

	public class Player : MonoBehaviour, IDamage
	{
		[SerializeField]
		private PlayerController _Controller;

		private bool _CanControl;

		public PlayerController Controller
		{
			get { return _Controller; }
		}

		public bool CanControl
		{
			get { return _CanControl && !GameManager.Instance.Pause; }
			set { _CanControl = value; }
		}

		public int Damage
		{
			get
			{
				return 1000;
			}
		}

		private void OnDestroy ()
		{
			_Controller = null;//いみないんじゃない？
		}

		private void Awake ()
		{
			GameManager.Instance.RegisterPlayer (this);
			CanControl = true;

			_Controller.Awake (this);
		}

		private void Start ()
		{
			_Controller.Start (this);
		}
	}
}