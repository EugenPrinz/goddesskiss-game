using System;
using System.Collections;
using System.Collections.Generic;
using ANMiniJSON;
using UnityEngine;

public class FB_UserInfo
{
	private string _id = string.Empty;

	private string _name = string.Empty;

	private string _first_name = string.Empty;

	private string _last_name = string.Empty;

	private string _username = string.Empty;

	private string _profile_url = string.Empty;

	private string _email = string.Empty;

	private string _location = string.Empty;

	private string _locale = string.Empty;

	private string _rawJSON = string.Empty;

	private DateTime _Birthday = default(DateTime);

	private FB_Gender _gender = FB_Gender.Male;

	private Dictionary<FB_ProfileImageSize, Texture2D> profileImages = new Dictionary<FB_ProfileImageSize, Texture2D>();

	public string RawJSON => _rawJSON;

	public string Id => _id;

	public DateTime Birthday => _Birthday;

	public string Name => _name;

	public string FirstName => _first_name;

	public string LastName => _last_name;

	public string UserName => _username;

	public string ProfileUrl => _profile_url;

	public string Email => _email;

	public string Locale => _locale;

	public string Location => _location;

	public FB_Gender Gender => _gender;

	public event Action<FB_UserInfo> OnProfileImageLoaded = delegate
	{
	};

	public FB_UserInfo(string data)
	{
		_rawJSON = data;
		IDictionary jSON = Json.Deserialize(_rawJSON) as IDictionary;
		InitializeData(jSON);
	}

	public FB_UserInfo(IDictionary JSON)
	{
		InitializeData(JSON);
	}

	public void InitializeData(IDictionary JSON)
	{
		if (JSON.Contains("id"))
		{
			_id = Convert.ToString(JSON["id"]);
		}
		if (JSON.Contains("birthday"))
		{
			_Birthday = DateTime.Parse(Convert.ToString(JSON["birthday"]));
		}
		if (JSON.Contains("name"))
		{
			_name = Convert.ToString(JSON["name"]);
		}
		if (JSON.Contains("first_name"))
		{
			_first_name = Convert.ToString(JSON["first_name"]);
		}
		if (JSON.Contains("last_name"))
		{
			_last_name = Convert.ToString(JSON["last_name"]);
		}
		if (JSON.Contains("username"))
		{
			_username = Convert.ToString(JSON["username"]);
		}
		if (JSON.Contains("link"))
		{
			_profile_url = Convert.ToString(JSON["link"]);
		}
		if (JSON.Contains("email"))
		{
			_email = Convert.ToString(JSON["email"]);
		}
		if (JSON.Contains("locale"))
		{
			_locale = Convert.ToString(JSON["locale"]);
		}
		if (JSON.Contains("location"))
		{
			IDictionary dictionary = JSON["location"] as IDictionary;
			_location = Convert.ToString(dictionary["name"]);
		}
		if (JSON.Contains("gender"))
		{
			string text = Convert.ToString(JSON["gender"]);
			if (text.Equals("male"))
			{
				_gender = FB_Gender.Male;
			}
			else
			{
				_gender = FB_Gender.Female;
			}
		}
	}

	public string GetProfileUrl(FB_ProfileImageSize size)
	{
		return "https://graph.facebook.com/" + Id + "/picture?type=" + size;
	}

	public Texture2D GetProfileImage(FB_ProfileImageSize size)
	{
		if (profileImages.ContainsKey(size))
		{
			return profileImages[size];
		}
		return null;
	}

	public void LoadProfileImage(FB_ProfileImageSize size)
	{
		if (GetProfileImage(size) != null)
		{
			this.OnProfileImageLoaded(this);
		}
		SA_WWWTextureLoader sA_WWWTextureLoader = SA_WWWTextureLoader.Create();
		switch (size)
		{
		case FB_ProfileImageSize.large:
			sA_WWWTextureLoader.OnLoad += OnLargeImageLoaded;
			break;
		case FB_ProfileImageSize.normal:
			sA_WWWTextureLoader.OnLoad += OnNormalImageLoaded;
			break;
		case FB_ProfileImageSize.small:
			sA_WWWTextureLoader.OnLoad += OnSmallImageLoaded;
			break;
		case FB_ProfileImageSize.square:
			sA_WWWTextureLoader.OnLoad += OnSquareImageLoaded;
			break;
		}
		sA_WWWTextureLoader.LoadTexture(GetProfileUrl(size));
	}

	private void OnSquareImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.square))
		{
			profileImages.Add(FB_ProfileImageSize.square, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnLargeImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.large))
		{
			profileImages.Add(FB_ProfileImageSize.large, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnNormalImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.normal))
		{
			profileImages.Add(FB_ProfileImageSize.normal, image);
		}
		this.OnProfileImageLoaded(this);
	}

	private void OnSmallImageLoaded(Texture2D image)
	{
		if (image != null && !profileImages.ContainsKey(FB_ProfileImageSize.small))
		{
			profileImages.Add(FB_ProfileImageSize.small, image);
		}
		this.OnProfileImageLoaded(this);
	}
}
