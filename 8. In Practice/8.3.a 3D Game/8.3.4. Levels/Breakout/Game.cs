using OpenTK.Mathematics;

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
    // estado do jogo
    public GameState State;
    public bool Keys;
    int Widht, Height;

    List<GameLevel> Levels = new List<GameLevel>();
    int Level;

    // Dados de estado relacionados ao jogo
    SpriteRenderer Renderer;

    // construtor/destruidor
    public Game(int width, int height) {
        this.State = GameState.GAME_ACTIVE;
        //this.Keys;
        this.Widht = width;
        this.Height = height;
    }

    public Game() {

    }

    // inicializa o estado do jogo (carrega todos os shaders/texturas/níveis)
    public void Init() {
        // carrega shaders
        ResourceManager.LoadShader("../../../shaders/sprite_vs.glsl", "../../../shaders/sprite_fs.glsl", null, "sprite");
        // configura shaders
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)(this.Widht) / (float)(this.Height), 0.1f, 100.0f);
        ResourceManager.GetShader("sprite").Use().SetInteger("image", 0);
        ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);
        // define controles específicos de renderização
        Renderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
        // carrega texturas
        ResourceManager.LoadTexture("../../../textures/background.jpg", false, "background");
        ResourceManager.LoadTexture("../../../textures/awesomeface.png", false, "face");
        ResourceManager.LoadTexture("../../../textures/block.png", false, "block");
        ResourceManager.LoadTexture("../../../textures/block_solid.png", false, "block_solid");
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
    }

    // loop do jogo
    public void ProcessInput(float dt) {

    }

    public void Update(float dt) {

    }

    public void Render() {
        if(this.State == GameState.GAME_ACTIVE) {
            //desenha o fundo
            Renderer.DrawSprite(ResourceManager.GetTexture("background"), new Vector2(0.0f, 0.0f), new Vector2(this.Widht, this.Height), 0.0f, new Vector3(1.0f));
            // desenha o nível
            this.Levels[this.Level].Draw(Renderer);
        }
    }
}
