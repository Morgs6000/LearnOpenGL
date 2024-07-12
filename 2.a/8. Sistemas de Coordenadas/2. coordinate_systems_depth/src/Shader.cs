using OpenTK.Graphics.OpenGL4;

namespace LearnOpenGL.src;

public class Shader {
    // o ID do programa
    public int shaderProgram;

    // construtor lê e constrói o shader
    public Shader(string vertexPath, string fragmentPath) {
        string vertexShaderSource = File.ReadAllText("../../../src/shaders/" + vertexPath);
        string fragmentShaderSource = File.ReadAllText("../../../src/shaders/" + fragmentPath);

        int success;
        string infoLog;

        // sombreador de vértice
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        // verifique se há erros de compilação do shader        
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(vertexShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" + infoLog);
        }

        // sombreador de fragmento
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        // verifique se há erros de compilação do shader
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(fragmentShader, out infoLog);
            Console.WriteLine("ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" + infoLog);
        }

        // shaders de link
        shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        // verifique se há erros de compilação do shader
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);
        if(success == 0) {
            GL.GetShaderInfoLog(shaderProgram, out infoLog);
            Console.WriteLine("ERROR::SHADER::PROGRAM::LINKING_FAILED\n" + infoLog);
        }

        // exclui os shaders, pois eles estão vinculados ao nosso programa agora e não são mais necessários
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    // usa/ativa o shader
    public void use() {
        GL.UseProgram(shaderProgram);
    }

    // funções uniformes utilitárias
    public void setBool(string name, bool value) {
        GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), value ? 1 : 0);
    }

    public void setInt(string name, int value) {
        GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), value);
    }

    public void setFloat(string name, float value) {
        GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), value);
    }
}
