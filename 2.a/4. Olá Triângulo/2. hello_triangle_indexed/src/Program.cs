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

        // verifique se há erros de vinculação
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);

        if(success == 0) {
            GL.GetProgramInfoLog(shaderProgram, out infoLog);

            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    //private float[] vertices = {
    //     0.5f,  0.5f, 0.0f,  // top right
    //     0.5f, -0.5f, 0.0f,  // bottom right
    //    -0.5f, -0.5f, 0.0f,  // bottom left
    //    -0.5f,  0.5f, 0.0f   // top left  
    //};
    //private float[] vertices = {
    //    // first triangle
    //    -0.5f, -0.5f,  // bottom left
    //     0.5f, -0.5f,  // bottom right
    //     0.5f,  0.5f,  // top right

    //    // second triangle
    //    -0.5f, -0.5f,  // bottom left
    //     0.5f,  0.5f,  // top right
    //    -0.5f,  0.5f   // top left 
    //};
    private float[] vertices = {
        -0.5f, -0.5f,  // bottom left
         0.5f, -0.5f,  // bottom right
         0.5f,  0.5f,  // top right
        -0.5f,  0.5f   // top left 
    };

    private int[] indices = { // observe que começamos do 0!
        0, 1, 2,  // primeiro Triângulo
        0, 2, 3   // segundo Triângulo
    };

    private int VBO, VAO;

    private void DrawTriangle() {
        GL.GenVertexArrays(1, out VAO);
        GL.GenBuffers(1, out VBO);
        
        // vincule o objeto Vertex Array primeiro, depois vincule e defina buffer(s) de vértice(s) e, em seguida, configure atributos de vértice(s).
        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        this.setEBO();

        // observe que isso é permitido, a chamada para glVertexAttribPointer registrou VBO como o objeto de buffer de vértice vinculado do atributo de vértice para que depois possamos desvincular com segurança
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // lembre-se: NÃO desvincule o EBO enquanto um VAO estiver ativo, pois o objeto buffer do elemento vinculado ESTÁ armazenado no VAO; mantenha o EBO vinculado.
        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificando outro
        // De qualquer forma, o VAOs requer uma chamada para glBindVertexArray, portanto, geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        GL.BindVertexArray(0);

        // Remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    private int EBO; // Element Buffer Object

    private void setEBO() {
        GL.GenBuffers(1, out EBO);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
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
        //GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
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

        this.wireframeMode();
    }

    private bool isWireframe = false;

    private void wireframeMode() {
        if(KeyboardState.IsKeyDown(Keys.F3) && KeyboardState.IsKeyPressed(Keys.W)) {
            isWireframe = !isWireframe;

            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "isWireframe"), isWireframe ? 1 : 0);

            GL.PolygonMode(MaterialFace.FrontAndBack, isWireframe ? PolygonMode.Line : PolygonMode.Fill);
        }
    }

    private void framebuffer_size_callback(int widht, int height) {
        GL.Viewport(0, 0, widht, height);
    }
}
