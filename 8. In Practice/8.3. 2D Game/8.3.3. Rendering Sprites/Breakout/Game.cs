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

    }

    //loop do jogo
    public void ProcessInput(float dt) {

    }

    public void Update(float dt) {

    }

    public void Render() {

    }
}
