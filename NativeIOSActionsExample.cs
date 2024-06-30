using System;
using UnityEngine;

public class NativeIOSActionsExample : BaseIOSFeaturePreview
{
	public Texture2D hello_texture;

	public Texture2D drawTexture;

	private DateTime time;

	private void Awake()
	{
		time = new DateTime(1997, 5, 11);
		IOSSharedApplication.OnUrlCheckResultAction += OnUrlCheckResultAction;
		IOSDateTimePicker instance = ISN_Singleton<IOSDateTimePicker>.instance;
		instance.OnDateChanged = (Action<DateTime>)Delegate.Combine(instance.OnDateChanged, new Action<DateTime>(OnDateChanged));
		IOSDateTimePicker instance2 = ISN_Singleton<IOSDateTimePicker>.instance;
		instance2.OnPickerClosed = (Action<DateTime>)Delegate.Combine(instance2.OnPickerClosed, new Action<DateTime>(OnPickerClosed));
	}

	private void OnGUI()
	{
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Using URL Scheme", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Check if FB App exists"))
		{
			ISN_Singleton<IOSSharedApplication>.instance.CheckUrl("fb://");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open FB Profile"))
		{
			ISN_Singleton<IOSSharedApplication>.instance.OpenUrl("fb://profile");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open App Store"))
		{
			ISN_Singleton<IOSSharedApplication>.instance.OpenUrl("itms-apps://");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get IFA"))
		{
			IOSSharedApplication.OnAdvertisingIdentifierLoadedAction += OnAdvertisingIdentifierLoadedAction;
			ISN_Singleton<IOSSharedApplication>.instance.GetAdvertisingIdentifier();
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Set App Bages Count"))
		{
			IOSNativeUtility.SetApplicationBagesNumber(10);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Clear Application Bages"))
		{
			IOSNativeUtility.SetApplicationBagesNumber(0);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Device Info"))
		{
			ShowDevoceInfo();
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Date Time Picker", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Time"))
		{
			ISN_Singleton<IOSDateTimePicker>.instance.Show(IOSDateTimePickerMode.Time);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date"))
		{
			ISN_Singleton<IOSDateTimePicker>.instance.Show(IOSDateTimePickerMode.Date);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date And Time"))
		{
			ISN_Singleton<IOSDateTimePicker>.instance.Show(IOSDateTimePickerMode.DateAndTime);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Countdown Timer"))
		{
			ISN_Singleton<IOSDateTimePicker>.instance.Show(IOSDateTimePickerMode.CountdownTimer);
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Set Date Without UI"))
		{
			ISN_Singleton<IOSDateTimePicker>.instance.Show(IOSDateTimePickerMode.Date, time);
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Video", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Player Streamed video"))
		{
			ISN_Singleton<IOSVideoManager>.instance.PlayStreamingVideo("https://dl.dropboxusercontent.com/u/83133800/Important/Doosan/GT2100-Video.mov");
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open YouTube Video"))
		{
			ISN_Singleton<IOSVideoManager>.instance.OpenYouTubeVideo("xzCEdSKMkdU");
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Camera Roll", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth + 10, buttonHeight), "Save Screenshot To Camera Roll"))
		{
			IOSCamera.OnImageSaved += OnImageSaved;
			ISN_Singleton<IOSCamera>.Instance.SaveScreenshotToCameraRoll();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Save Texture To Camera Roll"))
		{
			IOSCamera.OnImageSaved += OnImageSaved;
			ISN_Singleton<IOSCamera>.Instance.SaveTextureToCameraRoll(hello_texture);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Camera"))
		{
			IOSCamera.OnImagePicked += OnImage;
			ISN_Singleton<IOSCamera>.Instance.PickImage(ISN_ImageSource.Camera);
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Album"))
		{
			IOSCamera.OnImagePicked += OnImage;
			ISN_Singleton<IOSCamera>.Instance.PickImage(ISN_ImageSource.Album);
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "PickedImage", style);
		StartY += YLableStep;
		if (drawTexture != null)
		{
			GUI.DrawTexture(new Rect(StartX, StartY, buttonWidth, buttonWidth), drawTexture);
		}
	}

	private void ShowDevoceInfo()
	{
		ISN_Device currentDevice = ISN_Device.CurrentDevice;
		IOSMessage.Create("Device Info", string.Concat("Name: ", currentDevice.Name, "\nSystem Name: ", currentDevice.SystemName, "\nModel: ", currentDevice.Model, "\nLocalized Model: ", currentDevice.LocalizedModel, "\nSystem Version: ", currentDevice.SystemVersion, "\nMajor System Version: ", currentDevice.MajorSystemVersion, "\nUser Interface Idiom: ", currentDevice.InterfaceIdiom, "\nGUID in Base64: ", currentDevice.GUID.Base64String));
	}

	private void OnDateChanged(DateTime time)
	{
		ISN_Logger.Log("OnDateChanged: " + time.ToString());
	}

	private void OnPickerClosed(DateTime time)
	{
		ISN_Logger.Log("OnPickerClosed: " + time.ToString());
	}

	private void OnImage(IOSImagePickResult result)
	{
		if (result.IsSucceeded)
		{
			UnityEngine.Object.Destroy(drawTexture);
			drawTexture = result.Image;
			IOSMessage.Create("Success", "Image Successfully Loaded, Image size: " + result.Image.width + "x" + result.Image.height);
		}
		else
		{
			IOSMessage.Create("ERROR", "Image Load Failed");
		}
		IOSCamera.OnImagePicked -= OnImage;
	}

	private void OnImageSaved(ISN_Result result)
	{
		IOSCamera.OnImageSaved -= OnImageSaved;
		if (result.IsSucceeded)
		{
			IOSMessage.Create("Success", "Image Successfully saved to Camera Roll");
		}
		else
		{
			IOSMessage.Create("ERROR", "Image Save Failed");
		}
	}

	private void OnUrlCheckResultAction(ISN_CheckUrlResult result)
	{
		if (result.IsSucceeded)
		{
			IOSMessage.Create("Success", "The " + result.url + " is registered");
		}
		else
		{
			IOSMessage.Create("ERROR", "The " + result.url + " wasn't registered");
		}
	}

	private void OnAdvertisingIdentifierLoadedAction(string Identifier)
	{
		IOSMessage.Create("Identifier Loaded", Identifier);
	}
}
