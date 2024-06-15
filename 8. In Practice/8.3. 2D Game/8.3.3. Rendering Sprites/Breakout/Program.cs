using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Breakout;

public class Program {
    // A largura da tela
    private const int SCREEN_WIDTH = 800;
    // A altura da tela
    private const int SCREEN_HEIGHT = 600;

    private static Game Breakout = new Game(SCREEN_WIDTH, SCREEN_HEIGHT);

    private static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        var gws = GameWindowSettings.Default;

        var nws = NativeWindowSettings.Default;
        nws.ClientSize = (SCREEN_WIDTH, SCREEN_HEIGHT);
        nws.Title = "Breakout";

        var window = new GameWindow(gws, nws);
        window.CenterWindow();

        window.UpdateFrame += delegate(FrameEventArgs args) {
            key_callback(window, window.KeyboardState);
        };

        window.FramebufferResize += delegate(FramebufferResizeEventArgs args) {
            framebuffer_size_callback(window, SCREEN_WIDTH, SCREEN_HEIGHT);
        };

        // Configuração OpenGL
        // --------------------
        //GL.Viewport(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // inicializar jogo
        // ---------------
        window.Load += delegate() {
            Breakout.Init();
        };

        // variáveis ​​deltaTime
        // -------------------
        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        window.RenderFrame += delegate (FrameEventArgs args) {
            // calcular o tempo delta
            // --------------------
            float currentFrame = (float)GLFW.GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            // gerenciar a entrada do usuário
            // -----------------
            Breakout.ProcessInput(deltaTime);

            // atualizar o estado do jogo
            // -----------------
            Breakout.Update(deltaTime);

            // renderizar
            // ------
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Breakout.Render();

            window.SwapBuffers();
        };

        // exclua todos os recursos carregados usando o gerenciador de recursos
        // ---------------------------------------------------------
        ResourceManager.Clear();

        window.Run();
    }

    private static void key_callback(GameWindow window, KeyboardState key/*, int scancode, int action, int mode*/) {
        // quando um usuário pressiona a tecla escape, definimos a propriedade WindowShouldClose como true, fechando a aplicação
        if(key.IsKeyDown(Keys.Escape)) {
            window.Close();
        }
    }

    private static void framebuffer_size_callback(GameWindow window, int width, int height) {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que largura e
        // a altura será significativamente maior do que a especificada em telas retina.
        GL.Viewport(0, 0, width, height);
    }
}