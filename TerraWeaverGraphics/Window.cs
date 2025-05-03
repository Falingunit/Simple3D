using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using System;
using System.Diagnostics;
using TerraWeaver.TerraWeaverGraphics;
using TerraWeaverGraphics;
using TerraWeaverGraphics.Model;
using System.Drawing;

namespace TerraWeaverGraphics
{
	public class Window : GameWindow
	{
        private Vector3[] cubeVerts =
        {
			// Front face
			new Vector3( 0.5f,  0.5f,  0.5f), // Top Right
			new Vector3(-0.5f,  0.5f,  0.5f), // Top Left
			new Vector3(-0.5f, -0.5f,  0.5f), // Bottom Left
			new Vector3( 0.5f, -0.5f,  0.5f), // Bottom Right

			// Back face
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),

			// Left face
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),

			// Right face
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3( 0.5f,  0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),

			// Top face
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3( 0.5f,  0.5f,  0.5f),

			// Bottom face
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),
		};


        private Vector2[] texCoords =
		{
			// Front face
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),

			// Back face
			new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 1.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(0.0f, 0.0f),

			// Left face
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),

			// Right face
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),

			// Top face
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),

			// Bottom face
			new Vector2(1.0f, 1.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(1.0f, 0.0f),
		};

        private uint[] cubeIndices =
		{
			0, 1, 2, 0, 2, 3,       // Front
			4, 5, 6, 4, 6, 7,       // Back
			8, 9,10, 8,10,11,       // Left
		   12,13,14,12,14,15,       // Right
		   16,17,18,16,18,19,       // Top
		   20,21,22,20,22,23        // Bottom
		};

        private float speed = 1.5f;
		private float sensitivity = 0.05f;

		private float yaw, pitch = 0f;

		private GraphicsManager graphicsManager;
		private Stopwatch _timer;
		private int frameCount = 0;

		private Shader colorShader;
		private Shader textureShader;

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

			InitShaders();

            graphicsManager.OnLoad();
			graphicsManager.Camera.PointAt(Vector3.Zero);
			(yaw, pitch) = graphicsManager.Camera.GetYawAndPitch();
			(yaw, pitch) = (MathHelper.RadiansToDegrees(yaw), MathHelper.RadiansToDegrees(pitch));

			Texture texture = new Texture("Textures\\container.png", TextureUnit.Texture0);
			TextureModel model2 = new TextureModel(cubeVerts, cubeIndices, texCoords, textureShader, texture);
            model2.OnLoad();

			for (int i = 0; i < 100; i++) graphicsManager.CreateObject(model2, new Vector3(1.1f * i, 0, 0), Quaternion.Identity);
		}

		private void InitShaders()
		{
            textureShader = new Shader("Shaders\\texture.vert", "Shaders\\texture.frag");
            graphicsManager.AddShader(textureShader);

            colorShader = new Shader("Shaders\\color.vert", "Shaders\\color.frag");
            graphicsManager.AddShader(colorShader);
        }

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				this.Close();
			}

			frameCount++;
			if (frameCount >= 60)
			{
                Title = $"FPS: {frameCount / _timer.Elapsed.TotalSeconds}";
                frameCount = 0;
				_timer.Restart();
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