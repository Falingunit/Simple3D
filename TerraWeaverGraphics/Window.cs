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
using TerraWeaver.TerraWeaverGraphics.Model;

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

        private Vector3[] cubeNormals =
		{
			// Front face (Z+)
			new Vector3(0.0f, 0.0f, 1.0f),
			new Vector3(0.0f, 0.0f, 1.0f),
			new Vector3(0.0f, 0.0f, 1.0f),
			new Vector3(0.0f, 0.0f, 1.0f),

			// Back face (Z-)
			new Vector3(0.0f, 0.0f, -1.0f),
			new Vector3(0.0f, 0.0f, -1.0f),
			new Vector3(0.0f, 0.0f, -1.0f),
			new Vector3(0.0f, 0.0f, -1.0f),

			// Left face (X-)
			new Vector3(-1.0f, 0.0f, 0.0f),
			new Vector3(-1.0f, 0.0f, 0.0f),
			new Vector3(-1.0f, 0.0f, 0.0f),
			new Vector3(-1.0f, 0.0f, 0.0f),

			// Right face (X+)
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),

			// Top face (Y+)
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 1.0f, 0.0f),
			new Vector3(0.0f, 1.0f, 0.0f),

			// Bottom face (Y-)
			new Vector3(0.0f, -1.0f, 0.0f),
			new Vector3(0.0f, -1.0f, 0.0f),
			new Vector3(0.0f, -1.0f, 0.0f),
			new Vector3(0.0f, -1.0f, 0.0f),
		};


        private ModelImporter modelImporter = new ModelImporter();

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
			graphicsManager.Camera.Position = new Vector3(0, 5, 2);
			graphicsManager.Camera.PointAt(Vector3.Zero);
			(yaw, pitch) = graphicsManager.Camera.GetYawAndPitch();
			(yaw, pitch) = (MathHelper.RadiansToDegrees(yaw), MathHelper.RadiansToDegrees(pitch));

			colorShader.SetVector("ambientLight", new(1f));
			colorShader.SetFloat("ambientIntensity", 0.8f);

			textureShader.SetVector("ambientLight", new(1f));
			textureShader.SetFloat("ambientIntensity", 0.8f);

			GenerateTestModels();
			GenerateTestLights();
		}

		private void GenerateTestModels()
		{
			VertexColorModel boat = modelImporter.ImportVertexColorMesh("Models\\Boat.obj", colorShader);
			graphicsManager.CreateObject(boat, new Vector3(0, 0, 0), Quaternion.Identity);

			VertexColorModel light = modelImporter.ImportVertexColorMesh("Models\\Light.obj", colorShader, 0.2f);
			light.Material = new Material(light.Material.Diffuse, light.Material.Specular, new Vector3(1f), light.Material.Shininess);
			graphicsManager.CreateObject(light, new Vector3(0.0f, 3f, 0.0f), Quaternion.Identity);

			TextureModel box = new TextureModel(cubeVerts, cubeIndices, texCoords, cubeNormals, textureShader);
			box.TextureMaterial = new TextureMaterial(new Texture("Textures\\container2.png", TextureUnit.Texture0), new Texture("Textures\\container2_specular.png", TextureUnit.Texture1), new Texture("Textures\\blac.png", TextureUnit.Texture2), 32);
            box.OnLoad();

			TextureModel box2 = modelImporter.LoadTextureModel("Models\\Container.obj", textureShader, 0.01f);
            graphicsManager.CreateObject(box, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Identity);

            graphicsManager.CreateObject(box2, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Identity);
        }

        private void GenerateTestLights()
		{
			PointLight light = new PointLight(new Vector3(0.0f, 3f, 0.0f), new Vector3(1f), new Vector3(1.0f, 1.0f, 1.0f), 1f, 0.22f, 0.2f);
			graphicsManager.AddPointLight(light);

			DirectionalLight sun = new DirectionalLight((new Vector3(-1.0f, -0.5f, 0.0f)).Normalized(), new(0.7f, 0.6f, 0.2f), new(1.0f, 1.0f, 1.0f));
            graphicsManager.AddDirectionalLight(sun);
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