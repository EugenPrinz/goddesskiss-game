using System;
using System.Collections.Generic;

public class AN_GrantPermissionsResult : AN_Result
{
	private Dictionary<AN_ManifestPermission, AN_PermissionState> _RequestedPermissionsState = new Dictionary<AN_ManifestPermission, AN_PermissionState>();

	public Dictionary<AN_ManifestPermission, AN_PermissionState> RequestedPermissionsState => _RequestedPermissionsState;

	public AN_GrantPermissionsResult(string[] permissionsList, string[] resultsList)
		: base(IsResultSucceeded: true)
	{
		int num = 0;
		foreach (string fullName in permissionsList)
		{
			AN_ManifestPermission permissionByName = PermissionsManager.GetPermissionByName(fullName);
			int value = Convert.ToInt32(resultsList[num]);
			_RequestedPermissionsState.Add(permissionByName, (AN_PermissionState)value);
			num++;
		}
	}
}
