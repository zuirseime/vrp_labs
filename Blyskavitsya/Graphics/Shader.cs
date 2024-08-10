using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Blyskavitsya.Graphics;
public sealed class Shader : IDisposable
{
    private bool _disposed;

    internal int Handle { get; private set; }

    public Shader(string vertexPath, string fragmentPath)
    {
        Handle = GL.CreateProgram();

        int vertexShader = CreateShader(ShaderType.VertexShader, vertexPath);
        int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentPath);

        LinkProgram();

        DisposeShader(vertexShader);
    }

    private int CreateShader(ShaderType type, string path)
    {
        int shader = GL.CreateShader(type);

        GL.ShaderSource(shader, Shader.LoadSource(path));
        Shader.CompileShader(shader, type);
        GL.AttachShader(Handle, shader);

        return shader;
    }

    private void LinkProgram()
    {
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == (int)All.False)
            throw new Exception($"Error linking program: {GL.GetProgramInfoLog(Handle)}");
    }

    public void SetInt(string name, int value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
    public void SetFloat(string name, float value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
    public void SetVector3(string name, Vector3 value) => GL.Uniform3(GL.GetUniformLocation(Handle, name), value);
    public void SetVector4(string name, Color4 value) => GL.Uniform4(GL.GetUniformLocation(Handle, name), value);
    public void SetMatrix4(string name, Matrix4 value) => GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), true, ref value);

    private static void CompileShader(int shader, ShaderType type)
    {
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == (int)All.False)
            throw new Exception($"Error compiling {type}: {GL.GetShaderInfoLog(shader)}");
    }

    private static string LoadSource(string path)
    {
        try
        {
            string source = File.ReadAllText(Path.Combine("public/shaders", path));
            Console.WriteLine(source + '\n');

            return source;
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

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GL.DeleteProgram(Handle);
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
