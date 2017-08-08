using HitPoint6.Unity.StratosSylphs.TalkEvent;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	using Utils.CSV;

	public class TalkEvenTester : MonoBehaviour
	{
		public TextAsset Csv;
		public TalkEventSystem system;

		public void Start ()
		{
			this.UpdateAsObservable ()
				.First (_ => Input.anyKeyDown)
				.Subscribe (_ => system.TalkStart (TalkMessageReader.GetTalkData (Csv)));

			system.EventStartAsObservable ().Subscribe (_ => Debug.Log ("Eventstart"));
			system.EventDoneAsObservable ().Subscribe (_ => Debug.Log ("EventDone"));
		}
	}
}