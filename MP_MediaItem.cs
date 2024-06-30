public class MP_MediaItem
{
	private string _Id;

	private string _Title;

	private string _Artist;

	private string _AlbumTitle;

	private string _AlbumArtist;

	private string _Genre;

	private string _Composer;

	public string Id => _Id;

	public string Title => _Title;

	public string Artist => _Artist;

	public string AlbumTitle => _AlbumTitle;

	public string AlbumArtist => _AlbumArtist;

	public string Genre => _Genre;

	public string Composer => _Composer;

	public MP_MediaItem(string id, string title, string artist, string albumTitle, string albumArtist, string genre, string composer)
	{
		_Id = id;
		_Title = title;
		_Artist = artist;
		_AlbumTitle = albumTitle;
		_AlbumArtist = albumArtist;
		_Genre = genre;
		_Composer = composer;
	}
}
