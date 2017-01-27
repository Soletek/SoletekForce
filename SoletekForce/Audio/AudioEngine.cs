using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace SoletekForce.Audio
{
	public static class AudioEngine
	{
		static AudioContext audioContext;

		public static void Init()
		{
			audioContext = new AudioContext();

			UpdateListenerLocation();
		}

		public static void UpdateListenerLocation()
		{
			if (audioContext != null)
			{
				var pos = Graphics.Camera.Position;
				var at = Graphics.Camera.LookAt;
				var up = Graphics.Camera.Up;

				AL.Listener(ALListenerfv.Orientation, ref at, ref up);
				AL.Listener(ALListener3f.Position, ref pos);
			}
		}

		public static void SetMasterVolume(float volume)
		{
			AL.Listener(ALListenerf.Gain, volume);
		}
	}
}
