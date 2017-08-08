using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.InitializeData
{
	[CreateAssetMenu (menuName = "EnemyBehaviour/MoveData/Swing")]
	public class SwingData : ScriptableObject
	{
		[SerializeField]
		private float _SwingSpeed;

		[SerializeField]
		private Vector2 _SwingDirection;

		[SerializeField]
		private float _SwingWidth;

		public float SwingSpeed { get { return _SwingSpeed; } }

		public Vector2 SwingDirection { get { return _SwingDirection; } }

		public float SwingWidth { get { return _SwingWidth; } }
	}
}