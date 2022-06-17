namespace AnyDex.Shared.Extensions {
	public static class Logic {
		/// <summary>
		/// Repeat the last item of the <paramref name="array"/> (or <see langword="default"/> if it's empty) so 
		/// that its size equals <paramref name="toSize"/>.
		/// </summary>
		public static T[] Pad<T>(this T[] array, int toSize) where T : struct {
			if(array.Length == toSize) return array;	// Correct size already
			if(array.Length > toSize) return array[..(toSize - 1)];		// Over the size - truncate it

			T lastValue = array.Length == 0
				? default
				: array.Last();

			T[] paddedArray = new T[toSize];

			for(int i = 0; i < toSize; i++) {
				paddedArray[i] = array.Length > i 
					? array[i] 
					: lastValue;
			}

			return paddedArray;
		}
	}
}
