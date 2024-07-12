using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

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
        // positions          // colors           // texture coords
         0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // top left  
    };

    private int[] indices = {
        0, 1, 3, // first triangle
        1, 2, 3  // second triangle
    };

    private int VAO; // Vertex Array Object
    private int VBO; // Vertex Buffer Object
    private int EBO; // Element Buffer Object

    private void DrawTriangle() {
        GL.GenVertexArrays(1, out VAO);
        GL.GenBuffers(1, out VBO);
        GL.GenBuffers(1, out EBO);
        // vincule o objeto Vertex Array primeiro, depois vincule e defina buffer(s) de vértice(s) e, em seguida, configure atributos de vértice(s).
        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

        // atributo de posição
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // atributo de cor
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        // atributo de coordenação de textura
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // observe que isso é permitido, a chamada para glVertexAttribPointer registrou VBO como o objeto de buffer de vértice vinculado do atributo de vértice para que depois possamos desvincular com segurança
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // Você pode desvincular o VAO posteriormente para que outras chamadas VAO não modifiquem acidentalmente este VAO, mas isso raramente acontece. Modificando outro
        // De qualquer forma, o VAOs requer uma chamada para glBindVertexArray, portanto, geralmente não desvinculamos VAOs (nem VBOs) quando não é diretamente necessário.
        GL.BindVertexArray(0);

        // Remova o comentário desta chamada para desenhar polígonos em wireframe.
        //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
    }

    // carrega e cria uma textura
    private int texture0, texture1;

    private void Texture() {
        ImageResult image;

        // textura 0
        GL.GenTextures(1, out texture0);
        GL.BindTexture(TextureTarget.Texture2D, texture0); // todas as próximas operações GL_TEXTURE_2D agora terão efeito neste objeto de textura

        // define os parâmetros de quebra de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o empacotamento de textura para GL_REPEAT (método de empacotamento padrão)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // define os parâmetros de filtragem de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        StbImage.stbi_set_flip_vertically_on_load(1);
        image = ImageResult.FromStream(File.OpenRead("../../../src/textures/container.jpg"), ColorComponents.RedGreenBlueAlpha);
        if(image.Data != null) {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        else {
            Console.WriteLine("Failed to load texture");
        }

        // textura 1
        GL.GenTextures(1, out texture1);
        GL.BindTexture(TextureTarget.Texture2D, texture1); // todas as próximas operações GL_TEXTURE_2D agora terão efeito neste objeto de textura

        // define os parâmetros de quebra de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o empacotamento de textura para GL_REPEAT (método de empacotamento padrão)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // define os parâmetros de filtragem de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        StbImage.stbi_set_flip_vertically_on_load(1);
        image = ImageResult.FromStream(File.OpenRead("../../../src/textures/awesomeface.png"), ColorComponents.RedGreenBlueAlpha);
        if(image.Data != null) {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        else {
            Console.WriteLine("Failed to load texture");
        }
    }

    protected override void OnLoad() {
        // construir e compilar nosso programa shader
        ourShader = new Shader("shader.vert", "shader.frag"); // você pode nomear seus arquivos de shader como quiser

        // diz ao opengl para cada sampler a qual unidade de textura ele pertence (só precisa ser feito uma vez)
        ourShader.use(); // não se esqueça de ativar/usar o shader antes de definir os uniformes!

        // configure-o manualmente assim:
        GL.Uniform1(GL.GetAttribLocation(ourShader.shaderProgram, "texture0"), 0);

        // ou configure-o através da classe de textura
        ourShader.setInt("texture1", 1);

        DrawTriangle();
        Texture();
    }

    // loop de renderização
    protected override void OnRenderFrame(FrameEventArgs args) {
        // entrada
        processInput();

        // renderizar
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // vincula texturas em unidades de textura correspondentes
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, texture0);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, texture1);

        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));

        Matrix4 view = Matrix4.Identity;
        // observe que estamos traduzindo a cena na direção inversa de onde queremos nos mover
        view *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, -3.0f));

        Matrix4 projection;
        projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);

        int modelLoc = GL.GetUniformLocation(ourShader.shaderProgram, "model");
        GL.UniformMatrix4(modelLoc, false, ref model);

        int viewLoc = GL.GetUniformLocation(ourShader.shaderProgram, "view");
        GL.UniformMatrix4(viewLoc, false, ref view);

        int projectionLoc = GL.GetUniformLocation(ourShader.shaderProgram, "projection");
        GL.UniformMatrix4(projectionLoc, false, ref projection);

        ourShader.use();

        // renderiza o triângulo        
        GL.BindVertexArray(VAO); // visto que temos apenas um VAO, não há necessidade de vinculá-lo todas as vezes, mas faremos isso para manter as coisas um pouco mais organizadas
        //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
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
    }

    private void framebuffer_size_callback(int widht, int height) {
        GL.Viewport(0, 0, widht, height);
    }
}
