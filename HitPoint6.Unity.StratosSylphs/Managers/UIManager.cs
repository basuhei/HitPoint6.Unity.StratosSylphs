namespace HitPoint6.Unity.StratosSylphs.Managers
{
	using Library.CustomizedMonoBehavior;
	using UI;

	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		public static TargetSightCore TargetSight { get; set; }

		public static Score Score { get; set; }

		public static GameClear GameClear { get; set; }
	}
}