using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.TalkEvent
{
	using Constants;
	using Data;
	using GameUnits;
	using Utils.InputUtils;

	public class TalkEventSystem : MonoBehaviour
	{
		private const string FOCUS_TRIGGER = "Focus";
		private const string END_TRIGGER = "End";
		private const string SKIP_TRRIGER = "Skip";
		private const string ASSET_FOLDER = "EventGraphics/";

		private Dictionary<string, TalkEventSpriteData> _SpriteDataCashMap = new Dictionary<string, TalkEventSpriteData> ();

		[SerializeField]
		private TalkUI _UI;

		[SerializeField]
		private ChoiceDialog _Choices;

		[SerializeField]
		private float _WaitSec;

		[SerializeField]
		private bool _IsEventEscapementFromAnimationState = false;

		private static bool _IsTalking = false;

		private Subject<TalkMessage> _EventObserver = new Subject<TalkMessage> ();

		private Subject<TalkEventState> _AnimationEventAwaiter = new Subject<TalkEventState> ();
		private Subject<bool> _DialogResultObserver = new Subject<bool> ();
		private bool _StartAnimationCompleted = false;
		private bool _EndAnimationCompleted = false;

		private IObservable<Unit> _EventStartAsObservable;
		private IObservable<TalkMessage> _EventDoneAsObservable;

		private Subject<Unit> _EscapementMessageObserver = new Subject<Unit> ();

		public IObservable<Unit> EventStartAsObservable ()
		{
			return _EventStartAsObservable;
		}

		public static TalkEventSystem Instance
		{
			get; private set;
		}

		public IObservable<TalkMessage> EventDoneAsObservable ()
		{
			return _EventDoneAsObservable;
		}

		public IObservable<bool> DialogResultAsObservable ()
		{
			return _DialogResultObserver;
		}

		private void Awake ()
		{
			Instance = this;
			_EventDoneAsObservable = _EventObserver.Where (msg => msg != null).ThrottleFrame (1);
			_EventStartAsObservable = _EventObserver.Where (msg => msg == null).ThrottleFrame (1).AsUnitObservable ();

			gameObject.SetActive (false);

			_AnimationEventAwaiter.Where (state => state == TalkEventState.Start)
				.Subscribe (_ => _StartAnimationCompleted = true);

			_AnimationEventAwaiter.Where (state => state == TalkEventState.End)
				.Throttle (TimeSpan.FromSeconds (0.1))
				.Subscribe (_ => _EndAnimationCompleted = true);
		}

		public void UIAnimationCallBack (TalkEventState state)
		{
			_AnimationEventAwaiter.OnNext (state);
		}

		private void OnDestroy ()
		{
			_SpriteDataCashMap.Clear ();
			_SpriteDataCashMap = null;
			_UI = null;
			Instance = null;
			_EventDoneAsObservable = null;
			_EventStartAsObservable = null;
			_IsTalking = false;
			_AnimationEventAwaiter.OnCompleted ();
			_DialogResultObserver.OnCompleted ();
			Debug.Log ("OndeStroyTalk");
			_EventObserver.OnCompleted ();
			_EventObserver = null;
		}

		public void Escapement ()
		{
			_EscapementMessageObserver.OnNext (Unit.Default);
		}

		public void TalkStart (TalkMessage[] data)
		{
			gameObject.SetActive (true);
			_UI.Sentence.text = string.Empty;
			_EventObserver.OnNext (null);
			_StartAnimationCompleted = false;
			_EndAnimationCompleted = false;
			StartCoroutine (TalkCore (data));
		}

		private TalkEventSpriteData _LoadSpriteData (string characterName)
		{
			TalkEventSpriteData data;

			if (!_SpriteDataCashMap.TryGetValue (characterName, out data))
			{
				data = Resources.Load<TalkEventSpriteData> (ASSET_FOLDER + characterName);
				_SpriteDataCashMap.Add (characterName, data);
				return data;
			}

			return data;
		}

		private IEnumerator TalkCore (TalkMessage[] data)
		{
			if (_IsTalking) { yield break; }
			_IsTalking = true;

			var leftSprite = data.Where (d => d.LeftSide.Value).FirstOrDefault ();
			var rightSprite = data.Where (d => !d.LeftSide.Value).FirstOrDefault ();

			_SetSprite (leftSprite);
			_SetSprite (rightSprite);
			while (!_StartAnimationCompleted)
			{
				yield return null;
			}

			foreach (var d in data)
			{
				_AnimatorFocusChange (d);
				_SetSprite (d);
				_ImageFlip (d);
				if (d.Other == "選択肢(いいえを選ぶとタイトルへ戻る)")
				{
					yield return _Choices.DialogPop ();
					bool choiceResult = _Choices.Result;
					_DialogResultObserver.OnNext (choiceResult);
					/*TODO:いいえを選んだ場合会話を終了させてるのでよろしくない
					 */
					if (!choiceResult)
					{
						_IsTalking = false;
						_EndAnimation ();
						while (!_EndAnimationCompleted)
						{
							yield return null;
						}
						gameObject.SetActive (false);
						_EventObserver.OnNext (d);
						yield break;
					}
				}
				else if (d.Other == "背景にアプス出現")
				{
					var boss = GameObject.FindGameObjectWithTag (Tags.Boss).transform.root.GetComponent<Animator> ();
					boss.SetTrigger ("SpawnApis");
					Debug.Log (d.Other + " : SpawnApisトリガーを起動 " + boss.name);

					yield return _EventEscapement (d);
				}
				else if (d.Other == "ボス画面右外へ移動")
				{
					var boss = GameObject.FindGameObjectWithTag (Tags.Boss).transform.root.GetComponent<Animator> ();
					boss.SetTrigger ("EscapeWithApis");
					Debug.Log (d.Other + " : EscapeWithApisトリガーを起動");
					yield return _EventEscapement (d);
				}
				else if (d.Other == "アプス消える")
				{
					var boss = GameObject.FindGameObjectWithTag (Tags.Boss).GetComponent<Enemy> ();
					Debug.Log (d.Other + " メッセージ:ターゲットゲームオブジェクトの名前 " + boss.name);
					yield return _EventEscapement (d);
				}
				else
				{
					yield return _Escapement (d);
					yield return _WaitForAnyKeyDown ();
				}
			}
			_IsTalking = false;

			_EndAnimation ();

			while (!_EndAnimationCompleted)
			{
				yield return null;
			}
			gameObject.SetActive (false);
			_EventObserver.OnNext (data.Last ());
		}

		private IEnumerator _WaitForAnyKeyDown ()
		{
			while (!InputUtil.AnyButtonDownWithOutPauseButton)
			{
				yield return null;
			}
		}

		private IEnumerator _SkipAnimation ()
		{
			foreach (var animator in _UI.Animator.AllAnimator)
			{
				animator.SetBool (SKIP_TRRIGER, true);
			}

			yield return null;

			foreach (var animator in _UI.Animator.AllAnimator)
			{
				animator.SetBool (SKIP_TRRIGER, false);
			}
		}

		private void _EndAnimation ()
		{
			foreach (var animator in _UI.Animator.AllAnimator)
			{
				animator.SetTrigger (END_TRIGGER);
			}
		}

		private IEnumerator _EventEscapement (TalkMessage data)
		{
			yield return _UnSkippableEscapement (data);
			if (_IsEventEscapementFromAnimationState)
			{
				yield return _EscapementMessageObserver.First ().StartAsCoroutine ();
			}
			else
			{
				yield return _WaitForAnyKeyDown ();
			}
		}

		private IEnumerator _UnSkippableEscapement (TalkMessage data)
		{
			var message = data.Message;
			var fixedName = data.Name.Replace ("(制服)", string.Empty);
			fixedName = fixedName.Replace ("ボス素顔", "？？？");
			_UI.Name.text = fixedName;
			_UI.Sentence.text = string.Empty;
			yield return null;

			float deltaTime;
			for (int i = 0; i < message.Length; i++)
			{
				deltaTime = 0.0f;
				while (deltaTime < _WaitSec)
				{
					deltaTime += Time.deltaTime;
					yield return null;
				}
				_UI.Sentence.text += message[i];
				yield return null;
			}
		}

		private IEnumerator _Escapement (TalkMessage data)
		{
			var message = data.Message;
			var fixedName = data.Name.Replace ("(制服)", string.Empty);
			fixedName = fixedName.Replace ("ボス素顔", "？？？");
			_UI.Name.text = fixedName;
			_UI.Sentence.text = string.Empty;
			yield return null;
			float deltaTime;
			for (int i = 0; i < message.Length; i++)
			{
				deltaTime = 0.0f;
				while (deltaTime < _WaitSec && !InputUtil.AnyButtonDownWithOutPauseButton)
				{
					deltaTime += Time.deltaTime;
					yield return null;
				}
				if (InputUtil.AnyButtonDownWithOutPauseButton)
				{
					_UI.Sentence.text = data.Message;
					yield return _SkipAnimation ();
					break;
				}
				_UI.Sentence.text += message[i];
				yield return null;
			}
		}

		[Obsolete]
		private IEnumerable<string> _GetSentence (string message)
		{
			var splitSymbol = new string[] { Environment.NewLine };
			var tempText = Hyphenation.GetAdjustmentTextList (message, _UI.Sentence);
			var textList = tempText.Split (splitSymbol, StringSplitOptions.None);
			var dirty = true;
			string newText;
			int count = 0;
			while (dirty && count < 30)
			{
				count++;
				newText = string.Empty;
				for (int i = 0; i < textList.Length; i++)
				{
					dirty = false;
					if (i % 2 == 0 && textList[i].FirstOrDefault () != '「')
					{
						textList[i] = "「" + textList[i];
						dirty = true;
					}

					if (i != 0 && (i + 1) % 2 == 0 && textList[i].LastOrDefault () != '」')
					{
						textList[i] += "」";
						dirty = true;
					}
				}
				for (int i = 0; i < textList.Length; i++)
				{
					newText += textList[i];
				}
				tempText = Hyphenation.GetAdjustmentTextList (newText, _UI.Sentence);
				textList = tempText.Split (splitSymbol, StringSplitOptions.None);
			}
			return textList;
		}

		private void _AnimatorFocusChange (TalkMessage data)
		{
			if (!data.LeftSide.HasValue) { return; }

			if (data.LeftSide.Value)
			{
				_UI.Animator.Left.SetBool (FOCUS_TRIGGER, true);
				_UI.Animator.Right.SetBool (FOCUS_TRIGGER, false);
			}
			else
			{
				_UI.Animator.Right.SetBool (FOCUS_TRIGGER, true);
				_UI.Animator.Left.SetBool (FOCUS_TRIGGER, false);
			}
		}

		private void _SetSprite (TalkMessage data)
		{
			if (data == null) { return; }
			var sprite = _LoadSpriteData (data.Name);
			if (sprite == null) { return; }
			Sprite face;
			switch (data.Face)
			{
				case Emotion.None:
					face = data.LeftSide.Value ? _UI.Character.LeftFace.sprite : _UI.Character.RightFace.sprite;
					break;

				case Emotion.Normal:
					face = sprite.Normal;
					break;

				case Emotion.Seriously:
					face = sprite.Seriously;
					break;

				case Emotion.Trouble:
					face = sprite.Trouble;
					break;

				case Emotion.Grin:
					face = sprite.Grin;
					break;

				case Emotion.Smile:
					face = sprite.Smile;
					break;

				case Emotion.Surprise:
					face = sprite.Surprise;
					break;

				case Emotion.Salute:
					face = sprite.Salute;
					break;

				default:
					face = _UI.Character.LeftFace.sprite;
					break;
			}

			if (face == null) { return; }

			if (data.LeftSide.Value)
			{
				if (face == sprite.Salute)
				{
					_UI.Character.LeftBody.enabled = false;
					_UI.Character.LeftFace.sprite = face;
				}
				else
				{
					_UI.Character.LeftBody.enabled = true;
					_UI.Character.LeftFace.sprite = face;
					_UI.Character.LeftBody.sprite = sprite.Body;
				}
			}
			if (!data.LeftSide.Value)
			{
				if (face == sprite.Salute)
				{
					_UI.Character.RightBody.enabled = false;
					_UI.Character.RightFace.sprite = face;
				}
				else
				{
					_UI.Character.RightBody.enabled = true;
					_UI.Character.RightFace.sprite = face;
					_UI.Character.RightBody.sprite = sprite.Body;
				}
			}
		}

		private void _ImageFlip (TalkMessage data)
		{
			return;

			//if (!data.LeftSide.HasValue) { return; }
			//
			//if (!data.IsInside.HasValue) { return; }
			//
			//if (data.LeftSide.Value)
			//{
			//	if (data.IsInside.Value)
			//	{
			//		//右向きの画像を基準にする
			//		_UI.Character.LeftBody.rectTransform.localScale = Vector3.one;
			//	}
			//	else
			//	{
			//		_UI.Character.LeftBody.rectTransform.localScale = new Vector3 (-1, 1, 1);
			//	}
			//}
			//else
			//{
			//	if (data.IsInside.Value)
			//	{
			//		_UI.Character.RightBody.rectTransform.localScale = new Vector3 (-1, 1, 1);
			//	}
			//	else
			//	{
			//		_UI.Character.RightBody.rectTransform.localScale = Vector3.one;
			//	}
			//}
		}
	}
}