// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
	/// <summary>
	/// Input parameters for the <see cref="SessionSearch.Find" /> function.
	/// </summary>
	public struct SessionSearchFindOptions
	{
		/// <summary>
		/// The Product User ID of the local user who is searching
		/// </summary>
		public ProductUserId LocalUserId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct SessionSearchFindOptionsInternal : ISettable<SessionSearchFindOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LocalUserId;

		public ProductUserId LocalUserId
		{
			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public void Set(ref SessionSearchFindOptions other)
		{
			m_ApiVersion = SessionSearch.SessionsearchFindApiLatest;
			LocalUserId = other.LocalUserId;
		}

		public void Set(ref SessionSearchFindOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = SessionSearch.SessionsearchFindApiLatest;
				LocalUserId = other.Value.LocalUserId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_LocalUserId);
		}
	}
}