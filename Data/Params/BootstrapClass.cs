using System.Diagnostics;
using System.Text;

namespace AnyDex.Data.Params {
	public static class BootstrapClass {
		private static readonly ReadOnlyMemory<string> _sizeAcronyms = new(new string[] {
			"sm", "md", "lg", "xl", "xxl"
		});
		/// <summary> Sum of the length of all <see cref="_sizeAcronyms"/>. Useful for <see cref="StringBuilder"/> memory pre-allocation. </summary>
		private static readonly int _totalSizeAcronymsLength;

		static BootstrapClass() {
			var sizeAcronyms = _sizeAcronyms.Span;
			int acronymsLength = 0;
			for(int i = 0; i < sizeAcronyms.Length; i++) {
				acronymsLength += sizeAcronyms[i].Length;
			}
			_totalSizeAcronymsLength = acronymsLength;
		}

		public static string GenerateMarginClasses(params int[] sizeValues)
			=> GenerateSizeClasses("m", sizeValues);

		public static string GeneratePaddingClasses(params int[] sizeValues)
			=> GenerateSizeClasses("p", sizeValues);

		private static string GenerateSizeClasses(string classesPrefix, int[] sizeValues) {
			ReadOnlySpan<string> sizeAcronyms = _sizeAcronyms.Span;
			// Pre-allocate memory for the string
			int stringLength = (classesPrefix.Length + 4) * sizeAcronyms.Length + _totalSizeAcronymsLength;
			StringBuilder classesConcatenator = new(stringLength);

			if(sizeValues.Length == 0) {
				sizeValues = new int[] { 0 };
			}
			int lastIndex = sizeValues.Length - 1;

			for(int i = 0; i < _sizeAcronyms.Length; i++) {
				classesConcatenator.Append($"{classesPrefix}-{sizeAcronyms[i]}-{sizeValues[Math.Min(i, lastIndex)]} ");
			}

#if DEBUG
			// Check for disruptive changes
			if(stringLength != classesConcatenator.Length) {
				string wrongSizeCalculation = $"CALCULATED SIZE: {stringLength} | ACTUAL SIZE: {classesConcatenator.Length}";
				Debugger.Break();
			}
#endif

			return classesConcatenator.ToString();
		}

	}
}
