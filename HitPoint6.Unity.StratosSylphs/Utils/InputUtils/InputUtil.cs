using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Utils.InputUtils
{
	using Constants;

	public static class InputUtil
	{
		public static bool AnyButtonDownWithOutPauseButton
		{
			get
			{
				return Input.GetButtonDown (ButtonName.Aim)
					   || Input.GetButtonDown (ButtonName.Bomb)
					   || Input.GetButtonDown (ButtonName.ReLoad)
					   || Input.GetButtonDown (ButtonName.Shot);
			}
		}
	}
}