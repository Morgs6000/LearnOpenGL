using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Breakout;

// Representa uma única partícula e seu estado
public struct Particle {
    public Vector2 Position, Velocity;
    public Vector4 Color;
    public float Life;
}

// ParticleGenerator atua como um contêiner para renderizar um grande número de
// partículas gerando e atualizando repetidamente partículas e matando
// eles após um determinado período de tempo.
public class ParticleGenerator {
    // construtor
    public ParticleGenerator(Shader shader, Texture2D texture, int amount) {
        this.amount = amount;
        this.particles = new List<Particle>(amount); // Inicializa a lista de partículas
        this.shader = shader;
        this.texture = texture;

        this.init();
    }

    // atualiza todas as partículas
    public void Update(float dt, GameObject obj, int newParticles, Vector2 offset) {
        // adiciona novas partículas
        for(int i = 0; i < newParticles; i++) {
            int unusedParticle = this.firstUnusedParticle();
            this.respawnParticle(this.particles[unusedParticle], obj, offset);
        }
        // atualiza todas as partículas
        for(int i = 0; i < this.amount; i++) {
            Particle p = this.particles[i];
            p.Life -= dt; // reduz a vida
            if(p.Life > 0.0f) { // a partícula está viva, então atualize
                p.Position -= p.Velocity * dt;
                p.Color.W -= dt * 2.5f;
            }
        }
    }

    // renderiza todas as partículas
    public void Draw() {
        // usa mistura aditiva para dar um efeito de 'brilho'
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
        this.shader.Use();
        foreach(Particle particle in this.particles) {
            if(particle.Life > 0.0f) {
                this.shader.SetVector2f("offset", particle.Position);
                this.shader.SetVector4f("color", particle.Color);
                this.texture.Bind();
                GL.BindVertexArray(this.VAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }
        }
        // não se esqueça de redefinir o modo de mesclagem padrão
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    // estado
    private List<Particle> particles;
    int amount;
    // renderiza estado
    Shader shader;
    Texture2D texture;
    int VAO;

    // inicializa atributos de buffer e vértice
    private void init() {
        // configura propriedades de malha e atributos
        int VBO;
        float[] particle_quad = {
            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,

            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f
        };
        GL.GenVertexArrays(1, out this.VAO);
        GL.GenBuffers(1, out VBO);
        GL.BindVertexArray(this.VAO);
        // preenche o buffer da malha
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, particle_quad.Length * sizeof(float), particle_quad, BufferUsageHint.DynamicDraw);
        // define os atributos da malha
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.BindVertexArray(0);

        // cria isto->quantidade de instâncias de partículas padrão
        for(int i = 0; i < this.amount; i++) {
            this.particles.Add(new Particle());
        }
    }

    // armazena o índice da última partícula usada (para acesso rápido à próxima partícula morta)
    private int lastUsedParticle = 0;

    // retorna o primeiro índice de partículas que não está sendo utilizado no momento, por exemplo. Vida <= 0,0f ou 0 se nenhuma partícula estiver inativa no momento
    private int firstUnusedParticle() {
        // primeira pesquisa a partir da última partícula usada, isso geralmente retornará quase instantaneamente
        for(int i = lastUsedParticle; i < this.amount; i++) {
            if(this.particles[i].Life <= 0.0f) {
                lastUsedParticle = i;
                return i;
            }
        }
        // caso contrário, faça uma pesquisa linear
        for(int i = 0; i < lastUsedParticle; i++) {
            if(this.particles[i].Life <= 0.0f) {
                lastUsedParticle = i;
                return i;
            }
        }
        // todas as partículas são obtidas, sobrescreve a primeira (observe que se atingir repetidamente este caso, mais partículas deverão ser reservadas)
        lastUsedParticle = 0;
        return 0;
    }

    // faz respawn a partícula
    private void respawnParticle(Particle particle, GameObject obj, Vector2 offset) {
        Vector2 random = new Vector2(((new Random().Next(0, 100)) - 50) / 10.0f);
        float rColor = 0.5f + ((new Random().Next(0, 100)) / 100.0f);
        particle.Position = obj.Position + random + offset;
        particle.Color = new Vector4(rColor, rColor, rColor, 1.0f);
        particle.Life = 1.0f;
        particle.Velocity = obj.Velocity * 0.1f;
    }
}
