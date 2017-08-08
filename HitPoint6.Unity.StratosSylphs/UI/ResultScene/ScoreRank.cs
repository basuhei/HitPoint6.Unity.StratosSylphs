using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	[Serializable]
	public class ScoreRankEditData
	{
		[SerializeField]
		private int _Score;

		[SerializeField]
		private string _ScoreRank;

		public int Score { get { return _Score; } }

		public string ScoreRank { get { return _ScoreRank; } }
	}

	public class ScoreRank : MonoBehaviour
	{
		[SerializeField]
		private ScoreRankEditData[] _RankData;

		[SerializeField]
		private Text _RankText;

		[SerializeField]
		private int DebugScore;

		private void Awake ()
		{
			Debug.Assert (_RankData.Length > 0, "RankDataが設定されていません");

			var score = SceneOverValueHolder.ScoreValue;
			var orderdRank = _RankData.OrderBy (d => d.Score);

			var data = orderdRank.LastOrDefault (d => d.Score <= score);

			if (data == null)
			{
				data = orderdRank.First ();
			}
			_RankText.text = data.ScoreRank;
		}
	}
}