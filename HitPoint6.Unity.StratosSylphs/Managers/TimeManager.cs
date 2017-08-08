using System.Collections;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.Managers
{
	public static class TimeManager
	{
		private static float _BulletTimeScale = 1.0f;
		private static float _EnemyBulletTimeScale = 1.0f;
		private static float _PlayerBulletTimeScale = 1.0f;
		private static float _PlayerTimeScale = 1.0f;
		private static float _EnemyTimeScale = 1.0f;
		private static float _GameUnitTimeScale = 1.0f;
		private static float _TimeScale = 1.0f;

		private static bool _IsLerp = false;
		private static Coroutine _Coroutine;

		private static bool _Pause = false;
		private static float _PausedTimeScale;

		private static IEnumerator LerpCore (float timeScale, float targetValue, float duration)
		{
			_IsLerp = true;
			var deltaTime = 0.0f;
			while (duration + 0.01f > deltaTime)
			{
				if (!_Pause)
				{
					_TimeScale = Mathf.SmoothStep (timeScale, targetValue, deltaTime / duration);
					deltaTime += Time.deltaTime;
				}
				yield return null;
			}
			_IsLerp = false;
			_TimeScale = targetValue;
		}

		public static void Pause ()
		{
			_PausedTimeScale = _TimeScale;
			_TimeScale = 0.0f;
			_Pause = true;
		}

		public static void PauseRelease ()
		{
			_TimeScale = _PausedTimeScale;
			_Pause = false;
		}

		public static void Reset ()
		{
			_BulletTimeScale = 1.0f;
			_EnemyBulletTimeScale = 1.0f;
			_PlayerBulletTimeScale = 1.0f;
			_PlayerTimeScale = 1.0f;
			_EnemyTimeScale = 1.0f;
			_GameUnitTimeScale = 1.0f;
			_TimeScale = 1.0f;
		}

		public static bool IsPause { get { return _Pause; } }

		public static void SmoothStepTimeScale (float targetTimeScale, float duration)
		{
			if (_IsLerp) { return; }
			_Coroutine = GameManager.Instance.StartCoroutine (LerpCore (_TimeScale, targetTimeScale, duration));
		}

		public static float TimeScale
		{
			get { return _TimeScale; }

			set
			{
				if (_Pause) { return; }
				if (_Coroutine != null) GameManager.Instance.StopCoroutine (_Coroutine);
				_TimeScale = value;
				_IsLerp = false;
				_Coroutine = null;
			}
		}

		public static float DeltaTime
		{
			get { return _TimeScale * Time.deltaTime; }
		}

		public static float BulletTimeScale
		{
			get { return _BulletTimeScale * TimeScale; }
			set { _BulletTimeScale = value; }
		}

		public static float EnemyBulletTimeScale
		{
			get { return _EnemyBulletTimeScale * BulletTimeScale; }
			set { _EnemyBulletTimeScale = value; }
		}

		public static float PlayerBulletTimeScale
		{
			get { return _PlayerBulletTimeScale * BulletTimeScale; }
			set { _PlayerBulletTimeScale = value; }
		}

		public static float GameUnitTimeScale
		{
			get { return _GameUnitTimeScale * TimeScale; }
			set { _GameUnitTimeScale = value; }
		}

		public static float PlayerTimeScale
		{
			get { return _PlayerTimeScale * GameUnitTimeScale; }
			set { _PlayerTimeScale = value; }
		}

		public static float EnemyTimeScale
		{
			get { return _EnemyTimeScale * GameUnitTimeScale; }
			set { _EnemyTimeScale = value; }
		}

		public static float PlayerDeltaTime
		{
			get
			{
				return Time.deltaTime * PlayerTimeScale;
			}
		}

		public static float EnemyBulletFixedDeltaTime
		{
			get { return Time.fixedDeltaTime * EnemyBulletTimeScale; }
		}

		public static float BulletDeltaTime
		{
			get { return Time.deltaTime * BulletTimeScale; }
		}

		public static float PlayerBulletFixedDeltaTime
		{
			get { return Time.fixedDeltaTime * PlayerBulletTimeScale; }
		}

		public static float PlayerFixedDeltaTime
		{
			get { return Time.fixedDeltaTime * PlayerTimeScale; }
		}

		public static float EnemyFixedDeltaTime
		{
			get { return Time.fixedDeltaTime * EnemyTimeScale; }
		}

		public static float EnemyDeltaTime
		{
			get { return Time.deltaTime * EnemyTimeScale; }
		}
	}
}