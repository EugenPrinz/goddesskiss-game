public class ISN_Locale
{
	private string _CountryCode;

	private string _DisplayCountry;

	private string _LanguageCode;

	private string _DisplayLanguage;

	public string CountryCode => _CountryCode;

	public string DisplayCountry => _DisplayCountry;

	public string LanguageCode => _LanguageCode;

	public string DisplayLanguage => _DisplayLanguage;

	public ISN_Locale(string countryCode, string contryName, string languageCode, string languageName)
	{
		_CountryCode = countryCode;
		_DisplayCountry = contryName;
		_LanguageCode = languageCode;
		_DisplayLanguage = languageName;
	}
}
