using System;

namespace BSN.Resa.DoctorApp.iOS.Commons.Utilities
{
	public static class VersionExtension
	{
		public static Version Normalize(this Version version)
		{
			int Normalize(int sectionValue)
			{
				return sectionValue >= 0 ? sectionValue : 0;
			}

			return new Version(
				Normalize(version.Major), 
				Normalize(version.Minor), 
				Normalize(version.Build), 
				Normalize(version.Revision)
			);
		}
	}
}