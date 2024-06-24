using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.Serialization;

namespace Breakout;

// Representa o estado atual do jogo
public enum GameState {
    GAME_ACTIVE,
    GAME_MENU,
    GAME_WIN
}

// Representa as quatro direções possíveis (colisão)
public enum Direction {
    UP,
    RIGHT,
    DOWN,
    LEFT
}

// O jogo contém todos os estados e funcionalidades relacionados ao jogo.
// Combina todos os dados relacionados ao jogo em uma única classe para
// fácil acesso a cada um dos componentes e capacidade de gerenciamento.
public class Game {
    // Define um typedef Collision que representa dados de colisão
    private Tuple<bool, Direction, Vector2> Collision; // <colisão?, em que direção?, centro do vetor diferença - ponto mais próximo>

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
    private ParticleGenerator Particles;

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
        ResourceManager.LoadShader("../../../shaders/particle_vs.glsl", "../../../shaders/particle_fs.glsl", null, "particle");
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
        ResourceManager.LoadTexture("../../../textures/particle.png", true, "particle");

        // ..:: ::..
        Particles = new ParticleGenerator(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 500);

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
        // atualiza partículas
        Particles.Update(dt, Ball, 2, new Vector2(Ball.Radius / 2.0f));

        if(Ball.Position.Y >= this.Height) { // a bola atingiu a borda inferior?
            this.ResetLevel();
            this.ResetPlayer();
        }
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
            // desenha partículas
            Particles.Draw();
            // desenha a bola
            Ball.Draw(Renderer);
        }
    }

    public void ResetLevel() {
        if(this.Level == 0) {
            this.Levels[0].Load("../../../levels/one.lvl", this.Widht, this.Height / 2);
        }
        else if(this.Level == 1) {
            this.Levels[1].Load("../../../levels/two.lvl", this.Widht, this.Height / 2);
        }
        else if(this.Level == 2) {
            this.Levels[2].Load("../../../levels/three.lvl", this.Widht, this.Height / 2);
        }
        else if(this.Level == 3) {
            this.Levels[3].Load("../../../levels/four.lvl", this.Widht, this.Height / 2);
        }
    }

    public void ResetPlayer() {
        // redefinir estatísticas de jogador/bola
        Player.Size = PLAYER_SIZE;
        Player.Position = new Vector2(this.Widht / 2.0f - PLAYER_SIZE.X / 2.0f, this.Height - PLAYER_SIZE.Y);
        Ball.Reset(Player.Position + new Vector2(PLAYER_SIZE.X / 2.0f - BALL_RADIUS, -(BALL_RADIUS * 2.0f)), INITIAL_BALL_VELOCITY);
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
    //*/

    /*
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
    //*/

    //*
    public Tuple<bool, Direction, Vector2> CheckCollision(BallObject one, GameObject two) { // AABB - Colisão circular
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

        if(difference.Length < one.Radius) { // não <= já que nesse caso uma colisão também ocorre quando o objeto um toca exatamente o objeto dois, o que ocorre no final de cada estágio de resolução de colisão.
            return Tuple.Create(true, VectorDirection(difference), difference);
        }
        else {
            return Tuple.Create(false, Direction.UP, new Vector2(0.0f, 0.0f));
        }
    }
    //*/

    public void DoCollisions() {
        foreach(GameObject box in this.Levels[this.Level].Bricks) {
            if(!box.Destroyed) {
                var collision = CheckCollision(Ball, box);
                if(collision.Item1) { // se a colisão for verdadeira
                    // destrui o bloco se não for sólido
                    if(!box.IsSolid) {
                        box.Destroyed = true;
                    }
                    // resolução de colisão
                    Direction dir = collision.Item2;
                    Vector2 diff_vector = collision.Item3;
                    if(dir == Direction.LEFT || dir == Direction.RIGHT) { // colisão horizontal
                        Ball.Velocity.X = -Ball.Velocity.X; // reverte a velocidade horizontal
                        // realocar
                        float penetrarion = Ball.Radius - Math.Abs(diff_vector.X);
                        if(dir == Direction.LEFT) {
                            Ball.Position.X += penetrarion; // move a bola para a direita
                        }
                        else {
                            Ball.Position.X -= penetrarion; // move a bola para a esquerda;
                        }
                    }
                    else { // colisão vertical
                        Ball.Velocity.Y = -Ball.Velocity.Y; //reverte a velocidade vertical
                        // realocar
                        float penetration = Ball.Radius - Math.Abs(diff_vector.Y);
                        if(dir == Direction.UP) {
                            Ball.Position.Y -= penetration; // move a bola de volta para cima
                        }
                        else {
                            Ball.Position.Y += penetration; // move a bola de volta para baixo
                        }
                    }
                }
            }
            
            var result = CheckCollision(Ball, Player);
            if(!Ball.Stuck && result.Item1) {
                // verifica onde ele atingiu a prancha e altera a velocidade com base em onde ela atingiu a prancha
                float centerBoard = Player.Position.X + Player.Size.X / 2.0f;
                float distance = (Ball.Position.X + Ball.Radius) - centerBoard;
                float percentage = distance / (Player.Size.X / 2.0f);
                // então mova de acordo
                float strength = 2.0f;
                Vector2 oldVelocity = Ball.Velocity;
                Ball.Velocity.X = INITIAL_BALL_VELOCITY.X * percentage * strength;
                //Ball.Velocity.Y = -Ball.Velocity.Y;
                Ball.Velocity.Y = -1.0f * Math.Abs(Ball.Velocity.Y);
                Ball.Velocity = Vector2.Normalize(Ball.Velocity) * oldVelocity.Length;
            }
        }
    }

    public Direction VectorDirection(Vector2 target) {
        Vector2[] compass = {
            new Vector2(0.0f, 1.0f),	// up
            new Vector2(1.0f, 0.0f),	// right
            new Vector2(0.0f, -1.0f),	// down
            new Vector2(-1.0f, 0.0f)	// left
        };
        float max = 0.0f;
        int best_match = -1;
        for(int i = 0; i < 4; i++) {
            float dot_product = Vector2.Dot(Vector2.Normalize(target), compass[i]);
            if(dot_product > max ) {
                max = dot_product;
                best_match = i;
            }
        }
        return (Direction)best_match;
    }
}
