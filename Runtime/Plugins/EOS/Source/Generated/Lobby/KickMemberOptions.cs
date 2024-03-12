// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
	/// <summary>
	/// Input parameters for the <see cref="LobbyInterface.KickMember" /> function.
	/// </summary>
	public struct KickMemberOptions
	{
		/// <summary>
		/// The ID of the lobby
		/// </summary>
		public Utf8String LobbyId { get; set; }

		/// <summary>
		/// The Product User ID of the local user requesting the removal; this user must be the lobby owner
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The Product User ID of the lobby member to remove
		/// </summary>
		public ProductUserId TargetUserId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct KickMemberOptionsInternal : ISettable<KickMemberOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LobbyId;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_TargetUserId;

		public Utf8String LobbyId
		{
			set
			{
				Helper.Set(value, ref m_LobbyId);
			}
		}

		public ProductUserId LocalUserId
		{
			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public ProductUserId TargetUserId
		{
			set
			{
				Helper.Set(value, ref m_TargetUserId);
			}
		}

		public void Set(ref KickMemberOptions other)
		{
			m_ApiVersion = LobbyInterface.KickmemberApiLatest;
			LobbyId = other.LobbyId;
			LocalUserId = other.LocalUserId;
			TargetUserId = other.TargetUserId;
		}

		public void Set(ref KickMemberOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = LobbyInterface.KickmemberApiLatest;
				LobbyId = other.Value.LobbyId;
				LocalUserId = other.Value.LocalUserId;
				TargetUserId = other.Value.TargetUserId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_LobbyId);
			Helper.Dispose(ref m_LocalUserId);
			Helper.Dispose(ref m_TargetUserId);
		}
	}
}