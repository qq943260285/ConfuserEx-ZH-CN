// dnlib: See LICENSE.txt for more info

using System;

namespace dnlib.DotNet
{
    /// <summary>
    /// TypeDef和ExportedType标志。请参见CorHdr.h / CorTypeAttr
    /// </summary>
    [Flags]
    public enum TypeAttributes : uint
    {
        /// <summary>使用此掩码可以检索类型可见性信息。</summary>
        VisibilityMask = 0x00000007,
        /// <summary>类不是公共范围。</summary>
        NotPublic = 0x00000000,
        /// <summary>类是公共范围。</summary>
        Public = 0x00000001,
        /// <summary>类与公共可见性嵌套。</summary>
        NestedPublic = 0x00000002,
        /// <summary>类与私有可见性嵌套。</summary>
        NestedPrivate = 0x00000003,
        /// <summary>类与家庭可见性嵌套。</summary>
        NestedFamily = 0x00000004,
        /// <summary>类与程序集可见性嵌套。</summary>
        NestedAssembly = 0x00000005,
        /// <summary>类嵌套了族和组件可见性。</summary>
        NestedFamANDAssem = 0x00000006,
        /// <summary>类与系列或程序集可见性嵌套。</summary>
        NestedFamORAssem = 0x00000007,

        /// <summary>Use this mask to retrieve class layout information</summary>
        LayoutMask = 0x00000018,
        /// <summary>类字段是自动布局的</summary>
        AutoLayout = 0x00000000,
        /// <summary>类字段按顺序排列</summary>
        SequentialLayout = 0x00000008,
        /// <summary>布局是明确提供的</summary>
        ExplicitLayout = 0x00000010,

        /// <summary>Use this mask to retrieve class semantics information.</summary>
        ClassSemanticsMask = 0x00000020,
        /// <summary>Use this mask to retrieve class semantics information.</summary>
        ClassSemanticMask = ClassSemanticsMask,
        /// <summary>类型是一个类。</summary>
        Class = 0x00000000,
        /// <summary>Type is an interface.</summary>
        Interface = 0x00000020,

        /// <summary>Class is abstract</summary>
        Abstract = 0x00000080,
        /// <summary>Class is concrete and may not be extended</summary>
        Sealed = 0x00000100,
        /// <summary>Class name is special.  Name describes how.</summary>
        SpecialName = 0x00000400,

        /// <summary>Class / interface is imported</summary>
        Import = 0x00001000,
        /// <summary>The class is Serializable.</summary>
        Serializable = 0x00002000,
        /// <summary>The type is a Windows Runtime type</summary>
        WindowsRuntime = 0x00004000,

        /// <summary>Use StringFormatMask to retrieve string information for native interop</summary>
        StringFormatMask = 0x00030000,
        /// <summary>LPTSTR在此类中被解释为ANSI</summary>
        AnsiClass = 0x00000000,
        /// <summary>LPTSTR is interpreted as UNICODE</summary>
        UnicodeClass = 0x00010000,
        /// <summary>LPTSTR is interpreted automatically</summary>
        AutoClass = 0x00020000,
        /// <summary>A non-standard encoding specified by CustomFormatMask</summary>
        CustomFormatClass = 0x00030000,
        /// <summary>Use this mask to retrieve non-standard encoding information for native interop. The meaning of the values of these 2 bits is unspecified.</summary>
        CustomFormatMask = 0x00C00000,

        /// <summary>Initialize the class any time before first static field access.</summary>
        BeforeFieldInit = 0x00100000,
        /// <summary>This ExportedType is a type forwarder.</summary>
        Forwarder = 0x00200000,

        /// <summary>Flags reserved for runtime use.</summary>
        ReservedMask = 0x00040800,
        /// <summary>Runtime should check name encoding.</summary>
        RTSpecialName = 0x00000800,
        /// <summary>Class has security associate with it.</summary>
        HasSecurity = 0x00040000,
    }
}
