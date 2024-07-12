using OpenTK.Graphics.OpenGL4;
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

        // glfw: troca buffers e pesquisa eventos IO (teclas pressionadas/liberadas, mouse movido etc.)
        SwapBuffers();
    }

    private static void Main(string[] args) {
        GameWindowSettings gws = GameWindowSettings.Default;

        NativeWindowSettings nws = NativeWindowSettings.Default;
        nws.ClientSize = (SCR_WIDTH, SCR_HEIGHT);
        nws.Title = "LearnOpenGL";

        new Program(gws, nws).Run();
    }

    // processa todas as entradas: consulta ao GLFW se as teclas relevantes foram pressionadas/liberadas neste quadro e reage de acordo
    private void processInput() {
        if(KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
    }

    // glfw: sempre que o tamanho da janela for alterado (pelo SO ou redimensionamento do usuário) esta função de retorno de chamada é executada
    private void framebuffer_size_callback(int widht, int height) {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que largura e
        // a altura será significativamente maior do que a especificada em telas retina.
        GL.Viewport(0, 0, widht, height);
    }
}
