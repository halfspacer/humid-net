// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Version
{
	public sealed partial class VersionInterface
	{
		public static readonly Utf8String CompanyName = "Epic Games, Inc.";

		public static readonly Utf8String CopyrightString = "Copyright Epic Games, Inc. All Rights Reserved.";

		public const int MajorVersion = 1;

		public const int MinorVersion = 16;

		public const int PatchVersion = 1;

		public static readonly Utf8String ProductIdentifier = "Epic Online Services SDK";

		public static readonly Utf8String ProductName = "Epic Online Services SDK";

		/// <summary>
		/// Get the version of the EOSSDK binary
		/// </summary>
		public static Utf8String GetVersion()
		{
			var funcResult = Bindings.EOS_GetVersion();

			Utf8String funcResultReturn;
			Helper.Get(funcResult, out funcResultReturn);
			return funcResultReturn;
		}
	}
}