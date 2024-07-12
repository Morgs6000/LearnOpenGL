using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearnOpenGL.src;

public class Program : GameWindow {
    // configurações
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    // criação de janela glfw
    private Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        framebuffer_size_callback(SCR_WIDTH, SCR_HEIGHT);
    }

    // construir e compilar nosso programa shader
    private void Shader() {
        // sombreador de vértice
        this._vertexShader();

        // sombreador de fragmento
        this._fragmentShader();

        // shaders de link
        
        
        

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private string vertexShaderSource = File.ReadAllText("../../../src/shaders/shader.vert");

    private int vertexShader;

    private void _vertexShader() {
        vertexShader = GL.CreateShader(ShaderType.VertexShader);

        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        int success;
        string infoLog;

        // verifique se há erros de compilação do shader
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);

        if(success == 0) {
            GL.GetShaderInfoLog(vertexShader, out infoLog);

            Console.WriteLine("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" + infoLog);
        }
    }

    private string fragmentShaderSource = File.ReadAllText("../../../src/shaders/shader.frag");

    private int fragmentShader;

    private void _fragmentShader() {
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        int success;
        string infoLog;

        // verifique se há erros de compilação do shader
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);

        if(success == 0) {
            GL.GetShaderInfoLog(fragmentShader, out infoLog);

            Console.WriteLine("ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" + infoLog);
        }
    }

    private int shaderProgram;

    private void _shaderProgram() {
        shaderProgram = GL.CreateProgram();

        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);

        GL.LinkProgram(shaderProgram);

        int success;
        string infoLog;

        // verifique se há erros de compilação do shader
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);

        if(success == 0) {
            GL.GetShaderInfoLog(shaderProgram, out infoLog);

            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }
    }

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    private float[] vertices = {
        -0.5f, -0.5f, // left  
         0.5f, -0.5f, // right 
         0.0f,  0.5f  // top  
    };
    //private float[] vertices = {
    //     0.0f,  0.0f, // left  
    //     1.0f,  0.0f, // right 
    //     0.5f,  1.0f  // top  
    //};

    private void DrawTriangle() {
        setVAO();
        setVBO();
        clearBind();

        // Remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    private int VAO; // Vertex Array Object

    private void setVAO() {
        GL.GenVertexArrays(1, out VAO);

        // vincule o objeto Vertex Array primeiro, depois vincule e defina buffer(s) de vértice(s) e, em seguida, configure atributos de vértice(s).
        GL.BindVertexArray(VAO);
    }

    private int VBO; // Vertex Buffer Object

    private void setVBO() {
        GL.GenBuffers(1, out VBO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    private void clearBind() {
        // observe que isso é permitido, a chamada para glVertexAttribPointer registrou VBO como o objeto de buffer de vértice vinculado do atributo de vértice para que depois possamos desvincular com segurança
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificando outro
        // De qualquer forma, o VAOs requer uma chamada para glBindVertexArray, portanto, geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        GL.BindVertexArray(0);
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

        // desenhe nosso primeiro triângulo
        GL.UseProgram(shaderProgram);
        GL.BindVertexArray(VAO); // visto que temos apenas um VAO, não há necessidade de vinculá-lo todas as vezes, mas faremos isso para manter as coisas um pouco mais organizadas
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        //GL.BindVertexArray(0); // não há necessidade de desvinculá-lo todas as vezes

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
