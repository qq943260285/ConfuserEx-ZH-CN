// dnlib: See LICENSE.txt for more info

using System;

namespace dnlib.DotNet {
    /// <summary>
    /// 方法属性，请参阅CorHdr.h / CorMethodAttr
    /// </summary>
    [Flags]
	public enum MethodAttributes : ushort {
		/// <summary>member access mask - Use this mask to retrieve accessibility information.</summary>
		MemberAccessMask	= 0x0007,
		/// <summary>Member not referenceable.</summary>
		PrivateScope		= 0x0000,
		/// <summary>Member not referenceable.</summary>
		CompilerControlled	= PrivateScope,
		/// <summary>Accessible only by the parent type.</summary>
		Private				= 0x0001,
		/// <summary>Accessible by sub-types only in this Assembly.</summary>
		FamANDAssem			= 0x0002,
		/// <summary>Accessibly by anyone in the Assembly.</summary>
		Assembly			= 0x0003,
		/// <summary>Accessible only by type and sub-types.</summary>
		Family				= 0x0004,
		/// <summary>Accessibly by sub-types anywhere, plus anyone in assembly.</summary>
		FamORAssem			= 0x0005,
        /// <summary>任何能够了解此范围的人都可以访问。</summary>
        Public = 0x0006,

        /// <summary>在类型上定义，否则按实例定义。</summary>
        Static = 0x0010,
		/// <summary>Method may not be overridden.</summary>
		Final				= 0x0020,
		/// <summary>Method virtual.</summary>
		Virtual				= 0x0040,
        /// <summary>方法按名称+ sig隐藏，否则只按名称隐藏。</summary>
        HideBySig = 0x0080,

		/// <summary>vtable layout mask - Use this mask to retrieve vtable attributes.</summary>
		VtableLayoutMask	= 0x0100,
        /// <summary>默认。</summary>
        ReuseSlot = 0x0000,
		/// <summary>Method always gets a new slot in the vtable.</summary>
		NewSlot				= 0x0100,

		/// <summary>Overridability is the same as the visibility.</summary>
		CheckAccessOnOverride = 0x0200,
		/// <summary>Method does not provide an implementation.</summary>
		Abstract			= 0x0400,
		/// <summary>Method is special.  Name describes how.</summary>
		SpecialName			= 0x0800,

		/// <summary>Implementation is forwarded through pinvoke.</summary>
		PinvokeImpl			= 0x2000,
		/// <summary>Implementation is forwarded through pinvoke.</summary>
		PInvokeImpl			= PinvokeImpl,
		/// <summary>Managed method exported via thunk to unmanaged code.</summary>
		UnmanagedExport		= 0x0008,

		/// <summary>Runtime should check name encoding.</summary>
		RTSpecialName		= 0x1000,
		/// <summary>Method has security associate with it.</summary>
		HasSecurity			= 0x4000,
		/// <summary>Method calls another method containing security code.</summary>
		RequireSecObject	= 0x8000,
	}
}
