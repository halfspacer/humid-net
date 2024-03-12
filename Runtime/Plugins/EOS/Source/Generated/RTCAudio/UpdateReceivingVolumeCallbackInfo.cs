// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
	/// <summary>
	/// This struct is passed in with a call to <see cref="OnUpdateReceivingVolumeCallback" />.
	/// </summary>
	public struct UpdateReceivingVolumeCallbackInfo : ICallbackInfo
	{
		/// <summary>
		/// This returns:
		/// <see cref="Result.Success" /> if receiving volume of channels of the local user was successfully changed.
		/// <see cref="Result.UnexpectedError" /> otherwise.
		/// </summary>
		public Result ResultCode { get; set; }

		/// <summary>
		/// Client-specified data passed into <see cref="RTCAudioInterface.UpdateReceivingVolume" />.
		/// </summary>
		public object ClientData { get; set; }

		/// <summary>
		/// The Product User ID of the user who initiated this request.
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The room this settings should be applied on.
		/// </summary>
		public Utf8String RoomName { get; set; }

		/// <summary>
		/// The volume that was set for received audio (range 0.0 to 100.0).
		/// </summary>
		public float Volume { get; set; }

		public Result? GetResultCode()
		{
			return ResultCode;
		}

		internal void Set(ref UpdateReceivingVolumeCallbackInfoInternal other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			Volume = other.Volume;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct UpdateReceivingVolumeCallbackInfoInternal : ICallbackInfoInternal, IGettable<UpdateReceivingVolumeCallbackInfo>, ISettable<UpdateReceivingVolumeCallbackInfo>, System.IDisposable
	{
		private Result m_ResultCode;
		private System.IntPtr m_ClientData;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_RoomName;
		private float m_Volume;

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

		public Utf8String RoomName
		{
			get
			{
				Utf8String value;
				Helper.Get(m_RoomName, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_RoomName);
			}
		}

		public float Volume
		{
			get
			{
				return m_Volume;
			}

			set
			{
				m_Volume = value;
			}
		}

		public void Set(ref UpdateReceivingVolumeCallbackInfo other)
		{
			ResultCode = other.ResultCode;
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			Volume = other.Volume;
		}

		public void Set(ref UpdateReceivingVolumeCallbackInfo? other)
		{
			if (other.HasValue)
			{
				ResultCode = other.Value.ResultCode;
				ClientData = other.Value.ClientData;
				LocalUserId = other.Value.LocalUserId;
				RoomName = other.Value.RoomName;
				Volume = other.Value.Volume;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ClientData);
			Helper.Dispose(ref m_LocalUserId);
			Helper.Dispose(ref m_RoomName);
		}

		public void Get(out UpdateReceivingVolumeCallbackInfo output)
		{
			output = new UpdateReceivingVolumeCallbackInfo();
			output.Set(ref this);
		}
	}
}