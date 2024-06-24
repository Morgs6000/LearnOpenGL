using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Breakout;

public class SpriteRenderer {
    // Construtor (inicia shaders/formas)
    public SpriteRenderer(Shader shader) {
        this.shader = shader;
        this.initRenderData();
    }

    // Destruidor
    public SpriteRenderer() {
        GL.DeleteVertexArrays(1, ref this.quadVAO);
    }

    // Renderiza um quad definido texturizado com determinado sprite
    public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, float rotate, Vector3 color) {
        //prepara transformações
        this.shader.Use();
        Matrix4 model = Matrix4.Identity;

        /*
        model *= Matrix4.CreateScale(new Vector3(size.X, size.Y, 1.0f)); // última escala

        model *= Matrix4.CreateTranslation(new Vector3(-0.5f * size.X, -0.5f * size.Y, 0.0f)); // mover origem de volta
        model *= Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), MathHelper.DegreesToRadians(rotate)); // depois gira
        model *= Matrix4.CreateTranslation(new Vector3(0.5f * size.X, 0.5f * size.Y, 0.0f)); // move a origem da rotação para o centro do quadrante

        model *= Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f)); // primeira tradução (as transformações são: a escala acontece primeiro, depois a rotação e depois a tradução final acontece; ordem inversa)
        */
        model *= Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);

        this.shader.SetMatrix4("model", model);

        // renderiza quad texturizado
        this.shader.SetVector3f("spriteColor", color);

        GL.ActiveTexture(TextureUnit.Texture0);
        texture.Bind();

        GL.BindVertexArray(this.quadVAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
    }

    // Estado de renderização
    private Shader shader;
    private int quadVAO;

    // Inicializa e configura os atributos de buffer e vértice do quad
    private void initRenderData() {
        // configura VAO/VBO
        int VBO;
        float[] vertices = {
            // pos      // tex
            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,

            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f
        };

        GL.GenVertexArrays(1, out this.quadVAO);
        GL.GenBuffers(1, out VBO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        GL.BindVertexArray(this.quadVAO);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
}
