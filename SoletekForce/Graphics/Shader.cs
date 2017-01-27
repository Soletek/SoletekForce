using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace SoletekForce.Graphics
{
	public class Shader
	{
		int vertex_shader;
		int fragment_shader;
		int handle;
		int vao;

		string vertex_code;
		string fragment_code;

		Action enableAction;
		Action disableAction;

		readonly Dictionary<string, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
		readonly Dictionary<string, UniformInfo> Uniforms = new Dictionary<string, UniformInfo>();

		public Shader(string vertName, string fragName)
		{
			vertex_code = File.ReadAllText(vertName + ".vert");
			fragment_code = File.ReadAllText(fragName + ".frag");
		}

		public void Load()
		{
			handle = GL.CreateProgram();

			// Compile shaders
			{
				vertex_shader = GL.CreateShader(ShaderType.VertexShader);
				GL.ShaderSource(vertex_shader, vertex_code);
				GL.CompileShader(vertex_shader);
				GL.AttachShader(handle, vertex_shader);
				Console.WriteLine("VShader: <" + GL.GetShaderInfoLog(vertex_shader) + ">");

				fragment_shader = GL.CreateShader(ShaderType.FragmentShader);
				GL.ShaderSource(fragment_shader, fragment_code);
				GL.CompileShader(fragment_shader);
				GL.AttachShader(handle, fragment_shader);
				Console.WriteLine("FShader: <" + GL.GetShaderInfoLog(fragment_shader) + ">");
			}

			// Link program
			{
				GL.LinkProgram(handle);

				int AttributeCount;
				int UniformCount;

				GL.GetProgram(handle, GetProgramParameterName.ActiveAttributes, out AttributeCount);
				GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out UniformCount);

				foreach (int i in Enumerable.Range(0, AttributeCount))
				{
					var info = new AttributeInfo();
					var name = new StringBuilder(256);
					int length = 0;

					GL.GetActiveAttrib(handle, i, 256, out length, out info.size, out info.type, name);

					info.name = name.ToString();
					info.handle = GL.GetAttribLocation(handle, info.name);
					Attributes.Add(name.ToString(), info);
				}

				foreach (int i in Enumerable.Range(0, UniformCount))
				{
					var info = new UniformInfo();
					var name = new StringBuilder(256);
					int length = 0;

					GL.GetActiveUniform(handle, i, 256, out length, out info.size, out info.type, name);

					info.name = name.ToString();
					info.handle = GL.GetUniformLocation(handle, info.name);
					Uniforms.Add(name.ToString(), info);
				}
			}

			Console.WriteLine("Shader: <" + GL.GetProgramInfoLog(handle) + ">");

            GL.DetachShader(handle, vertex_shader);
			GL.DetachShader(handle, fragment_shader);
			vertex_code = null;
			fragment_code = null;
		}

		public void Use()
		{
			GL.UseProgram(handle);
		}

		public void Begin()
		{
			Use();
			GL.BindVertexArray(vao);
			if (enableAction != null) enableAction();
		}

		public void SetGLActions(Action enable, Action disable = null)
		{
			enableAction = enable;
			disableAction = disable;
		}

		public void Disable()
		{
			GL.UseProgram(0);
			if (disableAction != null) disableAction();
		}

		internal void GenerateVAO(GraphicsBuffer buffer, VertexAttribute[] metadata)
		{
			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.GetVBOHandle());

			foreach (var attribute in Attributes.Values)
			{
				GL.EnableVertexAttribArray(attribute.handle);
				var attrMeta = VertexAttribute.FindByName(attribute.name, metadata);
				GL.VertexAttribPointer(attribute.handle, attrMeta.size, attrMeta.type,
					attrMeta.normalized, attrMeta.stride, attrMeta.offset);
			}

			GL.BindVertexArray(0);
		}

		void BindVertexAttribute(AttributeInfo attribute)
		{
			GL.EnableVertexAttribArray(attribute.handle);
			GL.VertexAttribPointer(attribute.handle, attribute.size, VertexAttribPointerType.Float, false, 40, 0);
		}

		public int GetAttribute(string name)
		{
			if (Attributes.ContainsKey(name))
			{
				return Attributes[name].handle;
			}
			return -1;
		}

		public int GetUniform(string name)
		{
			if (Uniforms.ContainsKey(name))
			{
				return Uniforms[name].handle;
			}
			return -1;
		}

		public void LoadUniformMatrix(string name, OpenTK.Matrix4 value)
		{
			GL.UniformMatrix4(GetUniform(name), false, ref value);
		}

		public void LoadUniformFloat(string name, float value)
		{
			if (GetUniform(name) != -1)
			{
				GL.Uniform1(GetUniform(name), value);
			}
		}
	}

	public struct AttributeInfo
	{
		public string name;
		public int handle;
		public int size;
		public ActiveAttribType type;
	}

	public struct UniformInfo
	{
		public string name;
		public int handle;
		public int size;
		public ActiveUniformType type;
	}
}
