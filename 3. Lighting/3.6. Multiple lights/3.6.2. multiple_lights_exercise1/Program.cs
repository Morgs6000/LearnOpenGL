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

    // lighting
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
        Shader lightingShader = new Shader("../../../shaders/materials_vs.glsl", "../../../shaders/materials_fs.glsl");
        Shader lightCubeShader = new Shader("../../../shaders/light_cube_vs.glsl", "../../../shaders/light_cube_fs.glsl");

        // configura dados de vértice (e buffer(s)) e configura atributos de vértice
        // ------------------------------------------------------------------
        float[] vertices = {
            // positions          // normals           // texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f
        };
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
        // positions of the point lights
        Vector3[] pointLightPositions = {
            new Vector3( 0.7f,  0.2f,  2.0f),
            new Vector3( 2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f,  2.0f, -12.0f),
            new Vector3( 0.0f,  0.0f, -3.0f)
        };
        // primeiro, configure o VAO (e VBO) do cubo
        int VBO, cubeVAO;
        GL.GenVertexArrays(1, out cubeVAO);
        GL.GenBuffers(1, out VBO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindVertexArray(cubeVAO);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // segundo, configure o VAO da luz (o VBO permanece o mesmo; os vértices são os mesmos para o objeto de luz que também é um cubo 3D)
        int lightCubeVAO;
        GL.GenVertexArrays(1, out lightCubeVAO);
        GL.BindVertexArray(lightCubeVAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        // observe que atualizamos o passo do atributo de posição da lâmpada para refletir os dados atualizados do buffer
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // carregar texturas (agora usamos uma função utilitária para manter o código mais organizado)
        // -----------------------------------------------------------------------------
        int diffuseMap = loadTexture("../../../textures/container2.png");
        int specularMap = loadTexture("../../../textures/container2_specular.png");

        // configuração do sombreador
        // --------------------
        lightingShader.use();
        lightingShader.setInt("material.diffuse", 0);
        lightingShader.setInt("material.specular", 1);

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
            // == ==============================================================================================
            //       DESERT
            // == ==============================================================================================
            GL.ClearColor(0.75f, 0.52f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector3[] pointLightColors = {
                new Vector3(1.0f, 0.6f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.2f, 0.2f, 1.0f)
            };

            // certifique-se de ativar o shader ao definir uniformes/objetos de desenho
            lightingShader.use();
            lightingShader.setVec3("viewPos", camera.Position);
            lightingShader.setFloat("material.shininess", 32.0f);

            /*
               Aqui montamos todos os uniformes para os tipos de luzes 5/6 que temos. Temos que defini-los manualmente e indexar a estrutura PointLight adequada no array para definir cada variável uniforme. Isso pode ser feito de forma mais amigável ao código, definindo tipos de luz como classes e definindo seus valores lá, ou usando uma abordagem uniforme mais eficiente usando 'Objetos de buffer uniformes', mas isso é algo que discutiremos no 'GLSL Avançado ' tutorial.
            */
            // Directional light
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "dirLight.direction"), -0.2f, -1.0f, -0.3f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "dirLight.ambient"), 0.3f, 0.24f, 0.14f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "dirLight.diffuse"), 0.7f, 0.42f, 0.26f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "dirLight.specular"), 0.5f, 0.5f, 0.5f);
            // Point light 1
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].position"), pointLightPositions[0]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].ambient"), pointLightColors[0] * 0.1f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].diffuse"), pointLightColors[0]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].specular"), pointLightColors[0]);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[0].quadratic"), 0.032f);
            // Point light 2
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].position"), pointLightPositions[1]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].ambient"), pointLightColors[1] * 0.1f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].diffuse"), pointLightColors[1]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].specular"), pointLightColors[1]);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[1].quadratic"), 0.032f);
            // Point light 3
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].position"), pointLightPositions[2]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].ambient"), pointLightColors[2] * 0.1f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].diffuse"), pointLightColors[2]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].specular"), pointLightColors[2]);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[2].quadratic"), 0.032f);
            // Point light 4
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].position"), pointLightPositions[3]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].ambient"), pointLightColors[3] * 0.1f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].diffuse"), pointLightColors[3]);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].specular"), pointLightColors[3]);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "pointLights[3].quadratic"), 0.032f);
            // SpotLight
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "spotLight.position"), camera.Position);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "spotLight.direction"), camera.Front);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "spotLight.ambient"), 0.0f, 0.0f, 0.0f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "spotLight.diffuse"), 0.8f, 0.8f, 0.0f);
            GL.Uniform3(GL.GetUniformLocation(lightingShader.ID, "spotLight.specular"), 0.8f, 0.8f, 0.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "spotLight.constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "spotLight.linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "spotLight.quadratic"), 0.032f);
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "spotLight.cutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));
            GL.Uniform1(GL.GetUniformLocation(lightingShader.ID, "spotLight.outerCutOff"), (float)Math.Cos(MathHelper.DegreesToRadians(13.0f)));
            // == ==============================================================================================
            //       FACTORY
            // == ==============================================================================================
