public static class Toast
{
	public static void Show(string message, float duration = 2f)
	{
		UISimplePopup.Toast(localization: false, message, duration);
	}

	public static void Show(bool localization, string message, float duration = 2f)
	{
		UISimplePopup.Toast(localization, message, duration);
	}
}
