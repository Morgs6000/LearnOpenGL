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
    "void main() {\n" +
    "   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n" +
    "}\0";

    private const string fragmentShader1Source = "#version 330 core\n" +
    "out vec4 FragColor;\n" +
    "void main() {\n" +
    "   FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n" +
    "}\n\0";

    private const string fragmentShader2Source = "#version 330 core\n" +
    "out vec4 FragColor;\n" +
    "void main() {\n" +
    "   FragColor = vec4(1.0f, 1.0f, 0.0f, 1.0f);\n" +
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
        // desta vez ignoramos as verificações de log de compilação para facilitar a leitura (se você encontrar problemas, adicione as verificações de compilação! consulte os exemplos de código anteriores)
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShaderOrange = GL.CreateShader(ShaderType.FragmentShader); // o primeiro shader de fragmento que gera a cor laranja
        int fragmentShaderYellow = GL.CreateShader(ShaderType.FragmentShader); // o segundo shader de fragmento que gera a cor amarela
        int shaderProgramOrange = GL.CreateProgram();
        int shaderProgramYellow = GL.CreateProgram(); // o segundo programa shader
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        GL.ShaderSource(fragmentShaderOrange, fragmentShader1Source);
        GL.CompileShader(fragmentShaderOrange);
        GL.ShaderSource(fragmentShaderYellow, fragmentShader2Source);
        GL.CompileShader(fragmentShaderYellow);
        // vincula o primeiro objeto do programa
        GL.AttachShader(shaderProgramOrange, vertexShader);
        GL.AttachShader(shaderProgramOrange, fragmentShaderOrange);
        GL.LinkProgram(shaderProgramOrange);
        // então vincule o segundo objeto do programa usando um shader de fragmento diferente (mas o mesmo shader de vértice)
        // isso é perfeitamente permitido, pois as entradas e saídas dos shaders de vértice e de fragmento são igualmente correspondentes.
        GL.AttachShader(shaderProgramYellow, vertexShader);
        GL.AttachShader(shaderProgramYellow, fragmentShaderYellow);
        GL.LinkProgram(shaderProgramYellow);

        // configura dados de vértice (e buffer(s)) e configura atributos de vértice
        // ------------------------------------------------------------------
        float[] firstTriangle = {
            -0.9f, -0.5f, 0.0f,  // left 
            -0.0f, -0.5f, 0.0f,  // right
            -0.45f, 0.5f, 0.0f,  // top 
        };
        float[] secondTriangle = {
            0.0f, -0.5f, 0.0f,  // left
            0.9f, -0.5f, 0.0f,  // right
            0.45f, 0.5f, 0.0f   // top 
        };

        int[] VBOs = new int[2];
        int[] VAOs = new int[2];
        GL.GenVertexArrays(2, VAOs); // também podemos gerar vários VAOs ou buffers ao mesmo tempo
        GL.GenBuffers(2, VBOs);
        // configuração do primeiro triângulo
        // --------------------
        GL.BindVertexArray(VAOs[0]);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[0]);
        GL.BufferData(BufferTarget.ArrayBuffer, firstTriangle.Length * sizeof(float), firstTriangle, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Os atributos do vértice permanecem os mesmos
        GL.EnableVertexAttribArray(0);
        //GL.BindVertexArray(0); // não há necessidade de desvincular, pois vinculamos diretamente um VAO diferente nas próximas linhas
        // configuração do segundo triângulo
        // ---------------------
        GL.BindVertexArray(VAOs[1]); // observe que agora vinculamos a um VAO diferente
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[1]); // e um VBO diferente
        GL.BufferData(BufferTarget.ArrayBuffer, secondTriangle.Length * sizeof(float), secondTriangle, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0); // como os dados do vértice estão compactados, também podemos especificar 0 como o passo do atributo do vértice para permitir que o OpenGL descubra isso
        GL.EnableVertexAttribArray(0);
        //GL.BindVertexArray(0); // também não é realmente necessário, mas cuidado com chamadas que podem afetar VAOs enquanto este estiver vinculado (como vincular objetos de buffer de elemento ou ativar/desativar atributos de vértice)

        // remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

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

            // agora, quando desenharmos o triângulo, primeiro usaremos o vértice e o shader de fragmento laranja do primeiro programa
            GL.UseProgram(shaderProgramOrange);
            // desenha o primeiro triângulo usando os dados do primeiro VAO
            GL.BindVertexArray(VAOs[0]);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3); // esta chamada deve gerar um triângulo laranja
            // então desenhamos o segundo triângulo usando os dados do segundo VAO
            // quando desenhamos o segundo triângulo, queremos usar um programa de shader diferente, então mudamos para o programa de shader com nosso shader de fragmento amarelo.
            GL.UseProgram(shaderProgramYellow);
            GL.BindVertexArray(VAOs[1]);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3); // esta chamada deve gerar um triângulo amarelo

            // glfw: troca buffers e pesquisa eventos IO (teclas pressionadas/liberadas, mouse movido etc.)
            // -------------------------------------------------------------------------------
            window.SwapBuffers();
        };

        // opcional: desalocar todos os recursos assim que eles tiverem sobrevivido ao seu propósito:
        // ------------------------------------------------------------------------
        //GL.DeleteVertexArrays(2, VAOs);
        //GL.DeleteBuffers(2, VBOs);
        //GL.DeleteProgram(shaderProgramOrange);
        //GL.DeleteProgram(shaderProgramYellow);

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
