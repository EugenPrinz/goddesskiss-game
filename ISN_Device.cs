using System;

public class ISN_Device
{
	private static ISN_Device _CurrentDevice;

	private string _Name = "Test Name";

	private string _SystemName = "iPhone OS";

	private string _Model = "iPhone";

	private string _LocalizedModel = "iPhone";

	private string _SystemVersion = "9.0.0";

	private int _MajorSystemVersion = 9;

	private ISN_InterfaceIdiom _InterfaceIdiom;

	private ISN_DeviceGUID _GUID = new ISN_DeviceGUID(string.Empty);

	public string Name => _Name;

	public string SystemName => _SystemName;

	public string Model => _Model;

	public string LocalizedModel => _LocalizedModel;

	public string SystemVersion => _SystemVersion;

	public int MajorSystemVersion => _MajorSystemVersion;

	public ISN_InterfaceIdiom InterfaceIdiom => _InterfaceIdiom;

	public ISN_DeviceGUID GUID => _GUID;

	public static ISN_Device CurrentDevice
	{
		get
		{
			if (_CurrentDevice == null)
			{
				_CurrentDevice = new ISN_Device();
			}
			return _CurrentDevice;
		}
	}

	public ISN_Device()
	{
	}

	public ISN_Device(string deviceData)
	{
		string[] array = deviceData.Split('|');
		_Name = array[0];
		_SystemName = array[1];
		_Model = array[2];
		_LocalizedModel = array[3];
		_SystemVersion = array[4];
		_MajorSystemVersion = Convert.ToInt32(array[5]);
		_InterfaceIdiom = (ISN_InterfaceIdiom)Convert.ToInt32(array[6]);
		_GUID = new ISN_DeviceGUID(array[7]);
	}
}
