using UniRx;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public interface IReloadableContainer : IShootableContainer
	{
		void Reload ();

		ReadOnlyReactiveProperty<uint> RemainingResouceAsObservable ();
	}
}