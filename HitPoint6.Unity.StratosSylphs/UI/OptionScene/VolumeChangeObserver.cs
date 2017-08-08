using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI.OptionScene
{
	public class VolumeChangeObserver : MonoBehaviour
	{
		[SerializeField]
		private Slider[] _VolumeBar;

		private bool _DirtyFlag = false;

		public bool DirtyFlag { get { return _DirtyFlag; } }

		private void Start ()
		{
			foreach (var v in _VolumeBar)
			{
				v.OnValueChangedAsObservable ()
					.Subscribe (_ => _DirtyFlag = true);
			}
		}
	}
}