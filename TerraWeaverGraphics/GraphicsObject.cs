using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform.Windows;
using TerraWeaverGraphics;

namespace TerraWeaver.TerraWeaverGraphics
{
	public class GraphicsObject
	{
		private Vector3 _position;
		private Quaternion _rotation;
		private bool _dirty = true;
		private Matrix4 _modelMatrix;

		public Model Model;


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
		public Quaternion Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
				_dirty = true;
			}
		}

		public Matrix4 ModelMatrix
		{
			get
			{
				if (_dirty)
				{
					_dirty = false;
					_modelMatrix =  Matrix4.CreateFromQuaternion(_rotation) * Matrix4.CreateTranslation(_position);
					return _modelMatrix;
				}
				else
				{
					return _modelMatrix;
				}
			}
		}

		public GraphicsObject(Model model, Vector3? position, Quaternion? rotation)
		{
			Model = model;
			_position = position.HasValue ? position.Value : Vector3.Zero;
			_rotation = rotation.HasValue ? rotation.Value : Quaternion.Identity;
		}

		public void OnLoad()
		{
			_dirty = false;
			_modelMatrix = Matrix4.CreateFromQuaternion(_rotation) * Matrix4.CreateTranslation(_position);
		}

		public void Draw(Shader shader)
		{
			Model.Draw(shader, ModelMatrix);
		}
	}
}
