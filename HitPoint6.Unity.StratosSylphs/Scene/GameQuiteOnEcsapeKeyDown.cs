using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Scene
{
	public class GameQuiteOnEcsapeKeyDownInTitleScene : MonoBehaviour
	{
		private void Awake ()
		{
			this.UpdateAsObservable ()
				.First (_ => Input.GetKeyDown (KeyCode.Escape))
				.Subscribe (_ => Application.Quit ());
		}
	}
}
