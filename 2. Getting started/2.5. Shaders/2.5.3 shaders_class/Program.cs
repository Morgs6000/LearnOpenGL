using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearnOpenGL;

public class Program {
    // settings
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    private static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        var gws = GameWindowSettings.Default;

        var nws = NativeWindowSettings.Default;
        nws.ClientSize = (SCR_WIDTH, SCR_HEIGHT);
        nws.Title = "LearnOpenGL";

        // criação de janela glfw
        // --------------------
        var window = new GameWindow(gws, nws);
        window.CenterWindow();

        window.FramebufferResize += delegate(FramebufferResizeEventArgs args) {
            framebuffer_size_callback(window, SCR_WIDTH, SCR_HEIGHT);
        };

        // construir e compilar nosso programa shader
        // ------------------------------------
        Shader ourShader = new Shader("../../../shaders/shader_vs.glsl", "../../../shaders/shader_fs.glsl"); // você pode nomear seus arquivos de shader como quiser

        // configura dados de vértice (e buffer(s)) e configura atributos de vértice
        // ------------------------------------------------------------------
        float[] vertices = {
            // positions         // colors
             0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,  // bottom left
             0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f   // top  
        };

        int VBO, VAO;
        GL.GenVertexArrays(1, out VAO);
        GL.GenBuffers(1, out VBO);
        // vincule o objeto Vertex Array primeiro, depois vincule e defina buffer(s) de vértice(s) e então configure atributos de vértice(s).
        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // atributo de posição
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // atributo de cor
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Você pode desvincular o VAO posteriormente para que outras chamadas VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificar outros VAOs requer uma chamada para glBindVertexArray de qualquer maneira, então geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        //GL.BindVertexArray(0);

        // loop de renderização
        // -----------
        window.RenderFrame += delegate(FrameEventArgs args) {
            // entrada
            // -----
            processInput(window);

            // renderizar
            // ------
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // renderiza o triângulo
            ourShader.use();
            ourShader.setFloat("someUniform", 1.0f);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // glfw: troca buffers e pesquisa eventos IO (teclas pressionadas/liberadas, mouse movido etc.)
            // -------------------------------------------------------------------------------
            window.SwapBuffers();
        };

        // opcional: desalocar todos os recursos assim que eles tiverem sobrevivido ao seu propósito:
        // ------------------------------------------------------------------------
        //GL.DeleteVertexArrays(1, ref VAO);
        //GL.DeleteBuffers(1, ref VBO);

        window.Run();
    }

    // processar todas as entradas: consultar o GLFW se as teclas relevantes foram pressionadas/liberadas neste quadro e reagir de acordo
    // ---------------------------------------------------------------------------------------------------------
    private static void processInput(GameWindow window) {
        var input = window.KeyboardState;

        if(input.IsKeyDown(Keys.Escape)) {
            window.Close();
        }
    }

    // glfw: sempre que o tamanho da janela for alterado (por sistema operacional ou redimensionamento do usuário), esta função de retorno de chamada é executada
    // ---------------------------------------------------------------------------------------------
    private static void framebuffer_size_callback(GameWindow window, int width, int height) {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que a largura e a altura serão significativamente maiores do que as especificadas nas telas retina.
        GL.Viewport(0, 0, width, height);
    }
}
