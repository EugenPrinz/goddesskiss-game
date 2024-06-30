using System;

public class IOSNotificationDeviceToken
{
	private string _tokenString;

	private byte[] _tokenBytes;

	public string tokenString => _tokenString;

	public byte[] tokenBytes => _tokenBytes;

	public IOSNotificationDeviceToken(byte[] token)
	{
		_tokenBytes = token;
		_tokenString = BitConverter.ToString(token).Replace("-", string.Empty).ToLower();
	}
}
