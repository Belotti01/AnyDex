using ChartJs.Blazor.Util;

namespace AnyDex.Data.Params {
	public class ChartData<T> {
		public T Value { get; set; }
		public string UnlocalizedLabel { get; set; }
		public string Color { get; set; }


		public ChartData(T value, string unlocalizedLabel, byte r, byte g, byte b) {
			Value = value;
			UnlocalizedLabel = unlocalizedLabel;
			Color = ColorUtil.ColorHexString(r, g, b);
		}

		public ChartData(T value, string unlocalizedLabel, string color) {
			Value = value;
			UnlocalizedLabel = unlocalizedLabel;
			Color = color;
		}

		public ChartData(T value, string unlocalizedLabel, System.Drawing.Color color) {
			Value = value;
			UnlocalizedLabel = unlocalizedLabel;
			Color = ColorUtil.FromDrawingColor(color);
		}
	}
}
