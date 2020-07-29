using ClassLibrary;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SkiaSharpUnoApp
{
	public class MainPage : Page
	{
		private SKXamlCanvas skiaView;

		private string currentText = "Hello World!";
		private SKColor currentColor = SKColors.Black;
		private int currentSize = 24;

		public MainPage()
		{
			skiaView = new SKXamlCanvas();
			skiaView.PaintSurface += OnPaintSurface;

			Content = skiaView;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			SkiaSharpFunctions.DataReceived += OnDataReceived;
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);

			SkiaSharpFunctions.DataReceived -= OnDataReceived;
		}

		private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;
			var info = e.Info;

			canvas.Clear(SKColors.Azure);

			using var paint = new SKPaint
			{
				IsAntialias = true,
				Color = currentColor,
				TextSize = currentSize * (float)skiaView.Dpi,
				TextAlign = SKTextAlign.Center,
			};

			var x = info.Width / 2;
			var y = (info.Height + paint.TextSize) / 2;

			canvas.DrawText(currentText, x, y, paint);
		}

		private void OnDataReceived(object sender ,DataReceivedEventArgs e)
		{
			var data = e.Get<RedrawData>();

			currentText = data.Text;

			if (SKColor.TryParse(data.Color, out var color))
				currentColor = color;

			currentSize = data.Size;

			skiaView.Invalidate();
		}
	}
}
