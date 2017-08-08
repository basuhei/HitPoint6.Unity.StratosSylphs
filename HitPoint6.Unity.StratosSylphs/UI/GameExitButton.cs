﻿using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.UI
{
	public class GameExitButton : MonoBehaviour
	{
		[SerializeField]
		private Button _Button;

		private void Start ()
		{
			_Button.OnClickAsObservable ()
				.Subscribe (_ => Application.Quit ());
		}
	}
}