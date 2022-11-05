// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
	/// <summary>
	/// Input parameters for the EOS_UI_ReportKeyEvent function.
	/// </summary>
	public struct ReportKeyEventOptions
	{
		/// <summary>
		/// The input data pushed to the SDK.
		/// </summary>
		public System.IntPtr PlatformSpecificInputData { get; set; }

		internal void Set(ref ReportKeyEventOptionsInternal other)
		{
			PlatformSpecificInputData = other.PlatformSpecificInputData;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct ReportKeyEventOptionsInternal : IGettable<ReportKeyEventOptions>, ISettable<ReportKeyEventOptions>, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_PlatformSpecificInputData;

		public System.IntPtr PlatformSpecificInputData
		{
			get
			{
				return m_PlatformSpecificInputData;
			}

			set
			{
				m_PlatformSpecificInputData = value;
			}
		}

		public void Set(ref ReportKeyEventOptions other)
		{
			m_ApiVersion = UIInterface.ReportkeyeventApiLatest;
			PlatformSpecificInputData = other.PlatformSpecificInputData;
		}

		public void Set(ref ReportKeyEventOptions? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = UIInterface.ReportkeyeventApiLatest;
				PlatformSpecificInputData = other.Value.PlatformSpecificInputData;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_PlatformSpecificInputData);
		}

		public void Get(out ReportKeyEventOptions output)
		{
			output = new ReportKeyEventOptions();
			output.Set(ref this);
		}
	}
}