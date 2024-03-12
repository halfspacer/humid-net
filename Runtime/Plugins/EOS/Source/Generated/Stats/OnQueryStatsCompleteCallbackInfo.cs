// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Stats
{
	/// <summary>
	/// Data containing the result information for querying a player's stats request.
	/// </summary>
	public struct OnQueryStatsCompleteCallbackInfo : ICallbackInfo
	{
		/// <summary>
		/// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
		/// </summary>
		public Result ResultCode { get; set; }

		/// <summary>
		/// Context that was passed into <see cref="StatsInterface.QueryStats" />
		/// </summary>
		public object ClientData { get; set; }

		/// <summary>
		/// The Product User ID of the user who initiated this request
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The Product User ID whose stats which were retrieved
		/// </summary>
		public ProductUserId TargetUserId { get; set; }

		public Result? GetResultCode()
		{
			return ResultCode;
		}

		internal void Set(ref OnQueryStatsCompleteCallbackInfoInternal other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			TargetUserId = other.TargetUserId;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct OnQueryStatsCompleteCallbackInfoInternal : ICallbackInfoInternal, IGettable<OnQueryStatsCompleteCallbackInfo>, ISettable<OnQueryStatsCompleteCallbackInfo>, System.IDisposable
	{
		private Result m_ResultCode;
		private System.IntPtr m_ClientData;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_TargetUserId;

		public Result ResultCode
		{
			get
			{
				return m_ResultCode;
			}

			set
			{
				m_ResultCode = value;
			}
		}

		public object ClientData
		{
			get
			{
				object value;
				Helper.Get(m_ClientData, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_ClientData);
			}
		}

		public System.IntPtr ClientDataAddress
		{
			get
			{
				return m_ClientData;
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId value;
				Helper.Get(m_LocalUserId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId value;
				Helper.Get(m_TargetUserId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_TargetUserId);
			}
		}

		public void Set(ref OnQueryStatsCompleteCallbackInfo other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			TargetUserId = other.TargetUserId;
		}

		public void Set(ref OnQueryStatsCompleteCallbackInfo? other)
		{
			if (other.HasValue)
			{
				ResultCode = other.Value.ResultCode;
				ClientData = other.Value.ClientData;
				LocalUserId = other.Value.LocalUserId;
				TargetUserId = other.Value.TargetUserId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ClientData);
			Helper.Dispose(ref m_LocalUserId);
			Helper.Dispose(ref m_TargetUserId);
		}

		public void Get(out OnQueryStatsCompleteCallbackInfo output)
		{
			output = new OnQueryStatsCompleteCallbackInfo();
			output.Set(ref this);
		}
	}
}