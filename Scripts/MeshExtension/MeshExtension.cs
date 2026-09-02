using System;
using UnityEngine;

public static class MeshExtension
{
	public unsafe static void ApplyVertices(Mesh mesh, Vector3[] vertices, int count)
	{
		fixed (IntPtr* ptr = vertices)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.vertices = vertices;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyNormals(Mesh mesh, Vector3[] normals, int count)
	{
		fixed (IntPtr* ptr = normals)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.normals = normals;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyTangents(Mesh mesh, Vector4[] tangents, int count)
	{
		fixed (IntPtr* ptr = tangents)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.tangents = tangents;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyUvs(Mesh mesh, Vector2[] uvs, int channel, int count)
	{
		fixed (IntPtr* ptr = uvs)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				switch (channel)
				{
				case 0:
					mesh.uv = uvs;
					break;
				case 1:
					mesh.uv2 = uvs;
					break;
				case 2:
					mesh.uv3 = uvs;
					break;
				case 3:
					mesh.uv4 = uvs;
					break;
				}
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyColors32(Mesh mesh, Color32[] colors32, int count)
	{
		fixed (IntPtr* ptr = colors32)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.colors32 = colors32;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyColors(Mesh mesh, Color[] colors, int count)
	{
		fixed (IntPtr* ptr = colors)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.colors = colors;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyColors(Mesh mesh, Color32[] colors32, int count)
	{
		fixed (IntPtr* ptr = colors32)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.colors32 = colors32;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyTriangles(Mesh mesh, int[] triangles, int count)
	{
		fixed (IntPtr* ptr = triangles)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.triangles = triangles;
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}

	public unsafe static void ApplyTriangles(Mesh mesh, int subMeshIndex, int[] triangles, int count)
	{
		fixed (IntPtr* ptr = triangles)
		{
			UIntPtr* ptr2 = (UIntPtr*)((byte*)ptr - sizeof(UIntPtr));
			UIntPtr uIntPtr = *ptr2;
			try
			{
				*ptr2 = (UIntPtr)(ulong)count;
				mesh.SetTriangles(triangles, subMeshIndex);
			}
			finally
			{
				*ptr2 = uIntPtr;
			}
		}
	}
}
