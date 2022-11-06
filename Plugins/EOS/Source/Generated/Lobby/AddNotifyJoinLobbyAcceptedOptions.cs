// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
	public struct AddNotifyJoinLobbyAcceptedOptions
	{
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyJoinLobbyAcceptedOptionsInternal : ISettable<AddNotifyJoinLobbyAcceptedOptions>, System.IDisposable
	{
		private int m_ApiVersion;

		public void Set(ref AddNotifyJoinLobbyAcceptedOptions other)
		{
			m_ApiVersion = LobbyInterface.AddnotifyjoinlobbyacceptedApiLatest;
		}

		public void Set(ref AddNotifyJoinLobbyAcceptedOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = LobbyInterface.AddnotifyjoinlobbyacceptedApiLatest;
			}
		}

		public void Dispose()
		{
		}
	}
}