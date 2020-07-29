using System;
using System.Threading;
using Uno.Foundation;
using Uno.Foundation.Interop;

namespace SkiaSharpUnoApp
{
	public static class FragmentNavigationHandler
	{
		private static readonly FragmentNavigationHandlerJsInterop jsInterop = new FragmentNavigationHandlerJsInterop();

		public static string CurrentFragment
		{
			get => jsInterop.CurrentFragment;
			set => jsInterop.CurrentFragment = value;
		}

		public static event EventHandler<FragmentChangedEventArgs> FragmentChanged
		{
			add => jsInterop.FragmentChanged += value;
			remove => jsInterop.FragmentChanged -= value;
		}

		// JavaScript interop

		private class FragmentNavigationHandlerJsInterop : IJSObject, IJSObjectMetadata
		{
			private static long handleCounter = 0L;

			private readonly long jsHandle;

			private event EventHandler<FragmentChangedEventArgs> FragmentChangedProxy;

			public FragmentNavigationHandlerJsInterop()
			{
				jsHandle = Interlocked.Increment(ref handleCounter);
				Handle = JSObjectHandle.Create(this, this);
			}

			public JSObjectHandle Handle { get; }

			public void OnFragmentChanged(string fragment)
			{
				fragment = fragment.Trim('#');
				FragmentChangedProxy?.Invoke(null, new FragmentChangedEventArgs(fragment));
			}

			public string CurrentFragment
			{
				get
				{
					var fragment = WebAssemblyRuntime.InvokeJSWithInterop($"return {this}.getCurrentFragment();");
					return fragment.Trim('#');
				}
				set
				{
					var escaped = WebAssemblyRuntime.EscapeJs(value);
					WebAssemblyRuntime.InvokeJSWithInterop($"{this}.setCurrentFragment(\"{escaped}\");");
				}
			}

			public event EventHandler<FragmentChangedEventArgs> FragmentChanged
			{
				add
				{
					FragmentChangedProxy += value;
					WebAssemblyRuntime.InvokeJSWithInterop($"{this}.subscribeToFragmentChanged();");
				}
				remove
				{
					FragmentChangedProxy -= value;
				}
			}

			// IJSObjectMetadata

			long IJSObjectMetadata.CreateNativeInstance(IntPtr managedHandle)
			{
				WebAssemblyRuntime.InvokeJS($"SkiaSharpUnoApp.FragmentNavigationHandler.createInstance('{managedHandle}', '{jsHandle}')");
				return jsHandle;
			}

			string IJSObjectMetadata.GetNativeInstance(IntPtr managedHandle, long jsHandle) =>
				$"SkiaSharpUnoApp.FragmentNavigationHandler.getInstance('{jsHandle}')";

			void IJSObjectMetadata.DestroyNativeInstance(IntPtr managedHandle, long jsHandle) =>
				WebAssemblyRuntime.InvokeJS($"SkiaSharpUnoApp.FragmentNavigationHandler.destroyInstance('{jsHandle}')");

			object IJSObjectMetadata.InvokeManaged(object instance, string method, string parameters)
			{
				switch (method)
				{
					case nameof(OnFragmentChanged):
						OnFragmentChanged(parameters);
						break;

					default:
						throw new ArgumentException($"Unable to execute method: {method}", nameof(method));
				}

				return null;
			}
		}
	}

	public class FragmentChangedEventArgs : EventArgs
	{
		public FragmentChangedEventArgs(string fragment)
		{
			Fragment = fragment;
		}

		public string Fragment { get; }
	}
}
