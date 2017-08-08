using UnityEngine;

namespace HitPoint6.Unity.StratosSylphs
{
	public class MoveClamper
	{
		private Transform _Transform;
		private Rigidbody2D _Rigidbody2D;
		private Vector2 _LeftTop;
		private Vector2 _RightBottom;
		private Vector2 _Bounds;
		private Vector2? _Offset;

		private MoveClamper (Vector2 bounds, Vector2? offset = null)
		{
			_LeftTop = Camera.main.ViewportToWorldPoint (Vector3.zero);
			_RightBottom = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0));
			_Bounds = bounds;
			_Offset = offset;
			if (!_Offset.HasValue)
			{
				_Offset = Vector2.zero;
			}
		}

		public MoveClamper (Vector2 bounds, Transform transform, Vector2? offset = null) : this (bounds, offset)
		{
			_Transform = transform;
		}

		public MoveClamper (Vector2 bounds, Rigidbody2D rigidody2D, Vector2? offset = null) : this (bounds, offset)
		{
			_Rigidbody2D = rigidody2D;
		}

		private Vector2 _Position
		{
			get
			{
				if (_Transform != null)
				{
					return _Transform.position;
				}
				return _Rigidbody2D.position;
			}
		}

		public void Clamp ()
		{
			if (_Transform != null)
			{
				_ClampTransform ();
				return;
			}
			_ClampRigidbody ();
		}

		private void _ClampRigidbody ()
		{
			Vector2 position = _ClampPosition (_Rigidbody2D.position);
			if (position.x != _Rigidbody2D.position.x)
			{
				_Rigidbody2D.velocity = new Vector2 (0, _Rigidbody2D.velocity.y);
			}
			if (position.y != _Rigidbody2D.position.y)
			{
				_Rigidbody2D.velocity = new Vector2 (_Rigidbody2D.velocity.x, 0);
			}
			_Rigidbody2D.position = position;
		}

		private Vector2 _ClampPosition (Vector2 position)
		{
			position.x = Mathf.Clamp (position.x, (_LeftTop.x + _Offset.Value.x) + (_Bounds.x / 2), (_RightBottom.x + _Offset.Value.x) - (_Bounds.x / 2));
			position.y = Mathf.Clamp (position.y, (_LeftTop.y + _Offset.Value.y) + (_Bounds.y / 2), (_RightBottom.y + _Offset.Value.y) - (_Bounds.y / 2));
			return position;
		}

		private void _ClampTransform ()
		{
			_Transform.position = _ClampPosition (_Transform.position);
		}
	}
}