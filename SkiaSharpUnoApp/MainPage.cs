using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SkiaSharpUnoApp
{
	public class MainPage : Page
	{
		private SKXamlCanvas skiaView;

		private string currentText = "";
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

			FragmentNavigationHandler.FragmentChanged += OnFragmentChanged;

			HandleFragment(FragmentNavigationHandler.CurrentFragment);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);

			FragmentNavigationHandler.FragmentChanged -= OnFragmentChanged;
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

		private void OnFragmentChanged(object sender, FragmentChangedEventArgs e)
		{
			HandleFragment(e.Fragment);
		}

		private void HandleFragment(string fragment)
		{
			var pairs = GetFragmentData(fragment);

			var refresh = false;
			if (pairs.TryGetValue("text", out var text))
			{
				currentText = text;
				refresh = true;
			}

			if (pairs.TryGetValue("color", out var colorStr) && SKColor.TryParse(colorStr, out var color))
			{
				currentColor = color;
				refresh = true;
			}

			if (pairs.TryGetValue("size", out var sizeStr) && int.TryParse(sizeStr, out var size))
			{
				currentSize = size;
				refresh = true;
			}

			if (refresh)
				skiaView.Invalidate();
		}

		private static Dictionary<string, string> GetFragmentData(string fragment)
		{
			var pairStrings = fragment
				.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries));

			var pairs = new Dictionary<string, string>();
			foreach (var pair in pairStrings)
				if (pair.Length == 1)
					pairs[pair[0]] = "";
				else if (pair.Length == 2)
					pairs[pair[0]] = Uri.UnescapeDataString(pair[1] ?? "");
			return pairs;
		}
	}
}
