// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.CustomInvites
{
	public struct AddNotifyRequestToJoinRejectedOptions
	{
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyRequestToJoinRejectedOptionsInternal : ISettable<AddNotifyRequestToJoinRejectedOptions>, System.IDisposable
	{
		private int m_ApiVersion;

		public void Set(ref AddNotifyRequestToJoinRejectedOptions other)
		{
			m_ApiVersion = CustomInvitesInterface.AddnotifyrequesttojoinrejectedApiLatest;
		}

		public void Set(ref AddNotifyRequestToJoinRejectedOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = CustomInvitesInterface.AddnotifyrequesttojoinrejectedApiLatest;
			}
		}

		public void Dispose()
		{
		}
	}
}