// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
	/// <summary>
	/// Output parameters for the <see cref="LobbyInterface.RejectInvite" /> function.
	/// </summary>
	public struct RejectInviteCallbackInfo : ICallbackInfo
	{
		/// <summary>
		/// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
		/// </summary>
		public Result ResultCode { get; set; }

		/// <summary>
		/// Context that was passed into <see cref="LobbyInterface.RejectInvite" />
		/// </summary>
		public object ClientData { get; set; }

		/// <summary>
		/// The ID of the invitation being rejected
		/// </summary>
		public Utf8String InviteId { get; set; }

		public Result? GetResultCode()
		{
			return ResultCode;
		}

		internal void Set(ref RejectInviteCallbackInfoInternal other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			InviteId = other.InviteId;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct RejectInviteCallbackInfoInternal : ICallbackInfoInternal, IGettable<RejectInviteCallbackInfo>, ISettable<RejectInviteCallbackInfo>, System.IDisposable
	{
		private Result m_ResultCode;
		private System.IntPtr m_ClientData;
		private System.IntPtr m_InviteId;

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

		public Utf8String InviteId
		{
			get
			{
				Utf8String value;
				Helper.Get(m_InviteId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_InviteId);
			}
		}

		public void Set(ref RejectInviteCallbackInfo other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			InviteId = other.InviteId;
		}

		public void Set(ref RejectInviteCallbackInfo? other)
		{
			if (other.HasValue)
			{
				ResultCode = other.Value.ResultCode;
				ClientData = other.Value.ClientData;
				InviteId = other.Value.InviteId;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ClientData);
			Helper.Dispose(ref m_InviteId);
		}

		public void Get(out RejectInviteCallbackInfo output)
		{
			output = new RejectInviteCallbackInfo();
			output.Set(ref this);
		}
	}
}