// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices
{
	/// <summary>
	/// A page query is part of query options. It is used to allow pagination of query results.
	/// </summary>
	public struct PageQuery
	{
		/// <summary>
		/// The index into the ordered query results to start the page at.
		/// </summary>
		public int StartIndex { get; set; }

		/// <summary>
		/// The maximum number of results to have in the page.
		/// </summary>
		public int MaxCount { get; set; }

		internal void Set(ref PageQueryInternal other)
		{
			StartIndex = other.StartIndex;
			MaxCount = other.MaxCount;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct PageQueryInternal : IGettable<PageQuery>, ISettable<PageQuery>, System.IDisposable
	{
		private int m_ApiVersion;
		private int m_StartIndex;
		private int m_MaxCount;

		public int StartIndex
		{
			get
			{
				return m_StartIndex;
			}

			set
			{
				m_StartIndex = value;
			}
		}

		public int MaxCount
		{
			get
			{
				return m_MaxCount;
			}

			set
			{
				m_MaxCount = value;
			}
		}

		public void Set(ref PageQuery other)
		{
			m_ApiVersion = Common.PagequeryApiLatest;
			StartIndex = other.StartIndex;
			MaxCount = other.MaxCount;
		}

		public void Set(ref PageQuery? other)
		{
			if (other.HasValue)
			{
				m_ApiVersion = Common.PagequeryApiLatest;
				StartIndex = other.Value.StartIndex;
				MaxCount = other.Value.MaxCount;
			}
		}

		public void Dispose()
		{
		}

		public void Get(out PageQuery output)
		{
			output = new PageQuery();
			output.Set(ref this);
		}
	}
}