using Newtonsoft.Json;
using System;
using System.Threading;
using Uno.Foundation;
using Uno.Foundation.Interop;

namespace SkiaSharpUnoApp
{
	public static class SkiaSharpFunctions
	{
		private static readonly SkiaSharpFunctionsJsInterop jsInterop;

		static SkiaSharpFunctions()
		{
			jsInterop = new SkiaSharpFunctionsJsInterop();
			jsInterop.SetCurrent();
		}

		public static event EventHandler<DataReceivedEventArgs> DataReceived
		{
			add => jsInterop.DataReceived += value;
			remove => jsInterop.DataReceived -= value;
		}

		// JavaScript interop

		private class SkiaSharpFunctionsJsInterop : IJSObject, IJSObjectMetadata
		{
			private static long handleCounter = 0L;

			private readonly long jsHandle;

			public SkiaSharpFunctionsJsInterop()
			{
				jsHandle = Interlocked.Increment(ref handleCounter);
				Handle = JSObjectHandle.Create(this, this);
			}

			public JSObjectHandle Handle { get; }

			public event EventHandler<DataReceivedEventArgs> DataReceived;

			public void SetCurrent() =>
				WebAssemblyRuntime.InvokeJSWithInterop($"{this}.setCurrent();");

			private void OnDataReceived(DataReceivedEventArgs e) =>
				DataReceived?.Invoke(null, e);

			// IJSObjectMetadata

			long IJSObjectMetadata.CreateNativeInstance(IntPtr managedHandle)
			{
				WebAssemblyRuntime.InvokeJS($"SkiaSharpUnoApp.SkiaSharpFunctions.createInstance('{managedHandle}', '{jsHandle}')");
				return jsHandle;
			}

			string IJSObjectMetadata.GetNativeInstance(IntPtr managedHandle, long jsHandle) =>
				$"SkiaSharpUnoApp.SkiaSharpFunctions.getInstance('{jsHandle}')";

			void IJSObjectMetadata.DestroyNativeInstance(IntPtr managedHandle, long jsHandle) =>
				WebAssemblyRuntime.InvokeJS($"SkiaSharpUnoApp.SkiaSharpFunctions.destroyInstance('{jsHandle}')");

			object IJSObjectMetadata.InvokeManaged(object instance, string method, string parameters)
			{
				switch (method)
				{
					case nameof(OnDataReceived):
						OnDataReceived(new DataReceivedEventArgs(parameters));
						break;

					default:
						throw new ArgumentException($"Unable to execute method: {method}", nameof(method));
				}

				return null;
			}
		}
	}

	public class DataReceivedEventArgs : EventArgs
	{
		public DataReceivedEventArgs(string json)
		{
			Json = json;
		}

		public string Json { get; }

		public T Get<T>() =>
			JsonConvert.DeserializeObject<T>(Json);
	}
}
