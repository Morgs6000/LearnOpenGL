using OpenTK.Mathematics;

namespace Breakout;

// Objeto contêiner para manter todos os estados relevantes para um único
// entidade do objeto do jogo. Cada objeto no jogo provavelmente precisa do
// mínimo de estado conforme descrito em GameObject.
public class GameObject {
    // estado do objeto
    public Vector2 Position, Size, Velocity;
    public Vector3 Color;
    public float Rotation;
    public bool IsSolid;
    public bool Destroyed;
    // renderiza estado
    public Texture2D Sprite;

    // construtor(es)
    public GameObject() {
        this.Position = new Vector2(0.0f, 0.0f);
        this.Size = new Vector2(1.0f, 1.0f);
        this.Velocity = new Vector2(0.0f);
        this.Color = new Vector3(1.0f);
        this.Rotation = 0.0f;
        //this.Sprite
        this.IsSolid = false;
        this.Destroyed = false;
    }

    public GameObject(Vector2 pos, Vector2 size, Texture2D sprite, Vector3 color, Vector2 velocity) {
        this.Position = pos;
        this.Size = size;
        this.Velocity = velocity;
        this.Color = color;
        this.Rotation = 0.0f;
        this.Sprite = sprite;
        this.IsSolid = false;
        this.Destroyed = false;
    }

    // desenha sprite
    public virtual void Draw(SpriteRenderer renderer) {
        renderer.DrawSprite(this.Sprite, this.Position, this.Size, this.Rotation, this.Color);
    }
}
