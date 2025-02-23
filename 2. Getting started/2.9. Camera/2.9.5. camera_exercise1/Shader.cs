﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LearnOpenGL;

public class Shader {
    // o ID do programa
    public int ID;

    // construtor gera o shader dinamicamente
    // ------------------------------------------------------------------------
    public Shader(string vertexPath, string fragmentPath) {
        // 1. recupera o código-fonte do vértice/fragmento de filePath
        string vShaderCode = File.ReadAllText(vertexPath);
        string fShaderCode = File.ReadAllText(fragmentPath);
        // 2. compilar shaders
        int vertex, fragment;
        // shader de vértice
        vertex = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertex, vShaderCode);
        GL.CompileShader(vertex);
        checkCompileErrors(vertex, "VERTEX");
        // shader de fragmento
        fragment = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragment, fShaderCode);
        GL.CompileShader(fragment);
        checkCompileErrors(fragment, "FRAGMENT");

        // programa de sombreamento
        ID = GL.CreateProgram();
        GL.AttachShader(ID, vertex);
        GL.AttachShader(ID, fragment);

        GL.LinkProgram(ID);
        checkCompileErrors(ID, "PROGRAM");
        // exclui os shaders, pois eles estão vinculados ao nosso programa agora e não são mais necessários
        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    // ativar o sombreador
    // ------------------------------------------------------------------------
    public void use() {
        GL.UseProgram(ID);
    }

    // funções uniformes de utilidade
    // ------------------------------------------------------------------------
    public void setBool(string name, bool value) {
        GL.Uniform1(GL.GetUniformLocation(ID, name), value ? 1 : 0);
    }
    // ------------------------------------------------------------------------
    public void setInt(string name, int value) {
        GL.Uniform1(GL.GetUniformLocation(ID, name), value);
    }
    // ------------------------------------------------------------------------
    public void setFloat(string name, float value) {
        GL.Uniform1(GL.GetUniformLocation(ID, name), value);
    }
    // ------------------------------------------------------------------------
    public void setMat4(string name, Matrix4 value) {
        GL.UniformMatrix4(GL.GetUniformLocation(ID, name), false, ref value);
    }

    // função utilitária para verificar erros de compilação/vinculação de shader.
    // ------------------------------------------------------------------------
    private void checkCompileErrors(int shader, string type) {
        int success;
        string infoLog;
        if(type != "PROGRAM") {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out success);
            if(success == 0) {
                GL.GetShaderInfoLog(shader, out infoLog);
                Console.WriteLine("ERROR::SHADER_COMPILATION_ERROR of type: " + type + "\n" + infoLog + "\n -- --------------------------------------------------- -- ");
            }
        }
        else {
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out success);
            if(success == 0) {
                GL.GetProgramInfoLog(ID, out infoLog);
                Console.WriteLine("ERROR::PROGRAM_LINKING_ERROR of type: " + type + "\n" + infoLog + "\n -- --------------------------------------------------- -- ");
            }
        }
    }
}
