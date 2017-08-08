using UniRx;

namespace HitPoint6.Unity.StratosSylphs.GameUnits
{
	public interface IShootObserver
	{
		ReadOnlyReactiveProperty<uint> ReminingResourceCount ();

		IObservable<Unit> ReloadAsObservable ();

		IObservable<Unit> ShootAsObservable ();
	}
}