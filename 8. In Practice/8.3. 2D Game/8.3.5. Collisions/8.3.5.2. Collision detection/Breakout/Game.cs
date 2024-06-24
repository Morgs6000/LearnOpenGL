using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.Serialization;

namespace Breakout;

// Representa o estado atual do jogo
public enum GameState {
    GAME_ACTIVE,
    GAME_MENU,
    GAME_WIN
};

// O jogo contém todos os estados e funcionalidades relacionados ao jogo.
// Combina todos os dados relacionados ao jogo em uma única classe para
// fácil acesso a cada um dos componentes e capacidade de gerenciamento.
public class Game {
    // Tamanho inicial do paddle do jogador
    private Vector2 PLAYER_SIZE = new Vector2(100.0f, 20.0f);
    // Velocidade inicial da raquete do jogador
    private float PLAYER_VELOCITY = 500.0f;

    // Velocidade inicial da bola
    private Vector2 INITIAL_BALL_VELOCITY = new Vector2(100.0f, -350.0f);
    // Raio do objeto bola
    private float BALL_RADIUS = 12.5f;

    // Dados de estado relacionados ao jogo
    private SpriteRenderer Renderer;
    private GameObject Player;
    private BallObject Ball;

    // estado do jogo
    public GameState State;
    //public bool Keys;
    public int Widht, Height;
    List<GameLevel> Levels = new List<GameLevel>();
    int Level;

    // construtor/destruidor
    public Game(int width, int height) {
        this.State = GameState.GAME_ACTIVE;
        //this.Keys;
        this.Widht = width;
        this.Height = height;
    }

    public Game() {
        //delete Renderer;
        //delete Player;
    }

    // inicializa o estado do jogo (carrega todos os shaders/texturas/níveis)
    public void Init() {
        // carrega shaders
        ResourceManager.LoadShader("../../../shaders/sprite_vs.glsl", "../../../shaders/sprite_fs.glsl", null, "sprite");
        // configura shaders
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, (float)(this.Widht), (float)(this.Height), 0.0f, -1.0f, 1.0f);
        ResourceManager.GetShader("sprite").Use().SetInteger("image", 0);
        ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);
        // define controles específicos de renderização
        Renderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
        // carrega texturas
        ResourceManager.LoadTexture("../../../textures/background.jpg", false, "background");
        ResourceManager.LoadTexture("../../../textures/awesomeface.png", false, "face");
        ResourceManager.LoadTexture("../../../textures/block.png", false, "block");
        ResourceManager.LoadTexture("../../../textures/block_solid.png", false, "block_solid");
        ResourceManager.LoadTexture("../../../textures/paddle.png", true, "paddle");
        //níveis de carga
        GameLevel one = new GameLevel();
        one.Load("../../../levels/one.lvl", this.Widht, this.Height / 2);
        GameLevel two = new GameLevel();
        two.Load("../../../levels/two.lvl", this.Widht, this.Height / 2);
        GameLevel three = new GameLevel();
        three.Load("../../../levels/three.lvl", this.Widht, this.Height / 2);
        GameLevel four = new GameLevel();
        four.Load("../../../levels/four.lvl", this.Widht, this.Height / 2);
        this.Levels.Add(one);
        this.Levels.Add(two);
        this.Levels.Add(three);
        this.Levels.Add(four);
        this.Level = 0;
        //configura objetos do jogo
        Vector2 playerPos = new Vector2(this.Widht / 2.0f - PLAYER_SIZE.X / 2.0f, this.Height - PLAYER_SIZE.Y);
        Player = new GameObject(playerPos, PLAYER_SIZE, ResourceManager.GetTexture("paddle"), new Vector3(1.0f), new Vector2(0.0f));

        Vector2 ballPos = playerPos + new Vector2(PLAYER_SIZE.X / 2.0f - BALL_RADIUS, -BALL_RADIUS * 2.0f);
        Ball = new BallObject(ballPos, BALL_RADIUS, INITIAL_BALL_VELOCITY, ResourceManager.GetTexture("face"));
    }

    public void Update(float dt) {
        // atualiza objetos
        Ball.Move(dt, this.Widht);
        // verifica colisões
        this.DoCollisions();
    }

    // loop do jogo
    public void ProcessInput(float dt, KeyboardState key) {
        if(this.State == GameState.GAME_ACTIVE) {
            float velocity = PLAYER_VELOCITY * dt;
            // move o tabuleiro do jogador
            if(key.IsKeyDown(Keys.A)) {
                if(Player.Position.X >= 0.0f) {
                    Player.Position.X -= velocity;
                    if(Ball.Stuck) {
                        Ball.Position.X -= velocity;
                    }
                }
            }
            if(key.IsKeyDown(Keys.D)) {
                if(Player.Position.X <= this.Widht - Player.Size.X) {
                    Player.Position.X += velocity;
                    if(Ball.Stuck) {
                        Ball.Position.X += velocity;
                    }
                }
            }
            if(key.IsKeyDown(Keys.Space)) {
                Ball.Stuck = false;
            }
        }
    }

    public void Render() {
        if(this.State == GameState.GAME_ACTIVE) {
            //desenha o fundo
            Renderer.DrawSprite(ResourceManager.GetTexture("background"), new Vector2(0.0f, 0.0f), new Vector2(this.Widht, this.Height), 0.0f, new Vector3(1.0f));
            // desenha o nível
            this.Levels[this.Level].Draw(Renderer);
            //desenha o jogador
            Player.Draw(Renderer);

            Ball.Draw(Renderer);
        }
    }

    /*
    public bool CheckCollision(GameObject one, GameObject two) { // AABB - colisão AABB
        // colisão eixo x?
        bool collisionX = one.Position.X + one.Size.X >= two.Position.X && two.Position.X + two.Size.X >= one.Position.X;
        // colisão eixo y?
        bool collisionY = one.Position.Y + one.Size.Y >= two.Position.Y && two.Position.Y + two.Size.Y >= one.Position.Y;
        // colisão somente se estiver em ambos os eixos
        return collisionX && collisionY;
    }
    */

    public bool CheckCollision(BallObject one, GameObject two) { // AABB - Colisão circular
        // obtém o círculo do ponto central primeiro
        Vector2 center = one.Position + new Vector2(one.Radius);
        // calcula informações AABB (centro, meias extensões)
        Vector2 aabb_half_extents = new Vector2(two.Size.X / 2.0f, two.Size.Y / 2.0f);
        Vector2 aabb_center = new Vector2(two.Position.X + aabb_half_extents.X, two.Position.Y + aabb_half_extents.Y);
        // obtém o vetor diferença entre os dois centros
        Vector2 difference = center - aabb_center;
        Vector2 clamped = Vector2.Clamp(difference, -aabb_half_extents, aabb_half_extents);
        // adicionamos o valor fixado a AABB_center e obtemos o valor da caixa mais próxima do círculo
        Vector2 closest = aabb_center + clamped;
        // recupera o vetor entre o círculo central e o ponto mais próximo AABB e verifica se comprimento <= raio
        difference = closest - center;
        return difference.Length < one.Radius;
    }

    public void DoCollisions() {
        foreach(GameObject box in this.Levels[this.Level].Bricks) {
            if(!box.Destroyed) {
                if(CheckCollision(Ball, box)) {
                    if(!box.IsSolid) {
                        box.Destroyed = true;
                    }
                }
            }
        }
    }
}
