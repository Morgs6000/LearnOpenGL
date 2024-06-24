using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Breakout;

// Objeto shader de uso geral. Compila do arquivo, gera
// compila/link-time mensagens de erro e hospeda vários utilitários
//funções para fácil gerenciamento.
public class Shader {
    // estado
    public int ID;

    // construtor
    public Shader() {

    }

    // define o shader atual como ativo
    public Shader Use() {
        GL.UseProgram(this.ID);
        return this;
    }

    // compila o shader a partir do código-fonte fornecido
    public void Compile(string vertexSource, string fragmentSource, string geometrySource = null) { // nota: o código fonte da geometria é opcional
        int sVertex, sFragment, gShader;
        // sombreador de vértice
        sVertex = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(sVertex, vertexSource);
        GL.CompileShader(sVertex);
        checkCompileErrors(sVertex, "VERTEX");
        // fragmentar Shader
        sFragment = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(sFragment, fragmentSource);
        GL.CompileShader(sFragment);
        checkCompileErrors(sFragment, "FRAGMENT");

        // programa de sombreamento
        this.ID = GL.CreateProgram();
        GL.AttachShader(this.ID, sVertex);
        GL.AttachShader(this.ID, sFragment);

        // se o código fonte do shader de geometria for fornecido, também compila o shader de geometria
        if(geometrySource != null) {
            gShader = GL.CreateShader(ShaderType.GeometryShader);
            GL.ShaderSource(gShader, geometrySource);
            GL.CompileShader(gShader);
            checkCompileErrors(gShader, "GEOMETRY");

            GL.AttachShader(this.ID, gShader);

            GL.DeleteShader(gShader);
        }

        GL.LinkProgram(this.ID);
        checkCompileErrors(this.ID, "PROGRAM");
        // exclui os shaders, pois eles estão vinculados ao nosso programa agora e não são mais necessários
        GL.DeleteShader(sVertex);
        GL.DeleteShader(sFragment);
    }

    // funções utilitárias
    public void SetFloat(string name, float value, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform1(GL.GetUniformLocation(this.ID, name), value);
    }
    public void SetInteger(string name, int value, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform1(GL.GetUniformLocation(this.ID, name), value);
    }
    public void SetVector2f(string name, float x, float y, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform2(GL.GetUniformLocation(this.ID, name), x, y);
    }
    public void SetVector2f(string name, Vector2 value, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform2(GL.GetUniformLocation(this.ID, name), value);
    }
    public void SetVector3f(string name, float x, float y, float z, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform3(GL.GetUniformLocation(this.ID, name), x, y, z);
    }
    public void SetVector3f(string name, Vector3 value, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform3(GL.GetUniformLocation(this.ID, name), value);
    }
    public void SetVector4f(string name, float x, float y, float z, float w, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform4(GL.GetUniformLocation(this.ID, name), x, y, z, w);
    }
    public void SetVector4f(string name, Vector4 value, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.Uniform4(GL.GetUniformLocation(this.ID, name), value);
    }
    public void SetMatrix4(string name, Matrix4 matrix, bool useShader = false) {
        if(useShader) {
            this.Use();
        }
        GL.UniformMatrix4(GL.GetUniformLocation(this.ID, name), false, ref matrix);
    }

    // verifica se a compilação ou vinculação falhou e, em caso afirmativo, imprime os logs de erros
    private void checkCompileErrors(int obj, string type) {
        int success;
        string inforLog;
        if(type != "PROGRAM") {
            GL.GetShader(obj, ShaderParameter.CompileStatus, out success);
            if(success == 0) {
                GL.GetShaderInfoLog(obj, out inforLog);
                Console.WriteLine("| ERROR::SHADER: Compile-time error: Type: " + type + "\n" + inforLog + "\n -- --------------------------------------------------- -- ");
            }
        }
        else {
            GL.GetProgram(obj, GetProgramParameterName.LinkStatus, out success);
            if(success == 0) {
                GL.GetProgramInfoLog(obj, out inforLog);
                Console.WriteLine("| ERROR::Shader: Link-time error: Type: " + type + "\n" + inforLog + "\n -- --------------------------------------------------- -- ");
            }
        }
    }
}
