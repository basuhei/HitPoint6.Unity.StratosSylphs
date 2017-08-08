using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI.ResultScene
{
	public class ResultScore : MonoBehaviour
	{
		[SerializeField]
		private Text _Text;

		private void Awake ()
		{
			_Text.text = SceneOverValueHolder.ScoreValue.ToString ();
		}
	}
}