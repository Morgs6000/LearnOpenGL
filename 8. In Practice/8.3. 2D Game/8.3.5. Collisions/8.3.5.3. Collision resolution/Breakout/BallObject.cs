using OpenTK.Mathematics;

namespace Breakout;

// BallObject mantém o estado do objeto Ball herdado
// dados de estado relevantes do GameObject. Contém alguns extras
// funcionalidade específica para o objeto bola do Breakout que
// eram muito específicos apenas para GameObject.
public class BallObject : GameObject {

    // estado da bola
    public float Radius;
    public bool Stuck;

    // construtor(es)
    public BallObject() 
        : base() {
        this.Radius = 12.5f;
        this.Stuck = true;
    }

    public BallObject(Vector2 pos, float radius, Vector2 velocity, Texture2D sprite) 
        : base(pos, new Vector2(radius * 2.0f, radius * 2.0f), sprite, new Vector3(1.0f), velocity) { 
        this.Radius = radius;
        this.Stuck = true;
    }

    // move a bola, mantendo-a restrita dentro dos limites da janela (exceto na borda inferior); retorna nova posição
    public Vector2 Move(float dt, int window_widht) {
        // se não estiver preso ao tabuleiro do jogador
        if(!this.Stuck) {
            // mover a bola
            this.Position += this.Velocity * dt;
            // então verifica se está fora dos limites da janela e se estiver, inverte a velocidade e restaura na posição correta
            if(this.Position.X <= 0.0f) {
                this.Velocity.X = -this.Velocity.X;
                this.Position.X = 0.0f;
            }
            else if(this.Position.X + this.Size.X >= window_widht) {
                this.Velocity.X = -this.Velocity.X;
                this.Position.X = window_widht - this.Size.X;
            }
            if(this.Position.Y <= 0.0f) {
                this.Velocity.Y = -this.Velocity.Y;
                this.Position.Y = 0.0f;
            }
        }
        return this.Position;
    }

    // reinicia a bola ao estado original com determinada posição e velocidade
    // redefine a bola para a posição inicial presa (se a bola estiver fora dos limites da janela)
    public void Reset(Vector2 position, Vector2 velocity) {
        this.Position = position;
        this.Velocity = velocity;
        this.Stuck = true;
    }
}
