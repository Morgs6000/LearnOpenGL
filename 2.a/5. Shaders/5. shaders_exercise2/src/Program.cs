using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearnOpenGL.src;

public class Program : GameWindow {
    // configurações
    private const int SCR_WIDTH = 800;
    private const int SCR_HEIGHT = 600;

    private Shader ourShader;

    // criação de janela glfw
    private Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        framebuffer_size_callback(SCR_WIDTH, SCR_HEIGHT);
    }

    // configurar dados de vértice (e buffer(s)) e configurar atributos de vértice
    private float[] vertices = {
        // positions         // colors
         0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,  // bottom right
        -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,  // bottom left
         0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f   // top 
    };

    private int VBO, VAO;

    private void DrawTriangle() {
        GL.GenVertexArrays(1, out VAO);
        GL.GenBuffers(1, out VBO);
        // vincule o objeto Vertex Array primeiro, depois vincule e defina buffer(s) de vértice(s) e, em seguida, configure atributos de vértice(s).
        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // atributo de posição
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // atributo de cor
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // observe que isso é permitido, a chamada para glVertexAttribPointer registrou VBO como o objeto de buffer de vértice vinculado do atributo de vértice para que depois possamos desvincular com segurança
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificando outro
        // De qualquer forma, o VAOs requer uma chamada para glBindVertexArray, portanto, geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        GL.BindVertexArray(0);

        // Remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    protected override void OnLoad() {
        // construir e compilar nosso programa shader
        ourShader = new Shader("shader.vert", "shader.frag"); // você pode nomear seus arquivos de shader como quiser

        DrawTriangle();
    }

    // loop de renderização
    protected override void OnRenderFrame(FrameEventArgs args) {
        // entrada
        processInput();

        // renderizar
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // renderiza o triângulo
        ourShader.use();

        float offset = 0.5f;
        ourShader.setFloat("xOffset", offset);

        ourShader.setFloat("someUniform", 1.0f);
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
