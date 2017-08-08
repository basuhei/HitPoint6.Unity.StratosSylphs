using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	public abstract class TransitionEffectBase : MonoBehaviour
	{
		public abstract void EffectStart ();

		public abstract IObservable<Unit> EffectCompleteAsObservable ();
	}
}