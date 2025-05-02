
using OpenTK.Mathematics;
using System.ComponentModel.DataAnnotations;

namespace TerraWeaver.TerraWeaverGraphics
{
	public class Camera
	{
		private Vector3 _position;
		private Vector3 _up;
		private Vector3 _direction;
		private bool _dirty = true;

		private Matrix4 _view;

		public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				_dirty = true;
			}
		}

		public Vector3 Up
		{
			get
			{
				return _up;
			}
			set
			{
				_up = value;
				_dirty = true;
			}
		}

		public Vector3 Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
				_direction.NormalizeFast();
				_dirty = true;
			}
		}

		public Vector3 Target
		{
			set
			{
				Direction = _position - value;
			}
		}

		public Matrix4 View
		{
			get
			{
				if (_dirty)
				{
					_dirty = false;
					_view = Matrix4.LookAt(_position, _position + _direction, _up);
					return _view;
				}
				else
				{
					return _view;
				}
			}
		}


		public Camera(Vector3 position, Vector3 up, Vector3 direction)
		{
			Position = position;
			Up = up;
			Direction = direction;
			Position = position;
			Up = up;
			Direction = direction;
		}

		public void OnLoad()
		{
			_view = Matrix4.LookAt(_position, _position + _direction, _up);
		}

		public void MoveForward(float distance)
		{
			Position += distance * _direction;
		}

		public void MoveBackward(float distance)
		{
			MoveForward(-distance);
		}

		public void MoveUp(float distance)
		{
			Position += distance * _up;
		}

		public void MoveDown(float distance)
		{
			MoveUp(-distance);
		}

		public void StrafeRight(float distance)
		{
			Position += distance * (Vector3.Cross(_direction, _up));
		}

		public void StrafeLeft(float distance)
		{
			StrafeRight(-distance);
		}

		public void ChangeDirectionTo(float yaw, float pitch)
		{
			pitch = Math.Clamp(pitch, -89.0f, 89.0f);

			pitch = MathHelper.DegreesToRadians(pitch);
			yaw = MathHelper.DegreesToRadians(yaw);

			Direction = new Vector3(MathF.Cos(pitch) * MathF.Sin(yaw), MathF.Sin(pitch), MathF.Cos(pitch) * MathF.Cos(yaw));
		}

		public void PointAt(Vector3 point)
		{
			Direction = Vector3.NormalizeFast(point - Position);
		}

		public (float, float) GetYawAndPitch()
		{
			return (MathF.Atan2(Direction.X, Direction.Z), MathF.Asin(-Direction.Y));
		}
	}
}
