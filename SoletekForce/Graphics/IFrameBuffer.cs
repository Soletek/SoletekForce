using System;
namespace SoletekForce
{
	public interface IFrameBuffer
	{
		void Bind();
		void Clear();
		void End();
	}
}
