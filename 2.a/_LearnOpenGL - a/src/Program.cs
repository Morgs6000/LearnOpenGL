using OpenTK.Windowing.Desktop;

namespace LearnOpenGL.src;

public class Program : GameWindow {
    // configurações
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    // criação de janela glfw
    private Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    private static void Main(string[] args) {
        GameWindowSettings gws = GameWindowSettings.Default;

        NativeWindowSettings nws = NativeWindowSettings.Default;
        nws.ClientSize = (SCR_WIDTH, SCR_HEIGHT);
        nws.Title = "LearnOpenGL";

        new Program(gws, nws).Run();
    }
}
