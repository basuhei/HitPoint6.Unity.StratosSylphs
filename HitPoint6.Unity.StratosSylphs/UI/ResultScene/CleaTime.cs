using System;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI.ResultScene
{
	public class CleaTime : MonoBehaviour
	{
		[SerializeField]
		private Text _Text;

		private void Awake ()
		{
			var clearTime = SceneOverValueHolder.StageClearTime;

			var timeSpan = TimeSpan.FromSeconds (clearTime);

			_Text.text = timeSpan.ToString ();
		}
	}
}