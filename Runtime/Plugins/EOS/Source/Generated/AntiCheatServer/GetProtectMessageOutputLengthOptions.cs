// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatServer
{
	public struct GetProtectMessageOutputLengthOptions
	{
		/// <summary>
		/// Length in bytes of input
		/// </summary>
		public uint DataLengthBytes { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct GetProtectMessageOutputLengthOptionsInternal : ISettable<GetProtectMessageOutputLengthOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private uint m_DataLengthBytes;

		public uint DataLengthBytes
		{
			set
			{
				m_DataLengthBytes = value;
			}
		}

		public void Set(ref GetProtectMessageOutputLengthOptions other)
		{
			m_ApiVersion = AntiCheatServerInterface.GetprotectmessageoutputlengthApiLatest;
			DataLengthBytes = other.DataLengthBytes;
		}

		public void Set(ref GetProtectMessageOutputLengthOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = AntiCheatServerInterface.GetprotectmessageoutputlengthApiLatest;
				DataLengthBytes = other.Value.DataLengthBytes;
			}
		}

		public void Dispose()
		{
		}
	}
}