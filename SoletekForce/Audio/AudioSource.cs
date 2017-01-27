using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace SoletekForce.Audio
{
	public class AudioSource : IDisposable
	{
		protected Entity holder;

		int currentChannel;
		int maxChannels;
		readonly List<int> channels;
		readonly Dictionary<AudioClip, float> cooldowns;

		/// <summary>
		/// Gets or sets the cooldown time.
		/// </summary>
		/// <value>The time in which same clip will not be played twice.</value>
		public float CooldownTime { get; set; } = 0;

		public AudioSource(Entity holder) : this(holder, 4) { }

		public AudioSource(Entity holder, int maxChannels)
		{
			this.holder = holder;
			this.maxChannels = maxChannels;
			channels = new List<int>(maxChannels);
			cooldowns = new Dictionary<AudioClip, float>();

			for (int i = 0; i < maxChannels; i++)
			{
				channels.Add(AL.GenSource());
			}
		}

		public void PlayClip(AudioClip clip, float volume, float pitch)
		{
			int channel = channels[currentChannel++];
			if (currentChannel >= maxChannels) currentChannel = 0;

			if (CooldownTime > 0)
			{
				if (cooldowns.ContainsKey(clip) && cooldowns[clip] > Timer.Runtime) return;

				cooldowns[clip] = Timer.Runtime + CooldownTime;
			}

			Vector3 pos = new Vector3();

			if (holder != null) pos = holder.Transform.Position;

			AL.Source(channel, ALSourcef.Gain, volume);
			AL.Source(channel, ALSourcef.Pitch, pitch);
			AL.Source(channel, ALSource3f.Position, ref pos);
			clip.Play(channel);
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

			foreach (int channel in channels)
			{
				AL.DeleteSource(channel);
			}

			disposed = true;
		}
	}
}
