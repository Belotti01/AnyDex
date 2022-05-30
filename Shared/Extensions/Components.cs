namespace AnyDex.Shared.Extensions {
	public static class Components {
		#region Snackbars
		private static void AddGeneric(this ISnackbar snackbar, string message, Severity severity) {
			// Configure snackbar messages here if needed
			snackbar.Add(message, severity);
		}

		public static void AddSuccess(this ISnackbar snackbar, string message)
			=> snackbar.AddGeneric(message, Severity.Success);

		public static void AddWarning(this ISnackbar snackbar, string message)
			=> snackbar.AddGeneric(message, Severity.Warning);

		public static void AddError(this ISnackbar snackbar, string message)
			=> snackbar.AddGeneric(message, Severity.Error);

		public static void AddInfo(this ISnackbar snackbar, string message)
			=> snackbar.AddGeneric(message, Severity.Info);
		#endregion
	}
}