//            glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
//            [...]
//            glm::vec3 pointLightColors[] = {
//    glm::vec3(0.2f, 0.2f, 0.6f),
//    glm::vec3(0.3f, 0.3f, 0.7f),
//    glm::vec3(0.0f, 0.0f, 0.3f),
//    glm::vec3(0.4f, 0.4f, 0.4f)
//};
//            [...]
//            // Directional light
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.direction"), -0.2f, -1.0f, -0.3f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.ambient"), 0.05f, 0.05f, 0.1f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.diffuse"), 0.2f, 0.2f, 0.7);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.specular"), 0.7f, 0.7f, 0.7f);
//            // Point light 1
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].position"), pointLightPositions[0].x, pointLightPositions[0].y, pointLightPositions[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].ambient"), pointLightColors[0].x * 0.1, pointLightColors[0].y * 0.1, pointLightColors[0].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].diffuse"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].specular"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].linear"), 0.09);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].quadratic"), 0.032);
//            // Point light 2
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].position"), pointLightPositions[1].x, pointLightPositions[1].y, pointLightPositions[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].ambient"), pointLightColors[1].x * 0.1, pointLightColors[1].y * 0.1, pointLightColors[1].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].diffuse"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].specular"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].linear"), 0.09);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].quadratic"), 0.032);
//            // Point light 3
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].position"), pointLightPositions[2].x, pointLightPositions[2].y, pointLightPositions[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].ambient"), pointLightColors[2].x * 0.1, pointLightColors[2].y * 0.1, pointLightColors[2].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].diffuse"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].specular"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].linear"), 0.09);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].quadratic"), 0.032);
//            // Point light 4
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].position"), pointLightPositions[3].x, pointLightPositions[3].y, pointLightPositions[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].ambient"), pointLightColors[3].x * 0.1, pointLightColors[3].y * 0.1, pointLightColors[3].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].diffuse"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].specular"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].linear"), 0.09);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].quadratic"), 0.032);
//            // SpotLight
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.position"), camera.Position.x, camera.Position.y, camera.Position.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.direction"), camera.Front.x, camera.Front.y, camera.Front.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.ambient"), 0.0f, 0.0f, 0.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.diffuse"), 1.0f, 1.0f, 1.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.specular"), 1.0f, 1.0f, 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.linear"), 0.009);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.quadratic"), 0.0032);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.cutOff"), glm::cos(glm::radians(10.0f)));
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.outerCutOff"), glm::cos(glm::radians(12.5f)));
//            // == ==============================================================================================
//            //       HORROR
//            // == ==============================================================================================
//            glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
//            [...]
//            glm::vec3 pointLightColors[] = {
//    glm::vec3(0.1f, 0.1f, 0.1f),
//    glm::vec3(0.1f, 0.1f, 0.1f),
//    glm::vec3(0.1f, 0.1f, 0.1f),
//    glm::vec3(0.3f, 0.1f, 0.1f)
//};
//            [...]
//            // Directional light
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.direction"), -0.2f, -1.0f, -0.3f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.ambient"), 0.0f, 0.0f, 0.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.diffuse"), 0.05f, 0.05f, 0.05);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.specular"), 0.2f, 0.2f, 0.2f);
//            // Point light 1
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].position"), pointLightPositions[0].x, pointLightPositions[0].y, pointLightPositions[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].ambient"), pointLightColors[0].x * 0.1, pointLightColors[0].y * 0.1, pointLightColors[0].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].diffuse"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].specular"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].linear"), 0.14);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].quadratic"), 0.07);
//            // Point light 2
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].position"), pointLightPositions[1].x, pointLightPositions[1].y, pointLightPositions[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].ambient"), pointLightColors[1].x * 0.1, pointLightColors[1].y * 0.1, pointLightColors[1].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].diffuse"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].specular"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].linear"), 0.14);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].quadratic"), 0.07);
//            // Point light 3
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].position"), pointLightPositions[2].x, pointLightPositions[2].y, pointLightPositions[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].ambient"), pointLightColors[2].x * 0.1, pointLightColors[2].y * 0.1, pointLightColors[2].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].diffuse"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].specular"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].linear"), 0.22);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].quadratic"), 0.20);
//            // Point light 4
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].position"), pointLightPositions[3].x, pointLightPositions[3].y, pointLightPositions[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].ambient"), pointLightColors[3].x * 0.1, pointLightColors[3].y * 0.1, pointLightColors[3].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].diffuse"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].specular"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].linear"), 0.14);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].quadratic"), 0.07);
//            // SpotLight
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.position"), camera.Position.x, camera.Position.y, camera.Position.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.direction"), camera.Front.x, camera.Front.y, camera.Front.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.ambient"), 0.0f, 0.0f, 0.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.diffuse"), 1.0f, 1.0f, 1.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.specular"), 1.0f, 1.0f, 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.linear"), 0.09);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.quadratic"), 0.032);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.cutOff"), glm::cos(glm::radians(10.0f)));
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.outerCutOff"), glm::cos(glm::radians(15.0f)));
//            // == ==============================================================================================
//            //       BIOCHEMICAL LAB
//            // == ==============================================================================================
//            glClearColor(0.9f, 0.9f, 0.9f, 1.0f);
//            [...]
//            glm::vec3 pointLightColors[] = {
//    glm::vec3(0.4f, 0.7f, 0.1f),
//    glm::vec3(0.4f, 0.7f, 0.1f),
//    glm::vec3(0.4f, 0.7f, 0.1f),
//    glm::vec3(0.4f, 0.7f, 0.1f)
//};
//            [...]
//            // Directional light
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.direction"), -0.2f, -1.0f, -0.3f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.ambient"), 0.5f, 0.5f, 0.5f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.diffuse"), 1.0f, 1.0f, 1.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "dirLight.specular"), 1.0f, 1.0f, 1.0f);
//            // Point light 1
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].position"), pointLightPositions[0].x, pointLightPositions[0].y, pointLightPositions[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].ambient"), pointLightColors[0].x * 0.1, pointLightColors[0].y * 0.1, pointLightColors[0].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].diffuse"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[0].specular"), pointLightColors[0].x, pointLightColors[0].y, pointLightColors[0].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].linear"), 0.07);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[0].quadratic"), 0.017);
//            // Point light 2
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].position"), pointLightPositions[1].x, pointLightPositions[1].y, pointLightPositions[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].ambient"), pointLightColors[1].x * 0.1, pointLightColors[1].y * 0.1, pointLightColors[1].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].diffuse"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[1].specular"), pointLightColors[1].x, pointLightColors[1].y, pointLightColors[1].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].linear"), 0.07);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[1].quadratic"), 0.017);
//            // Point light 3
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].position"), pointLightPositions[2].x, pointLightPositions[2].y, pointLightPositions[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].ambient"), pointLightColors[2].x * 0.1, pointLightColors[2].y * 0.1, pointLightColors[2].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].diffuse"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[2].specular"), pointLightColors[2].x, pointLightColors[2].y, pointLightColors[2].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].linear"), 0.07);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[2].quadratic"), 0.017);
//            // Point light 4
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].position"), pointLightPositions[3].x, pointLightPositions[3].y, pointLightPositions[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].ambient"), pointLightColors[3].x * 0.1, pointLightColors[3].y * 0.1, pointLightColors[3].z * 0.1);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].diffuse"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "pointLights[3].specular"), pointLightColors[3].x, pointLightColors[3].y, pointLightColors[3].z);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].linear"), 0.07);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "pointLights[3].quadratic"), 0.017);
//            // SpotLight
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.position"), camera.Position.x, camera.Position.y, camera.Position.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.direction"), camera.Front.x, camera.Front.y, camera.Front.z);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.ambient"), 0.0f, 0.0f, 0.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.diffuse"), 0.0f, 1.0f, 0.0f);
//            glUniform3f(glGetUniformLocation(lightingShader.Program, "spotLight.specular"), 0.0f, 1.0f, 0.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.constant"), 1.0f);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.linear"), 0.07);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.quadratic"), 0.017);
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.cutOff"), glm::cos(glm::radians(7.0f)));
//            glUniform1f(glGetUniformLocation(lightingShader.Program, "spotLight.outerCutOff"), glm::cos(glm::radians(10.0f)));

            // transformações de visualização/projeção
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            Matrix4 view = camera.GetViewMatrix();
            lightingShader.setMat4("projection", projection);
            lightingShader.setMat4("view", view);

            // transformação mundial
            Matrix4 model = Matrix4.Identity;
            lightingShader.setMat4("model", model);

            // vincula mapa difuso
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, diffuseMap);
            // vincula mapa especular
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, specularMap);

            // renderiza contêineres
            GL.BindVertexArray(cubeVAO);
            for(int i = 0; i < 10; i++) {
                // calcula a matriz do modelo para cada objeto e passa para o shader antes de desenhar
                model = Matrix4.Identity;
                model *= Matrix4.CreateTranslation(cubePositions[i]);
                float angle = 20.0f * i;
                model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), MathHelper.DegreesToRadians(angle));
                lightingShader.setMat4("model", model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            // also draw the lamp object(s)
            lightCubeShader.use();
            lightCubeShader.setMat4("projection", projection);
            lightCubeShader.setMat4("view", view);

            // we now draw as many light bulbs as we have point lights.
            GL.BindVertexArray(lightCubeVAO);
            for(int i = 0; i < 4; i++) {
                model = Matrix4.Identity;
                model *= Matrix4.CreateTranslation(pointLightPositions[i]);
                model *= Matrix4.CreateScale(new Vector3(0.2f)); // Make it a smaller cube
                lightCubeShader.setMat4("model", model);
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

    // função utilitária para carregar uma textura 2D do arquivo
    // ---------------------------------------------------
    private static int loadTexture(string path) {
        int textureID;
        GL.GenTextures(1, out textureID);

        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        if(image.Data != null) {
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else {
            Console.WriteLine("Failed to load texture");
        }

        return textureID;
    }
}
