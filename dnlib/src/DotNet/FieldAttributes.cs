// dnlib: See LICENSE.txt for more info

using System;

namespace dnlib.DotNet
{
    /// <summary>
    /// 字段标志，请参阅CorHdr.h / CorFieldAttr
    /// </summary>
    [Flags]
    public enum FieldAttributes : ushort
    {
        /// <summary>member access mask - Use this mask to retrieve accessibility information.</summary>
        FieldAccessMask = 0x0007,
        /// <summary>Member not referenceable.</summary>
        PrivateScope = 0x0000,
        /// <summary>Member not referenceable.</summary>
        CompilerControlled = PrivateScope,
        /// <summary>Accessible only by the parent type.</summary>
        Private = 0x0001,
        /// <summary>Accessible by sub-types only in this Assembly.</summary>
        FamANDAssem = 0x0002,
        /// <summary>Accessibly by anyone in the Assembly.</summary>
        Assembly = 0x0003,
        /// <summary>Accessible only by type and sub-types.</summary>
        Family = 0x0004,
        /// <summary>Accessibly by sub-types anywhere, plus anyone in assembly.</summary>
        FamORAssem = 0x0005,
        /// <summary>任何能够了解此范围的人都可以访问。</summary>
        Public = 0x0006,

        /// <summary>在类型上定义，否则按实例定义。</summary>
        Static = 0x0010,
        /// <summary>Field may only be initialized, not written to after init.</summary>
        InitOnly = 0x0020,
        /// <summary>Value is compile time constant.</summary>
        Literal = 0x0040,
        /// <summary>Field does not have to be serialized when type is remoted.</summary>
        NotSerialized = 0x0080,

        /// <summary>field is special.  Name describes how.</summary>
        SpecialName = 0x0200,

        /// <summary>Implementation is forwarded through pinvoke.</summary>
        PinvokeImpl = 0x2000,

        /// <summary>Runtime(metadata internal APIs) should check name encoding.</summary>
        RTSpecialName = 0x0400,
        /// <summary>Field has marshalling information.</summary>
        HasFieldMarshal = 0x1000,
        /// <summary>Field has default.</summary>
        HasDefault = 0x8000,
        /// <summary>Field has RVA.</summary>
        HasFieldRVA = 0x0100,
    }
}
