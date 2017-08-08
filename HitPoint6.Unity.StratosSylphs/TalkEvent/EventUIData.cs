using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs.TalkEvent
{
	[Serializable]
	public class AnimatorSet
	{
		[SerializeField]
		private Animator _Left;

		[SerializeField]
		private Animator _Right;

		[SerializeField]
		private Animator _Text;

		private Animator[] _AllAnimator;

		public Animator Left { get { return _Left; } }

		public Animator Right { get { return _Right; } }

		public Animator Text { get { return _Text; } }

		public IEnumerable<Animator> AllAnimator
		{
			get
			{
				if (_AllAnimator == null)
				{
					_AllAnimator = new Animator[] { _Left, _Right, _Text };
				}
				return _AllAnimator;
			}
		}
	}

	[Serializable]
	public class Character
	{
		[SerializeField]
		private Image _LeftBody;

		[SerializeField]
		private Image _LeftFace;

		[SerializeField]
		private Image _RightBody;

		[SerializeField]
		private Image _RightFace;

		public Image LeftBody { get { return _LeftBody; } }

		public Image LeftFace { get { return _LeftFace; } }

		public Image RightBody { get { return _RightBody; } }

		public Image RightFace { get { return _RightFace; } }
	}

	[Serializable]
	public class TalkUI
	{
		[SerializeField]
		private AnimatorSet _Animator;

		[SerializeField]
		private Text _Name;

		[SerializeField]
		private Text _Sentence;

		[SerializeField]
		private Character _Character;

		public AnimatorSet Animator
		{
			get { return _Animator; }
		}

		public Text Name
		{
			get { return _Name; }
		}

		public Text Sentence
		{
			get { return _Sentence; }
		}

		public Character Character { get { return _Character; } }
	}
}