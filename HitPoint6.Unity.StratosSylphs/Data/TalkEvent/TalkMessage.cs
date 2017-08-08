namespace HitPoint6.Unity.StratosSylphs.Data
{
	public enum Emotion
	{
		None,
		Normal,
		Seriously,
		Trouble,
		Grin,
		Smile,
		Surprise,
		Salute
	}

	public class TalkMessage
	{
		public string Name { get; set; }

		public Emotion Face { get; set; }

		public bool? IsInside { get; set; }

		public bool? LeftSide { get; set; }

		public string Message { get; set; }

		public string Other { get; set; }
	}
}