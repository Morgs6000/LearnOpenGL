using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Program {
    // settings
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    private const string vertexShaderSource = "#version 330 core\n" +
    "layout (location = 0) in vec3 aPos;\n" +
    "layout (location = 1) in vec3 aColor;\n" +
    "out vec3 ourColor;\n" +
    "void main() {\n" +
    "   gl_Position = vec4(aPos, 1.0);\n" +
    "   ourColor = aColor;\n" +
    "}\0";

    private const string fragmentShaderSource = "#version 330 core\n" +
    "out vec4 FragColor;\n" +
    "in vec3 ourColor;\n" +
    "void main() {\n" +
    "   FragColor = vec4(ourColor, 1.0f);\n" +
    "}\n\0";

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
        // shader de vértice
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        // verifica erros de compilação do shader
        int success;
        string infoLog;
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(vertexShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" + infoLog);
        }
        // shader de fragmento
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        // verifica erros de compilação do shader
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(fragmentShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" + infoLog);
        }
        // vincula shaders
        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        // verifica se há erros de vinculação
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);
        if(success == 0) {
            GL.GetProgramInfoLog(shaderProgram, out infoLog);
            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

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

        // como temos apenas um único shader, também poderíamos ativar nosso shader uma vez antes, se quisermos
        GL.UseProgram(shaderProgram);

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
        //GL.DeleteProgram(shaderProgram);

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
