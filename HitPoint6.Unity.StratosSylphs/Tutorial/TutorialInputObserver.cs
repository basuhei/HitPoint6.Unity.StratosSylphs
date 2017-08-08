using System;
using System.Linq;
using UniRx;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	public class TutorialInputObserver
	{
		private Subject<Unit> _CompleteObserver = new Subject<Unit> ();
		private IObservable<long> _ObsarveComplete;

		public void ObsavationStart (params Func<bool>[] input)
		{
			_ObsarveComplete = Observable.EveryUpdate ().First (_ => input.First ().Invoke ());
			foreach (var i in input.Skip (1))
			{
				_ObsarveComplete = _ObsarveComplete.Zip (Observable.EveryUpdate ()
															.First (_ => i.Invoke ()), (l, r) => r);
			}
			_ObsarveComplete.Subscribe (_ =>
			{
				_CompleteObserver.OnNext (Unit.Default);
			});
		}

		public IObservable<Unit> ObsavationCompleteAsObsavable ()
		{
			return _CompleteObserver;
		}
	}
}