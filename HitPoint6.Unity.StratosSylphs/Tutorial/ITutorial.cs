using UniRx;

namespace HitPoint6.Unity.StratosSylphs.Tutorial
{
	internal interface ITutorial
	{
		void InputObsavationStart ();

		IObservable<Unit> ProcessDoneAsObservable ();
	}
}