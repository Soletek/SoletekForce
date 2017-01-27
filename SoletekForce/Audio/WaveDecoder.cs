using System;
using System.IO;
using SoletekForce.Utilities;

namespace SoletekForce.Audio
{
	public static class WaveDecoder
	{
		public static AudioClip LoadWave(Stream stream)
		{
			byte[] data;
			int channels = 0;
			int bits = 0;
			int rate = 0;

			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.ProcessDataPacket<string>(0, 4, (signature) => 					// RIFF signature
				{
					if (signature != "RIFF") throw new NotSupportedException("File is not supported");
				});
				reader.ProcessDataPacket<int>(4, 8);									// File size
				reader.ProcessDataPacket<string>(8, 12, (signature) =>					// WAVE signature
				{
					if (signature != "WAVE") throw new NotSupportedException("File is not supported");
				});
				reader.ProcessDataPacket<string>(12, 16, (signature) =>					// fmt signature
				{
					if (signature != "fmt ") throw new NotSupportedException("File is not supported");
				});
				reader.ProcessDataPacket<int>(16, 20);									// Format data lenght
				reader.ProcessDataPacket<short>(20, 22);								// Format type
				reader.ProcessDataPacket<short>(22, 24, (value) => channels = value);	// Number of channels
				reader.ProcessDataPacket<int>(24, 28, (value) => rate = value);			// Sample rate
				reader.ProcessDataPacket<int>(28, 32);									// Byte rate
				reader.ProcessDataPacket<short>(32, 34);								// Block align
				reader.ProcessDataPacket<short>(34, 36, (value) => bits = value);		// Bits per sample
				reader.ProcessDataPacket<string>(36, 40, (blockHeader) => 				// Data chunk headers
				{
					while (blockHeader.ToLower() != "data")
					{
						int uselessDataBlockLenght = reader.ReadInt32();
						reader.ReadBytes(uselessDataBlockLenght);
						blockHeader = new string(reader.ReadChars(4));
					}
				});
				reader.ProcessDataPacket   <int>(40, 44);								// Data section size
				data = reader.ReadBytes((int)reader.BaseStream.Length);					// Data
			}

			return new AudioClip(data, channels, bits, rate);
		}
	}
}
