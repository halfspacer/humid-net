// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Achievements
{
	/// <summary>
	/// Input parameters for the <see cref="AchievementsInterface.QueryPlayerAchievements" /> function.
	/// </summary>
	public struct QueryPlayerAchievementsOptions
	{
		/// <summary>
		/// The Product User ID for the user whose achievements are to be retrieved.
		/// </summary>
		public ProductUserId TargetUserId { get; set; }

		/// <summary>
		/// The Product User ID for the user who is querying for player achievements. For a Dedicated Server this should be null.
		/// </summary>
		public ProductUserId LocalUserId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct QueryPlayerAchievementsOptionsInternal : ISettable<QueryPlayerAchievementsOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_TargetUserId;
		private System.IntPtr m_LocalUserId;

		public ProductUserId TargetUserId
		{
			set
			{
				Helper.Set(value, ref m_TargetUserId);
			}
		}

		public ProductUserId LocalUserId
		{
			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public void Set(ref QueryPlayerAchievementsOptions other)
		{
			m_ApiVersion = AchievementsInterface.QueryplayerachievementsApiLatest;
			TargetUserId = other.TargetUserId;
			LocalUserId = other.LocalUserId;
		}

		public void Set(ref QueryPlayerAchievementsOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = AchievementsInterface.QueryplayerachievementsApiLatest;
				TargetUserId = other.Value.TargetUserId;
				LocalUserId = other.Value.LocalUserId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_TargetUserId);
			Helper.Dispose(ref m_LocalUserId);
		}
	}
}