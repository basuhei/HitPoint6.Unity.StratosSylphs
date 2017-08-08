using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	using Managers;

	public class Score : MonoBehaviour
	{
		//TODO: EnemyManagerクラス作りたい
		private ReactiveProperty<int> _ScoreValue;

		private void Awake ()
		{
			UIManager.Score = this;
			_ScoreValue = new ReactiveProperty<int> (0);
			ScoreValue = _ScoreValue.ToReadOnlyReactiveProperty ();
		}

		private void Start ()
		{
			var text = GetComponent<Text> ();
			_ScoreValue.SubscribeToText (text);
		}

		public ReadOnlyReactiveProperty<int> ScoreValue
		{
			get;
			private set;
		}

		public void AddScore (int score)
		{
			_ScoreValue.Value += score;
		}
	}
}