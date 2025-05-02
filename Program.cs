using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using TerraWeaverGraphics;

namespace TerraWeaver
{
	public static class Program
	{
		private static void Main()
		{
			var nativeWindowSettings = new NativeWindowSettings()
			{
				ClientSize = new Vector2i(1920, 1080),
				Title = "TerraWeaver",
				Flags = ContextFlags.ForwardCompatible,
			};

			using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
			{
				window.Run();
			}
		}
	}
}