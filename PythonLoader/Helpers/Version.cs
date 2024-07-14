using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Python_Loader.Helpers
{
    internal class Version
    {
        public int Major;
        public int Minor;
        public int Patch;

        public Version((int major, int minor, int patch) version)
        {
            this.Major = version.major;
            this.Minor = version.minor;
            this.Patch = version.patch;
        }

        /// <summary>
        /// Converts the string into usable ints for <see cref="Major"/>, <see cref="Minor"/> and <see cref="Patch"/>
        /// </summary>
        /// <param name="version"></param>
        public Version (string version)
        {
            var v = ConvertVersion(version);
            this.Major = v.major;
            this.Minor = v.minor;
            this.Patch = v.patch;
        }

        /// <summary>
        /// Empty version. All values set to 0.
        /// </summary>
        public Version ()
        {
            this.Major = 0;
            this.Minor = 0;
            this.Patch = 0;
        }

        public override string ToString()
        {
            string output = string.Empty;
            if (this.Major > 0)
                output += Major;
            if (this.Minor > 0)
                output += "." + this.Minor;
            if (this.Patch > 0)
                output += "." + this.Patch;
            return output;
        }

        static internal (int major, int minor, int patch) ConvertVersion(string version)
        {
            if (!version.Contains("."))
            {
                // Assume the package only contains a major version.
                return (int.Parse(version), 0, 0);
            }

            string[] parts = version.Split('.');
            int major = int.Parse(parts[0]);
            int minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            int patch = parts.Length > 2 ? int.Parse(parts[2]) : 0;

            return (major, minor, patch);
        }
    }
}
