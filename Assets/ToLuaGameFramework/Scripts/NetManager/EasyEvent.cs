using System;

namespace ToLuaGameFramework
{	
	public class EasyEvent<T> 
	{
		private Action<T> mOnEvent = e => { };

		public void Register(Action<T> onEvent)
		{
			mOnEvent += onEvent;
		}

		public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;

		public void Trigger(T t) => mOnEvent?.Invoke(t);

	}
}