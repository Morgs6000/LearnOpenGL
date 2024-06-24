using OpenTK.Graphics.OpenGL4;

namespace Breakout;

// Texture2D é capaz de armazenar e configurar uma textura em OpenGL.
// Ele também hospeda funções utilitárias para fácil gerenciamento.
public class Texture2D {
    // mantém o ID do objeto de textura, usado para todas as operações de textura para fazer referência a esta textura específica
    public int ID;
    //dimensões da imagem da textura
    public int Width, Height; // largura e altura da imagem carregada em pixels
    // formato da textura
    public PixelInternalFormat Internal_Format; // formato do objeto de textura
    public PixelFormat Image_Format; // formato da imagem carregada
    // configuração de textura
    public int Wrap_S; // modo de encapsulamento no eixo S
    public int Wrap_T; // modo de encapsulamento no eixo T
    public int Filter_Min; // modo de filtragem se pixels de textura < pixels da tela
    public int Filter_Max; // modo de filtragem se pixels de textura > pixels de tela

    // construtor (define os modos de textura padrão)
    public Texture2D() {
        this.Width = 0;
        this.Height = 0;
        this.Internal_Format = PixelInternalFormat.Rgba;
        this.Image_Format = PixelFormat.Rgba;
        this.Wrap_S = (int)TextureWrapMode.Repeat;
        this.Wrap_T = (int)TextureWrapMode.Repeat;
        this.Filter_Min = (int)TextureMinFilter.Linear;
        this.Filter_Max = (int)TextureMagFilter.Linear;

        GL.GenTextures(1, out this.ID);
    }

    // gera textura a partir de dados de imagem
    public void Generate(int width, int height, byte[] data) {
        this.Width = width;
        this.Height = height;
        // cria textura
        GL.BindTexture(TextureTarget.Texture2D, this.ID);
        GL.TexImage2D(TextureTarget.Texture2D, 0, this.Internal_Format, width, height, 0, this.Image_Format, PixelType.UnsignedByte, data);
        // define os modos de ajuste de textura e filtro
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, this.Wrap_S);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, this.Wrap_T);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, this.Filter_Min);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, this.Filter_Max);
        // desvincula a textura
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    // vincula a textura como o objeto de textura GL_TEXTURE_2D ativo atual
    public void Bind() {
        GL.BindTexture(TextureTarget.Texture2D, this.ID);
    }
}
