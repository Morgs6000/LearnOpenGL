using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearnOpenGL;

public class Program : GameWindow {
    // configurações
    const int SCR_WIDTH = 800;
    const int SCR_HEIGHT = 600;

    // criação de janela glfw
    private Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        framebuffer_size_callback(SCR_WIDTH, SCR_HEIGHT);
    }

    // loop de renderização
    protected override void OnRenderFrame(FrameEventArgs args) {
        // entrada
        processInput();

        // renderizar
        //GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        //GL.ClearColor(new Color4(0.2f, 0.3f, 0.3f, 1.0f));
        //GL.ClearColor(ConvertColorToRGBA(51, 76, 76, 255));
        GL.ClearColor(ConvertColorToHex("334C4C", 255));
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // glfw: troca buffers e pesquisa eventos IO (teclas pressionadas/liberadas, mouse movido etc.)
        SwapBuffers();
    }

    private Color4 ConvertColorToRGBA(int r, int g, int b, int a) {
        float fr = (float)r / 255;
        float fg = (float)g / 255;
        float fb = (float)b / 255;
        float fa = (float)a / 255;

        return new Color4(fr, fg, fb, fa);
    }

    private Color4 ConvertColorToHex(string hex, int a) {        
        int fr = Convert.ToInt32(hex.Substring(0, 2), 16);
        int fg = Convert.ToInt32(hex.Substring(2, 2), 16);
        int fb = Convert.ToInt32(hex.Substring(4, 2), 16);
        int fa = a / 255;

        return ConvertColorToRGBA(fr, fg, fb, fa);
    }

    private static void Main(string[] args) {
        GameWindowSettings gws = GameWindowSettings.Default;

        NativeWindowSettings nws = NativeWindowSettings.Default;
        nws.ClientSize = (SCR_WIDTH, SCR_HEIGHT);
        nws.Title = "LearnOpenGL";

        new Program(gws, nws).Run();
    }

    private void processInput() {
        if(KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
    }

    private void framebuffer_size_callback(int widht, int height) {
        GL.Viewport(0, 0, widht, height);
    }
}
