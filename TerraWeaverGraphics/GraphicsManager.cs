using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaverGraphics;
using TerraWeaverGraphics.Model;

namespace TerraWeaverGraphics
{
	public class GraphicsManager
	{
		private Window window;

		private List<GraphicsObject> _objects;
		public List<Shader> _shaders = new List<Shader>();

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
		public void AddShader(Shader shader)
        {
            this._shaders.Add(shader);
        }

        public GraphicsObject CreateObject(Model.Model model, Vector3? position, Quaternion? rotation)
		{
			GraphicsObject obj = new GraphicsObject(model, position, rotation);
			obj.OnLoad();
			AddObject(obj);
			return obj;
		}

		public GraphicsObject[] CreateObjects(Model.Model model, Vector3[] positions, Quaternion[] rotations)
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
			this._camera = new Camera(Vector3.UnitZ * 5, Vector3.UnitY, -Vector3.UnitZ);

			foreach (GraphicsObject obj in _objects)
			{
				obj.OnLoad();
			}

			//Set projection matrix
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)window.Size.X / (float)window.Size.Y, 0.1f, 100.0f);
            
			foreach (Shader shader in _shaders)
			{
				shader.OnLoad();
				shader.Use();
                GL.UniformMatrix4(shader.GetUniformLocation("projectionMatrix"), true, ref _projection);
            }
        }

        public void OnRenderFrame()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			foreach (GraphicsObject obj in _objects)
			{
				Matrix4 view = _camera.View;
                foreach (Shader shader in _shaders)
                {
					shader.Use();
                    GL.UniformMatrix4(shader.GetUniformLocation("viewMatrix"), true, ref view);
                }
				obj.Draw();
			}
			window.SwapBuffers();
		}
	}
}
