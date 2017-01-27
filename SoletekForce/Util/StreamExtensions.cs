using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SoletekForce.Utilities
{
	public static class StreamExtensions
	{
		public static void ProcessDataPacket<TData>(this BinaryReader reader, int start, int end, Action<TData> action = null)
			where TData : IConvertible
		{
			int lenght = end - start;
			byte[] byteData = reader.ReadBytes(lenght);
			object data;

			if (typeof(TData).IsValueType)
			{
				Debug.Assert(Marshal.SizeOf(typeof(TData)) == lenght);
				GCHandle handle = GCHandle.Alloc(byteData, GCHandleType.Pinned);
				data = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TData));
				handle.Free();
			}
			else if (typeof(TData) == typeof(string))
			{
				data = Encoding.Default.GetString(byteData);
			}
			else
			{
				throw new NotSupportedException("TData type not supported");
			}

			if (action != null) action((TData)data);
		}
	}
}
