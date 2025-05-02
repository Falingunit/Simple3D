using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraWeaverGraphics;

namespace TerraWeaver.TerraWeaverGraphics
{
	public class GraphicsManager
	{
		private Window window;

		private List<GraphicsObject> _objects;

		private Shader _shader;
		private Camera _camera;

		private Matrix4 _projection;

		public Camera Camera
		{
			get
			{
				return _camera;
			}
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public GraphicsManager(Window window) 
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		{
			_objects = new List<GraphicsObject>();
			this.window = window;
		}

		public void AddObject(GraphicsObject obj)
		{
			_objects.Add(obj);
		}

		public void AddObjects(GraphicsObject[] objects)
		{
			this._objects.AddRange(objects);
		}

		public GraphicsObject CreateObject(Model model, Vector3? position, Quaternion? rotation)
		{
			GraphicsObject obj = new GraphicsObject(model, position, rotation);
			obj.OnLoad();
			AddObject(obj);
			return obj;
		}

		public GraphicsObject[] CreateObjects(Model model, Vector3[] positions, Quaternion[] rotations)
		{
			GraphicsObject[] objects = new GraphicsObject[positions.Length];
			if (positions.Length != rotations.Length)
			{
				throw new ArgumentException("Positions and rotations arrays must have the same length.");
			}
			for (int i = 0; i < positions.Length; i++)
			{
				objects[i] = CreateObject(model, positions[i], rotations[i]);
			}

			return objects;
		}

		public void OnLoad()
		{
			this._shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
			this._camera = new Camera(Vector3.UnitZ * 5, Vector3.UnitY, -Vector3.UnitZ);

			_shader.Use();

			_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
				(float)window.Size.X / (float)window.Size.Y, 0.1f, 100.0f);
			GL.UniformMatrix4(_shader.GetUniformLocation("projection"), true, ref _projection);

			foreach (GraphicsObject obj in _objects)
			{
				obj.OnLoad();
			}
		}

		public void OnRenderFrame()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			foreach (GraphicsObject obj in _objects)
			{
				Matrix4 view = _camera.View;
				GL.UniformMatrix4(_shader.GetUniformLocation("view"), true, ref view);
				obj.Draw(_shader);
			}
			window.SwapBuffers();
		}
	}
}
