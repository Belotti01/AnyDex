namespace AnyDex.Shared.Extensions {
	public static class Components {
		private static readonly string[] RandomIcons = {
			Icons.Material.Filled.NordicWalking,
			Icons.Material.Filled.Volcano,
			Icons.Material.Filled.Sledding,
			Icons.Material.Filled.Snowboarding,
			Icons.Material.Filled.Snowshoeing,
			Icons.Material.Filled.SportsMartialArts,
			Icons.Material.Filled.SportsHandball,
			Icons.Material.Filled.EmojiTransportation,
			Icons.Material.Filled.EmojiFoodBeverage
		};

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

		public static string RandomIcon(this Filled material) {
			int randomIndex = Random.Shared.Next(RandomIcons.Length);
			string icon = RandomIcons[randomIndex];
			return icon;
		}
		#endregion
	}
}
