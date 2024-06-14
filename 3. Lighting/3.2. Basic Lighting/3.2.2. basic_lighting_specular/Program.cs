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

    // camera
    private static Camera camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f));
    private static float lastX = SCR_WIDTH / 2.0f;
    private static float lastY = SCR_HEIGHT / 2.0f;
    private static bool firstMouse = true;

    // tempo
    private static float deltaTime = 0.0f;
    private static float lastFrame = 0.0f;

    // iluminação
    private static Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

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

        window.UpdateFrame += delegate(FrameEventArgs args) {
            mouse_callback(window, window.MouseState.X, window.MouseState.Y);
        };

        window.MouseWheel += delegate(MouseWheelEventArgs args) {
            scroll_callback(window, args.OffsetX, args.OffsetY);
        };

        // diz ao GLFW para capturar nosso mouse
        window.CursorState = CursorState.Grabbed;

        // configurar o estado opengl global
        // -----------------------------
        GL.Enable(EnableCap.DepthTest);

        // construir e compilar nosso programa shader
        // ------------------------------------
        Shader lightingShader = new Shader("../../../shaders/basic_lighting_vs.glsl", "../../../shaders/basic_lighting_fs.glsl");
        Shader lightCubeShader = new Shader("../../../shaders/light_cube_vs.glsl", "../../../shaders/light_cube_fs.glsl");

        // configura dados de vértice (e buffer(s)) e configura atributos de vértice
        // ------------------------------------------------------------------
        float[] vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };
        // primeiro, configure o VAO (e VBO) do cubo
        int VBO, cubeVAO;
        GL.GenVertexArrays(1, out cubeVAO);
        GL.GenBuffers(1, out VBO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindVertexArray(cubeVAO);

        // atributo de posição
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // atributo normal
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // segundo, configure o VAO da luz (o VBO permanece o mesmo; os vértices são os mesmos para o objeto de luz que também é um cubo 3D)
        int lightCubeVAO;
        GL.GenVertexArrays(1, out lightCubeVAO);
        GL.BindVertexArray(lightCubeVAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        // observe que atualizamos o passo do atributo de posição da lâmpada para refletir os dados atualizados do buffer
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // loop de renderização
        // -----------
        window.RenderFrame += delegate(FrameEventArgs args) {
            // lógica de tempo por quadro
            // --------------------
            float currentFrame = (float)(GLFW.GetTime());
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            // entrada
            // -----
            processInput(window);

            // renderizar
            // ------
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // certifique-se de ativar o shader ao definir uniformes/objetos de desenho
            lightingShader.use();
            lightingShader.setVec3("objectColor", 1.0f, 0.5f, 0.31f);
            lightingShader.setVec3("lightColor", 1.0f, 1.0f, 1.0f);
            lightingShader.setVec3("lightPos", lightPos);
            lightingShader.setVec3("viewPos", camera.Position);

            //transformações de visualização/projeção
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            Matrix4 view = camera.GetViewMatrix();
            lightingShader.setMat4("projection", projection);
            lightingShader.setMat4("view", view);

            // transformação mundial
            Matrix4 model = Matrix4.Identity;
            lightingShader.setMat4("model", model);

            //renderiza o cubo
            GL.BindVertexArray(cubeVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            // também desenha o objeto lâmpada
            lightCubeShader.use();
            lightCubeShader.setMat4("projection", projection);
            lightCubeShader.setMat4("view", view);
            model = Matrix4.Identity;
            model *= Matrix4.CreateScale(new Vector3(0.2f)); // um cubo menor
            model *= Matrix4.CreateTranslation(lightPos);
            lightCubeShader.setMat4("model", model);

            GL.BindVertexArray(lightCubeVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);           

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

        if(input.IsKeyDown(Keys.W)) {
            camera.ProcessKeyboard(Camera_Movement.FORWARD, deltaTime);
        }
        if(input.IsKeyDown(Keys.S)) {
            camera.ProcessKeyboard(Camera_Movement.BACKWARD, deltaTime);
        }
        if(input.IsKeyDown(Keys.A)) {
            camera.ProcessKeyboard(Camera_Movement.LEFT, deltaTime);
        }
        if(input.IsKeyDown(Keys.D)) {
            camera.ProcessKeyboard(Camera_Movement.RIGHT, deltaTime);
        }

        if(input.IsKeyDown(Keys.Space)) {
            camera.ProcessKeyboard(Camera_Movement.UP, deltaTime);
        }
        if(input.IsKeyDown(Keys.LeftShift)) {
            camera.ProcessKeyboard(Camera_Movement.DOWN, deltaTime);
        }
    }

    // glfw: sempre que o tamanho da janela for alterado (por sistema operacional ou redimensionamento do usuário), esta função de retorno de chamada é executada
    // ---------------------------------------------------------------------------------------------
    private static void framebuffer_size_callback(GameWindow window, int width, int height) {
        // certifique-se de que a viewport corresponda às novas dimensões da janela; observe que a largura e a altura serão significativamente maiores do que as especificadas nas telas retina.
        GL.Viewport(0, 0, width, height);
    }

    // glfw: sempre que o mouse se move, esse retorno de chamada é chamado
    // -------------------------------------------------------
    private static void mouse_callback(GameWindow window, double xposIn, double yposIn) {
        float xpos = (float)(xposIn);
        float ypos = (float)(yposIn);

        if(firstMouse) {
            lastX = xpos;
            lastY = ypos;
            firstMouse = false;
        }

        float xoffset = xpos - lastX;
        float yoffset = lastY - ypos; // invertido já que as coordenadas y vão de baixo para cima

        lastX = xpos;
        lastY = ypos;

        camera.ProcessMouseMovement(xoffset, yoffset);
    }

    // glfw: sempre que a roda de rolagem do mouse rola, esse retorno de chamada é chamado
    // ----------------------------------------------------------------------
    private static void scroll_callback(GameWindow window, double xoffset, double yoffset) {
        camera.ProcessMouseScroll((float)(yoffset));
    }
}
