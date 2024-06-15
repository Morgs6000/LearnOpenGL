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
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, (float)(this.Widht), (float)(this.Height), 0.0f, -1.0f, 1.0f);
        ResourceManager.GetShader("sprite").Use().SetInteger("image", 0);
        ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);
        // define controles específicos de renderização
        Renderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
        // carrega texturas
        ResourceManager.LoadTexture("../../../textures/awesomeface.png", true, "face");
    }

    // loop do jogo
    public void ProcessInput(float dt) {

    }

    public void Update(float dt) {

    }

    public void Render() {
        Renderer.DrawSprite(ResourceManager.GetTexture("face"), new Vector2(200.0f, 200.0f), new Vector2(300.0f, 400.0f), 45.0f, new Vector3(0.0f, 1.0f, 0.0f));
    }
}
