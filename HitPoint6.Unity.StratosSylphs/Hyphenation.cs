using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace HitPoint6.Unity.StratosSylphs
{
	public static class Hyphenation
	{

		private static readonly string NEW_LINE = Environment.NewLine;

		private const string HYPHENATION_FRONT = ",)]｝、。）〕〉》」』】〙〗〟’”｠»" +
			"ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ" +
			"‐゠–〜ー" +
			"?!‼⁇⁈⁉" +
			"・:;" +
			"。.";

		private const string HYPHENATION_BACK = "([｛〔〈《「『【〘〖〝‘“｟«";

		private static readonly StringBuilder _TextBuilder = new StringBuilder ();

		public static string GetAdjustmentTextList (string text, Text textUI)
		{
			if (string.IsNullOrEmpty (text))
			{
				return string.Empty;
			}

			float rectWidth = textUI.GetComponent<RectTransform> ().rect.width;

			textUI.text = "m m";
			float twoCharacterWithSpaceWidth = textUI.preferredWidth;
			textUI.text = "mm";
			float noneSpaceTwoCharacterWidth = textUI.preferredWidth;
			float spaceSize = (twoCharacterWithSpaceWidth - noneSpaceTwoCharacterWidth);
			
			textUI.horizontalOverflow = HorizontalWrapMode.Overflow;

			var wordlist = _GetTextList (text);
			float lineWidth = 0.0f;
			_TextBuilder.Length = 0;

			foreach (var word in wordlist)
			{
				textUI.text = word;
				lineWidth += textUI.preferredWidth;

				if (word == NEW_LINE)
				{
					lineWidth = 0;
				}
				else
				{
					if (word == " ")
					{
						lineWidth += spaceSize;
					}
					if (lineWidth > rectWidth)
					{
						_TextBuilder.AppendLine ();
						textUI.text = word;
						lineWidth = textUI.preferredWidth;
					}
				}
				_TextBuilder.Append (word);
			}
			textUI.text = string.Empty;
			return _TextBuilder.ToString ();
		}

		private static IEnumerable<string> _GetTextList (string text)
		{
			List<string> words = new List<string> ();

			_TextBuilder.Length = 0;

			for (int i = 0; i < text.Length; i++)
			{
				string currentCharacter = text[i].ToString ();
				string nextCharacter = string.Empty;

				if (i < text.Length - 1)
				{
					nextCharacter = text[i + 1].ToString ();
				}

				string previousCharacter = string.Empty;

				if (i > 0)
				{
					previousCharacter = text[i - 1].ToString ();
				}

				if (IsLatin (currentCharacter) && !IsLatin (previousCharacter))
				{
					words.Add (_TextBuilder.ToString ());
					_TextBuilder.Length = 0;
				}

				_TextBuilder.Append (currentCharacter);

				if (!IsLatin (currentCharacter) && _CheckHypenationBack (previousCharacter))
				{
					words.Add (_TextBuilder.ToString ());
					_TextBuilder.Length = 0;
				}

				if (!IsLatin (nextCharacter) && !_CheckHypenationFront (nextCharacter) && !_CheckHypenationBack (currentCharacter))
				{
					words.Add (_TextBuilder.ToString ());
					_TextBuilder.Length = 0;
				}

				if (i == text.Length - 1)
				{
					words.Add (_TextBuilder.ToString ());
				}
			}
			return words;
		}

		private static bool _CheckHypenationFront (string text)
		{
			return HYPHENATION_FRONT.Contains (text);
		}

		private static bool _CheckHypenationBack (string text)
		{
			return HYPHENATION_BACK.Contains (text);
		}

		private static bool IsLatin (string text)
		{
			return Regex.IsMatch (text, @"^[a-zA-Z0-9<>().,]+$");
		}
	}
}