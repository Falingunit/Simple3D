using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using TerraWeaver.TerraWeaverGraphics;

namespace TerraWeaverGraphics
{
	public class Window : GameWindow
	{
		private float[] verts =
		{
			//Postions             //Colors
			0.5f,  0.5f, 0.5f,    1.0f, 1.0f, 0.0f, //I
			-0.5f, 0.5f, 0.5f,    1.0f, 0.0f, 1.0f, //II
			-0.5f, -0.5f, 0.5f,   0.0f, 1.0f, 1.0f, //III
			0.5f,  -0.5f, 0.5f,   1.0f, 1.0f, 1.0f, //IV
			0.5f,  0.5f, -0.5f,    1.0f, 1.0f, 0.0f, //V
			-0.5f, 0.5f, -0.5f,    1.0f, 0.0f, 1.0f, //VI
			-0.5f, -0.5f, -0.5f,   0.0f, 1.0f, 1.0f, //VII
			0.5f,  -0.5f, -0.5f,   1.0f, 1.0f, 1.0f, //VIII
		};

		private uint[] indices =
		{
			0, 1, 2,
			0, 2, 3,
			4, 5, 6,
			4, 6, 7,
			0, 1, 5,
			0, 5, 4,
			1, 2, 6,
			1, 6, 5,
			2, 3, 7,
			2, 7, 6,
			3, 0, 4,
			3, 4, 7,
		};

		private float speed = 1.5f;
		private float sensitivity = 0.05f;

		private float yaw, pitch = 0f;

		private TerraWeaver.TerraWeaverGraphics.GraphicsObject cube;

		private GraphicsManager graphicsManager;
		private Stopwatch _timer;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{
		}

		protected override void OnLoad()
		{
			base.OnLoad();
			_timer = new Stopwatch();
			_timer.Start();

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);

			graphicsManager = new(this);

			graphicsManager.OnLoad();
			graphicsManager.Camera.PointAt(Vector3.Zero);
			(yaw, pitch) = graphicsManager.Camera.GetYawAndPitch();
			(yaw, pitch) = (MathHelper.RadiansToDegrees(yaw), MathHelper.RadiansToDegrees(pitch));

			Model model = new Model(verts, indices);
			model.OnLoad();
			cube = graphicsManager.CreateObject(model, new Vector3(0, 0, 0), Quaternion.Identity);
			graphicsManager.CreateObject(model, new Vector3(0, 0, 0), Quaternion.Identity);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				this.Close();
			}

			if (MouseState.IsButtonDown(MouseButton.Left))
			{
				CursorState = CursorState.Grabbed;
				yaw -= MouseState.Delta.X * sensitivity;
				pitch -= MouseState.Delta.Y * sensitivity;
				pitch = Math.Clamp(pitch, -89.0f, 89.0f);

				graphicsManager.Camera.ChangeDirectionTo(yaw, pitch);
			}
			else
			{
				CursorState = CursorState.Normal;
			}

				var dist = speed * (float)args.Time;

			if (KeyboardState.IsKeyDown(Keys.D))
			{
				graphicsManager.Camera.StrafeRight(dist);
			}
			if (KeyboardState.IsKeyDown(Keys.A))
			{
				graphicsManager.Camera.StrafeLeft(dist);
			}
			if (KeyboardState.IsKeyDown(Keys.Space))
			{
				graphicsManager.Camera.MoveUp(dist);
			}
			if (KeyboardState.IsKeyDown(Keys.LeftShift))
			{
				graphicsManager.Camera.MoveDown(dist);
			}
			if (KeyboardState.IsKeyDown(Keys.W))
			{
				graphicsManager.Camera.MoveForward(dist);
			}
			if (KeyboardState.IsKeyDown(Keys.S))
			{
				graphicsManager.Camera.MoveBackward(dist);
			}

		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			graphicsManager.OnRenderFrame();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, e.Size.X, e.Size.Y);
		}

		protected override void OnUnload()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
			GL.UseProgram(0);
			base.OnUnload();
		}
	}
}