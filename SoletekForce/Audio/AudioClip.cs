using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace SoletekForce.Audio
{
	public class AudioClip : IDisposable
	{
		internal static SortedDictionary<string, AudioClip> clips = new SortedDictionary<string, AudioClip>();

		readonly int buffer;
		readonly int channels;
		readonly int bits;
		readonly int rate;

		public AudioClip(byte[] data, int channels, int bits, int rate)
		{
			this.channels = channels;
			this.bits = bits;
			this.rate = rate;

			buffer = AL.GenBuffer();
			AL.BufferData(buffer, GetSoundFormat(channels, bits), data, data.Length, rate);
		}

		public static AudioClip LoadClip(string filename)
		{
			using (FileStream stream = File.Open(filename, FileMode.Open)) 
			{
				// Select decoder based on file format

				if (Path.GetExtension(filename) == ".wav")
				{
					return WaveDecoder.LoadWave(stream);
				}
			}

			return null;
		}

		public void Play(int source)
		{
			AL.SourceStop(source);
			AL.Source(source, ALSourcei.Buffer, buffer);
			AL.SourcePlay(source);
		}

		bool disposed;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
			}

			AL.DeleteBuffer(buffer);

			disposed = true;
		}

		public static implicit operator int(AudioClip self)
		{
			return self.buffer;
		}

		public static void Add(string name, string path)
		{
			if (clips.ContainsKey(name)) return;
			AudioClip clip = LoadClip(path);
			if (clip != null) clips.Add(name, clip);
		}

		public static AudioClip Get(string name)
		{
			return clips[name];
		}

		public static ALFormat GetSoundFormat(int channels, int bits)
		{
			switch (channels)
			{
				case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
				case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
				default: throw new NotSupportedException("The specified sound format is not supported.");
			}
		}
	}
}
