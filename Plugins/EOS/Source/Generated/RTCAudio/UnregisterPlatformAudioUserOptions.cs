// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
	/// <summary>
	/// This struct is used to remove a user from the audio system.
	/// </summary>
	public struct UnregisterPlatformAudioUserOptions
	{
		/// <summary>
		/// The account of a user associated with this event.
		/// </summary>
		public Utf8String UserId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct UnregisterPlatformAudioUserOptionsInternal : ISettable<UnregisterPlatformAudioUserOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_UserId;

		public Utf8String UserId
		{
			set
			{
				Helper.Set(value, ref m_UserId);
			}
		}

		public void Set(ref UnregisterPlatformAudioUserOptions other)
		{
			m_ApiVersion = RTCAudioInterface.UnregisterplatformaudiouserApiLatest;
			UserId = other.UserId;
		}

		public void Set(ref UnregisterPlatformAudioUserOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = RTCAudioInterface.UnregisterplatformaudiouserApiLatest;
				UserId = other.Value.UserId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_UserId);
		}
	}
}