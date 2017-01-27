using System;
namespace SoletekForce.Backend
{
	public interface IBaseBackend
	{
		void Init();
		void Start();
		void Stop();
		void Unload();
	}
}
