using ChartJs.Blazor.Common;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using ChartType = ChartJs.Blazor.Common.Enums.ChartType;

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

	public class ChartDataset<TData> : Dataset<TData>
		where TData : ChartData, new() {
		public ChartDataset(ChartType type, string? id = null) : base(type, id) { }
	}

	public static class ChartDataExtensions {
		public static PieConfig GeneratePieChartConfig<TData>(this IEnumerable<ChartData<TData>> data, string title, bool isDarkMode) {
			string fontColor = isDarkMode
				? "#FFFFFF"
				: "#000000";

			PieConfig _config = new(false) {
				Options = new() {
					Title = new() {
						Text = title,
						Display = true,
						FontSize = 24,
						FontColor = fontColor
					},
					Responsive = true
				}
			};

			foreach(string label in data.Select(x => x.UnlocalizedLabel)) {
				_config.Data.Labels.Add(label);
			}
			PieDataset<TData> dataset = new(false) {
				BackgroundColor = new(data.Select(x => x.Color).ToArray())
			};
			foreach(var item in data) {
				dataset.Add(item.Value);
			}

			_config.Data.Datasets.Add(dataset);

			return _config;
		}
	}
}
