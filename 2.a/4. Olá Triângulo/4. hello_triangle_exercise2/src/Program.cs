using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearnOpenGL.src;

public class Program : GameWindow {
    // configurações
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    private string vertexShaderSource = File.ReadAllText("../../../src/shaders/shader.vert");
    private string fragmentShaderSource = File.ReadAllText("../../../src/shaders/shader.frag");

    // criação de janela glfw
    private Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        framebuffer_size_callback(SCR_WIDTH, SCR_HEIGHT);
    }

    // construir e compilar nosso programa shader
    private int shaderProgram;

    private void Shader() {
        // sombreador de vértice
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        // verifique se há erros de compilação do shader
        int success;
        string infoLog;
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(vertexShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" + infoLog);
        }

        // sombreador de fragmento
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        // verifique se há erros de compilação do shader
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(fragmentShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" + infoLog);
        }

        // shaders de link
        shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        // verifique se há erros de compilação do shader
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(shaderProgram, out infoLog);
            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    private float[] firstTriangle = {
         // first triangle
        -0.9f, -0.5f, 0.0f,  // left 
        -0.0f, -0.5f, 0.0f,  // right
        -0.45f, 0.5f, 0.0f,  // top 
    };
    private float[] secondTriangle = {
        // second triangle
         0.0f, -0.5f, 0.0f,  // left
         0.9f, -0.5f, 0.0f,  // right
         0.45f, 0.5f, 0.0f   // top  
    };

    private int[] VBOs = new int[2];
    private int[] VAOs = new int[2];

    private void DrawTriangle() {
        GL.GenVertexArrays(2, VAOs); // também podemos gerar vários VAOs ou buffers ao mesmo tempo
        GL.GenBuffers(2, VBOs);

        // configuração do primeiro triângulo
        GL.BindVertexArray(VAOs[0]);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[0]);
        GL.BufferData(BufferTarget.ArrayBuffer, firstTriangle.Length * sizeof(float), firstTriangle, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Os atributos do vértice permanecem os mesmos
        GL.EnableVertexAttribArray(0);
        //GL.BindVertexArray(0); // não há necessidade de desvincular, pois vinculamos diretamente um VAO diferente nas próximas linhas

        // configuração do segundo triângulo
        GL.BindVertexArray(VAOs[1]); // observe que agora vinculamos a um VAO diferente
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[1]); // e um VBO diferente
        GL.BufferData(BufferTarget.ArrayBuffer, secondTriangle.Length * sizeof(float), secondTriangle, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Os atributos do vértice permanecem os mesmos
        GL.EnableVertexAttribArray(0);
        //GL.BindVertexArray(0); // não há necessidade de desvincular, pois vinculamos diretamente um VAO diferente nas próximas linhas

        // Remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    protected override void OnLoad() {
        Shader();
        DrawTriangle();
    }

    // loop de renderização
    protected override void OnRenderFrame(FrameEventArgs args) {
        // entrada
        processInput();

        // renderizar
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(shaderProgram);

        // desenha o primeiro triângulo usando os dados do primeiro VAO
        GL.BindVertexArray(VAOs[0]);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        // então desenhamos o segundo triângulo usando os dados do segundo VAO
        GL.BindVertexArray(VAOs[1]);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

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

    private void processInput() {
        if(KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
    }

    private void framebuffer_size_callback(int widht, int height) {
        GL.Viewport(0, 0, widht, height);
    }
}
