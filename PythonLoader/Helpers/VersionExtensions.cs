using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Python_Loader.Helpers
{
    internal static class VersionExtensions
    {
        /// <summary>
        /// Compare this version to another one. Returns true if your version is higher than the compared one, and false it equal to or lower than the other one.
        /// Use the out bool isEqual to check if the values are the same.
        /// </summary>
        /// <param name="myVersion"></param>
        /// <param name="compareTo"></param>
        /// <param name="isEqual">Is true if the versions are the same.</param>
        /// <returns></returns>
        static internal bool IsHigherVersion(this Version myVersion, Version compareTo, out bool isEqual)
        {
            isEqual = myVersion.Major == compareTo.Major && myVersion.Minor == compareTo.Minor && myVersion.Patch == compareTo.Patch;

            if (myVersion.Major > compareTo.Major)
                return true;
            if (myVersion.Minor > compareTo.Minor)
                return true;
            if (myVersion.Patch > compareTo.Patch)
                return true;

            return false;
        }
    }
}
