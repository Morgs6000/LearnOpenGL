using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Breakout;

// Uma classe ResourceManager singleton estática que hospeda vários
// funções para carregar Texturas e Shaders. Cada textura carregada
// e/ou shader também são armazenados para referência futura por string
// manipula. Todas as funções e recursos são estáticos e não
// construtor público é definido.
public class ResourceManager {
    // armazenamento de recursos
    public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
    public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

    // carrega (e gera) um programa de shader a partir do arquivo que carrega o código-fonte do shader de vértice, fragmento (e geometria). Se gShaderFile não for nullptr, ele também carrega um shader de geometria
    public static Shader LoadShader(string vShaderFile, string fShaderFile, string gShaderFIle, string name) {
        Shaders[name] = loadShaderFromFile(vShaderFile, fShaderFile, gShaderFIle);
        return Shaders[name];
    }

    // recupera um sader armazenado
    public static Shader GetShader(string name) {
        return Shaders[name];
    }

    // carrega (e gera) uma textura do arquivo
    public static Texture2D LoadTexture(string file, bool alpha, string name) {
        Textures[name] = loadTextureFromFile(file, alpha);
        return Textures[name];
    }

    // recupera uma textura armazenada
    public static Texture2D GetTexture(string name) {
        return Textures[name];
    }

    // desaloca corretamente todos os recursos carregados
    public static void Clear() {
        // (corretamente) exclui todos os shaders
        foreach(var inter in Shaders.Values) {
            GL.DeleteProgram(inter.ID);
        }
        // (corretamente) exclua todas as texturas
        foreach(var inter in Textures.Values) {
            GL.DeleteTextures(1, ref inter.ID);
        }
    }

    // construtor privado, ou seja, não queremos nenhum objeto real do gerenciador de recursos. Seus membros e funções devem estar disponíveis publicamente (estático).
    private ResourceManager() {

    }

    // carrega e gera um shader do arquivo
    private static Shader loadShaderFromFile(string vShaderFile, string fShaderFile, string gShaderFile = null) {
        // 1. recupera o código-fonte do vértice/fragmento de filePath
        // Abrir arquivos
        string vShaderCode = File.ReadAllText(vShaderFile);
        string fShaderCode = File.ReadAllText(fShaderFile);
        string gShaderCode = File.ReadAllText(gShaderFile);
        // 2. agora crie um objeto shader a partir do código-fonte
        Shader shader = new Shader();
        shader.Compile(vShaderCode, fShaderCode, gShaderFile != null ? gShaderCode : null);
        return shader;
    }

    // carrega uma única textura do arquivo
    private static Texture2D loadTextureFromFile(string file, bool alpha) {
        // cria objeto de textura
        Texture2D texture = new Texture2D();
        if(alpha) {
            texture.Internal_Format = PixelInternalFormat.Rgba;
            texture.Image_Format = PixelFormat.Rgba;
        }
        // carrega a imagem
        ImageResult image = ImageResult.FromStream(File.OpenRead(file), ColorComponents.RedGreenBlueAlpha);
        // agora gera textura
        texture.Generate(image.Width, image.Height, image.Data);
        // e finalmente dados de imagem gratuitos
        return texture;
    }
}
