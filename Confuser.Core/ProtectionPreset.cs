using System;

namespace Confuser.Core {
	/// <summary>
	///     Various presets of protections.
	/// </summary>
	public enum ProtectionPreset {
		/// <summary> The protection does not belong to any preset. </summary>
		无 = 0,

        /// <summary> The protection provides basic security. </summary>
        最小 = 1,

        /// <summary> The protection provides normal security for public release. </summary>
        正常 = 2,

        /// <summary> The protection provides better security with observable performance impact. </summary>
        侵略性 = 3,

		/// <summary> The protection provides strongest security with possible incompatibility. </summary>
		最大 = 4
	}
}