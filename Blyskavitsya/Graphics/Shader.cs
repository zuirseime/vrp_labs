using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Blyskavitsya.Graphics;
public sealed class Shader : Resource
{
    public Shader(string vertexPath, string fragmentPath)
    {
        Handle = GL.CreateProgram();

        int vertexShader = CreateShader(ShaderType.VertexShader, vertexPath);
        int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentPath);

        LinkProgram();

        DisposeShader(vertexShader);
        DisposeShader(fragmentShader);
    }

    private int CreateShader(ShaderType type, string path)
    {
        int shader = GL.CreateShader(type);

        GL.ShaderSource(shader, LoadSource(path));
        CompileShader(shader, type);
        GL.AttachShader(Handle, shader);

        return shader;
    }

    private void LinkProgram()
    {
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == (int)All.False)
            throw new Exception($"Shader Program Linking Error: {GL.GetProgramInfoLog(Handle)}");
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.Uniform1(location, value);

        CheckGLError($"SetInt: {name}:{value}");
    }

    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.Uniform1(location, value);

        CheckGLError($"SetFloat: {name}:{value}");
    }

    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.Uniform3(location, value);

        CheckGLError($"SetVector3: {name}:{value}");
    }

    public void SetVector4(string name, Vector4 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.Uniform4(location, value);

        CheckGLError($"SetVector4: {name}:{value}");
    }

    public void SetColor4(string name, Color4 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.Uniform4(location, value);

        CheckGLError($"SetColor4: {name}:{value}");
    }

    public void SetMatrix4(string name, Matrix4 value)
    {
        var location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            //Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
            return;
        }
        GL.UniformMatrix4(location, true, ref value);

        CheckGLError($"SetMatrix4: {name}:{value}");
    }

    private static void CompileShader(int shader, ShaderType type)
    {
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == (int)All.False)
            throw new Exception($"{type} Compilation Error: {GL.GetShaderInfoLog(shader)}");

        CheckGLError($"CompileShader: {shader}:{type}");
    }

    private static string LoadSource(string path)
    {
        try
        {
            return File.ReadAllText(path);
        } catch (Exception ex)
        {
            throw new Exception($"Failed to load shader source file: {ex.Message}");
        }

    }

    internal void Bind() => GL.UseProgram(Handle);
    internal void Unbind() => GL.UseProgram(0);

    private void DisposeShader(int shader)
    {
        GL.DetachShader(Handle, shader);
        GL.DeleteShader(shader);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteProgram(Handle);
        }
    }
}
