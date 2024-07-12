using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System.Reflection;

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

        // configurar o estado opengl global
        // -----------------------------
        GL.Enable(EnableCap.DepthTest);

        // construir e compilar nosso programa shader
        // ------------------------------------
        Shader ourShader = new Shader("../../../shaders/shader_vs.glsl", "../../../shaders/shader_fs.glsl"); // você pode nomear seus arquivos de shader como quiser

        // configura dados de vértice (e buffer(s)) e configura atributos de vértice
        // ------------------------------------------------------------------
        float[] vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };
        // posições espaciais mundiais dos nossos cubos
        Vector3[] cubePositions = {
            new Vector3( 0.0f,  0.0f,  0.0f),
            new Vector3( 2.0f,  5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3( 2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f,  3.0f, -7.5f),
            new Vector3( 1.3f, -2.0f, -2.5f),
            new Vector3( 1.5f,  2.0f, -2.5f),
            new Vector3( 1.5f,  0.2f, -1.5f),
            new Vector3(-1.3f,  1.0f, -1.5f)
        };

        int VBO, VAO;
        GL.GenVertexArrays(1, out VAO);
        GL.GenBuffers(1, out VBO);

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // atributo de posição
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // atributo de coordenação de textura
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // carrega e cria uma textura
        // -------------------------
        int texture1, texture2;
        // textura 1
        // ---------
        GL.GenTextures(1, out texture1);
        GL.BindTexture(TextureTarget.Texture2D, texture1);
        // define os parâmetros de quebra de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o empacotamento de textura para GL_REPEAT (método de empacotamento padrão)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        // define os parâmetros de filtragem de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        // carrega a imagem, cria textura e gera mipmaps
        int widht, height, nrChannels;
        // O FileSystem::getPath(...) faz parte do repositório GitHub para que possamos encontrar arquivos em qualquer IDE/plataforma; substitua-o pelo seu próprio caminho de imagem.
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(File.OpenRead("../../../textures/container.jpg"), ColorComponents.RedGreenBlueAlpha);
        if(image.Data != null) {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        else {
            Console.WriteLine("Failed to load texture");
        }
        // textura 2
        // ---------
        GL.GenTextures(1, out texture2);
        GL.BindTexture(TextureTarget.Texture2D, texture2);
        // define os parâmetros de quebra de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // define o empacotamento de textura para GL_REPEAT (método de empacotamento padrão)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        // define os parâmetros de filtragem de textura
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        // carrega a imagem, cria textura e gera mipmaps
        StbImage.stbi_set_flip_vertically_on_load(1);
        image = ImageResult.FromStream(File.OpenRead("../../../textures/awesomeface.png"), ColorComponents.RedGreenBlueAlpha);
        if(image.Data != null) {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        else {
            Console.WriteLine("Failed to load texture");
        }

        // diz ao opengl para cada sampler a qual unidade de textura ele pertence (só precisa ser feito uma vez)
        // -------------------------------------------------------------------------------------------
        ourShader.use();
        ourShader.setInt("texture1", 0);
        ourShader.setInt("texture2", 1);

        // passe a matriz de projeção para o shader (como a matriz de projeção raramente muda, não há necessidade de fazer isso por quadro)
        // -----------------------------------------------------------------------------------------------------------
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
        ourShader.setMat4("projection", projection);

        // loop de renderização
        // -----------
        window.RenderFrame += delegate(FrameEventArgs args) {
            // entrada
            // -----
            processInput(window);

            // renderizar
            // ------
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // também limpe o buffer de profundidade agora!

            // vincula texturas em unidades de textura correspondentes
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2);

            // ativar shader
            ourShader.use();

            // transformação de câmera/visualização            
            Matrix4 view = Matrix4.Identity; // certifique-se de inicializar a matriz para a matriz identidade primeiro
            float radius = 10.0f;
            float camX = (float)(Math.Sin(GLFW.GetTime()) * radius);
            float camZ = (float)(Math.Cos(GLFW.GetTime()) * radius);
            view *= Matrix4.LookAt(new Vector3(camX, 1.0f, camZ), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            ourShader.setMat4("view", view);            

            // renderiza caixas
            GL.BindVertexArray(VAO);
            for(int i = 0; i < 10; i++) {
                // calcula a matriz do modelo para cada objeto e passa para o shader antes de desenhar
                Matrix4 model = Matrix4.Identity;
                float angle = 20.0f * i;
                model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), MathHelper.DegreesToRadians(angle));
                model *= Matrix4.CreateTranslation(cubePositions[i]);
                ourShader.setMat4("model", model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }            

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
