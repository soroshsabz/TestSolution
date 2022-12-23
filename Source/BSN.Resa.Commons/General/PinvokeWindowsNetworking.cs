using System;
using System.Runtime.InteropServices;

namespace BSN.Resa.Commons
{
	public class PinvokeWindowsNetworking
	{
		#region Consts
		const int ResourceConnected = 0x00000001;
		const int ResourceGlobalnet = 0x00000002;
		const int ResourceRemembered = 0x00000003;

		const int ResourcetypeAny = 0x00000000;
		const int ResourcetypeDisk = 0x00000001;
		const int ResourcetypePrint = 0x00000002;

		const int ResourcedisplaytypeGeneric = 0x00000000;
		const int ResourcedisplaytypeDomain = 0x00000001;
		const int ResourcedisplaytypeServer = 0x00000002;
		const int ResourcedisplaytypeShare = 0x00000003;
		const int ResourcedisplaytypeFile = 0x00000004;
		const int ResourcedisplaytypeGroup = 0x00000005;

		const int ResourceusageConnectable = 0x00000001;
		const int ResourceusageContainer = 0x00000002;


		const int ConnectInteractive = 0x00000008;
		const int ConnectPrompt = 0x00000010;
		const int ConnectRedirect = 0x00000080;
		const int ConnectUpdateProfile = 0x00000001;
		const int ConnectCommandline = 0x00000800;
		const int ConnectCmdSavecred = 0x00001000;

		const int ConnectLocaldrive = 0x00000100;
		#endregion

		#region Errors
		const int NoError = 0;

		const int ErrorAccessDenied = 5;
		const int ErrorAlreadyAssigned = 85;
		const int ErrorBadDevice = 1200;
		const int ErrorBadNetName = 67;
		const int ErrorBadProvider = 1204;
		const int ErrorCancelled = 1223;
		const int ErrorExtendedError = 1208;
		const int ErrorInvalidAddress = 487;
		const int ErrorInvalidParameter = 87;
		const int ErrorInvalidPassword = 1216;
		const int ErrorMoreData = 234;
		const int ErrorNoMoreItems = 259;
		const int ErrorNoNetOrBadPath = 1203;
		const int ErrorNoNetwork = 1222;

		const int ErrorBadProfile = 1206;
		const int ErrorCannotOpenProfile = 1205;
		const int ErrorDeviceInUse = 2404;
		const int ErrorNotConnected = 2250;
		const int ErrorOpenFiles = 2401;

		private struct ErrorClass
		{
			public readonly int Num;
			public readonly string Message;
			public ErrorClass(int num, string message)
			{
				Num = num;
				Message = message;
			}
		}


		// Created with excel formula:
		// ="new ErrorClass("&A1&", """&PROPER(SUBSTITUTE(MID(A1,7,LEN(A1)-6), "_", " "))&"""), "
		private static readonly ErrorClass[] ErrorList = {
			new ErrorClass(ErrorAccessDenied, "Error: Access Denied"),
			new ErrorClass(ErrorAlreadyAssigned, "Error: Already Assigned"),
			new ErrorClass(ErrorBadDevice, "Error: Bad Device"),
			new ErrorClass(ErrorBadNetName, "Error: Bad Net Name"),
			new ErrorClass(ErrorBadProvider, "Error: Bad Provider"),
			new ErrorClass(ErrorCancelled, "Error: Cancelled"),
			new ErrorClass(ErrorExtendedError, "Error: Extended Error"),
			new ErrorClass(ErrorInvalidAddress, "Error: Invalid Address"),
			new ErrorClass(ErrorInvalidParameter, "Error: Invalid Parameter"),
			new ErrorClass(ErrorInvalidPassword, "Error: Invalid Password"),
			new ErrorClass(ErrorMoreData, "Error: More Data"),
			new ErrorClass(ErrorNoMoreItems, "Error: No More Items"),
			new ErrorClass(ErrorNoNetOrBadPath, "Error: No Net Or Bad Path"),
			new ErrorClass(ErrorNoNetwork, "Error: No Network"),
			new ErrorClass(ErrorBadProfile, "Error: Bad Profile"),
			new ErrorClass(ErrorCannotOpenProfile, "Error: Cannot Open Profile"),
			new ErrorClass(ErrorDeviceInUse, "Error: Device In Use"),
			new ErrorClass(ErrorExtendedError, "Error: Extended Error"),
			new ErrorClass(ErrorNotConnected, "Error: Not Connected"),
			new ErrorClass(ErrorOpenFiles, "Error: Open Files"),
		};

		private static string GetErrorForNumber(int errNum)
		{
			foreach (ErrorClass er in ErrorList)
			{
				if (er.Num == errNum) return er.Message;
			}
			return "Error: Unknown, " + errNum;
		}
		#endregion

		[DllImport("Mpr.dll")]
		private static extern int WNetUseConnection(
			IntPtr hwndOwner,
			Netresource lpNetResource,
			string lpPassword,
			string lpUserId,
			int dwFlags,
			string lpAccessName,
			string lpBufferSize,
			string lpResult
		);

		[DllImport("Mpr.dll")]
		private static extern int WNetCancelConnection2(
			string lpName,
			int dwFlags,
			bool fForce
		);

		[StructLayout(LayoutKind.Sequential)]
		private class Netresource
		{
			public int dwScope = 0;
			public int dwType = 0;
			public int dwDisplayType = 0;
			public int dwUsage = 0;
			public string lpLocalName = "";
			public string lpRemoteName = "";
			public string lpComment = "";
			public string lpProvider = "";
		}


		public static string ConnectToRemote(string remoteUnc, string username, string password)
		{
			return ConnectToRemote(remoteUnc, username, password, false);
		}

		public static string ConnectToRemote(string remoteUnc, string username, string password, bool promptUser)
		{
			var nr = new Netresource
			{
				dwType = ResourcetypeDisk,
				lpRemoteName = remoteUnc
			};
			//			nr.lpLocalName = "F:";

			int ret;
			if (promptUser)
				ret = WNetUseConnection(IntPtr.Zero, nr, "", "", ConnectInteractive | ConnectPrompt, null, null, null);
			else
				ret = WNetUseConnection(IntPtr.Zero, nr, password, username, 0, null, null, null);

			if (ret == NoError) return null;
			return GetErrorForNumber(ret);
		}

		public static string DisconnectRemote(string remoteUnc)
		{
			int ret = WNetCancelConnection2(remoteUnc, ConnectUpdateProfile, false);
			if (ret == NoError) return null;
			return GetErrorForNumber(ret);
		}
	}
}
