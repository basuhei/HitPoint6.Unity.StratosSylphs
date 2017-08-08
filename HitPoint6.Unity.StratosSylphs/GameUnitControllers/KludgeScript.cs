using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs.GameUnitControllers
{
	public class KludgeScript : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer t;

		[SerializeField]
		private SpriteRenderer me;

		private void Start ()
		{
			this.LateUpdateAsObservable ()
				.Subscribe (_ =>
				{
					transform.localPosition = t.transform.localPosition;
					me.flipX = t.flipX;
					me.sprite = t.sprite;
				});
		}
	}
}