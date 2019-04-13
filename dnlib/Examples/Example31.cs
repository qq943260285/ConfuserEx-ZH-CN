using System;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Utils;

namespace dnlib.Examples
{
    // 此示例创建一个新程序集并将其保存到磁盘。
    // 这是反编译它的样子：
    //
    // using System;
    // namespace My.Namespace
    // {
    //     internal class Startup
    //     {
    //         private static int Main(string[] args)
    //         {
    //             Console.WriteLine("Hello World!");
    //             return 0;
    //         }
    //     }
    // }
    public class Example31
    {
        public static void Run()
        {
            // 创建一个新模块。 传入的字符串是模块的名称，而不是文件名。
            ModuleDef mod = new ModuleDefUser("MyModule.exe");
            // 这是一个控制台应用程序
            mod.Kind = ModuleKind.Console;

            //将模块添加到装配中
            AssemblyDef asm = new AssemblyDefUser("MyAssembly", new Version(1, 2, 3, 4), null, "");
            asm.Modules.Add(mod);

            //添加.NET资源
            //         byte[] resourceData = Encoding.UTF8.GetBytes("Hello, world!");
            //mod.Resources.Add(new EmbeddedResource("My.Resource", resourceData,
            //				ManifestResourceAttributes.Private));

            // 添加启动类型。 它派生自System.Object。
            TypeDef startUpType = new TypeDefUser("My.Namespace", "Startup", mod.CorLibTypes.Object.TypeDefOrRef);
            startUpType.Attributes = TypeAttributes.NotPublic | TypeAttributes.AutoLayout |
                                    TypeAttributes.Class | TypeAttributes.AnsiClass;
            // 将类型添加到模块
            mod.Types.Add(startUpType);

            // 创建入口点方法
            MethodDef entryPoint = new MethodDefUser("Main",
                MethodSig.CreateStatic(mod.CorLibTypes.Void, new SZArraySig(mod.CorLibTypes.String)));
            entryPoint.Attributes = MethodAttributes.Private | MethodAttributes.Static |
                            MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
            entryPoint.ImplAttributes = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            // 命名第一个参数（参数0是返回类型）
            entryPoint.ParamDefs.Add(new ParamDefUser("args", 1));
            // 将方法添加到启动类型
            startUpType.Methods.Add(entryPoint);
            // 设置模块入口点
            mod.EntryPoint = entryPoint;

            // 创建TypeRef到System.Console
            TypeRef consoleRef = new TypeRefUser(mod, "System", "Console", mod.CorLibTypes.AssemblyRef);
            // 创建方法ref为'System.Void System.Console :: WriteLine（System.String）'
            MemberRef consoleWrite1 = new MemberRefUser(mod, "WriteLine",
                        MethodSig.CreateStatic(mod.CorLibTypes.Void, mod.CorLibTypes.String),
                        consoleRef);

            //System.Console::ReadKey()'
            MemberRef memberRefReadLine = new MemberRefUser(
                mod,
                "ReadLine",
              //MethodSig.CreateStatic(mod.CorLibTypes.Void),
              MethodSig.CreateStatic(mod.CorLibTypes.String),
                consoleRef
                );

            // 创建TypeRef到System.Console
            TypeRef int32Ref = new TypeRefUser(mod, "System", "Int32", mod.CorLibTypes.AssemblyRef);

            MemberRef memberRefParse = new MemberRefUser(
                mod,
                "Parse",
              MethodSig.CreateStatic(mod.CorLibTypes.Int32, mod.CorLibTypes.String),
                int32Ref
                );

            //string[mscorlib] System.String::Concat(object[])
            TypeRef stringRef = new TypeRefUser(mod, "System", "String", mod.CorLibTypes.AssemblyRef);

            MemberRef memberRefSystemStringConcat = new MemberRefUser(
                mod,
                "Concat",
              MethodSig.CreateStatic(mod.CorLibTypes.String, new SZArraySig(mod.CorLibTypes.Object)),
                stringRef
                );


            // 将CIL方法体添加到入口点方法
            CilBody epBody = new CilBody();
            entryPoint.Body = epBody;
            epBody.Variables.Add(new Local(mod.CorLibTypes.String, "s1"));
            epBody.Variables.Add(new Local(mod.CorLibTypes.Int32, "i1"));
            epBody.Variables.Add(new Local(mod.CorLibTypes.Int32, "i2"));
            epBody.Variables.Add(new Local(mod.CorLibTypes.String, "s2"));


            //: nop
            //: ldstr     "xyzs"
            //: call      void [mscorlib]System.Console::WriteLine(string)
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("小宇专属"));
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            //: nop
            //: call      string [mscorlib]System.Console::ReadLine()
            //: stloc.0
            //: ldloc.0
            //: call      void [mscorlib]System.Console::WriteLine(string)
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefReadLine));
            epBody.Instructions.Add(OpCodes.Stloc_0.ToInstruction());
            epBody.Instructions.Add(OpCodes.Ldloc_0.ToInstruction());
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            //: nop
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            //: call      string [mscorlib]System.Console::ReadLine()
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefReadLine));
            //: call      int32 [mscorlib]System.Int32::Parse(string)
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefParse));
            //: stloc.1
            epBody.Instructions.Add(OpCodes.Stloc_1.ToInstruction());
            //: call      string [mscorlib]System.Console::ReadLine()
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefReadLine));
            //: call      int32 [mscorlib]System.Int32::Parse(string)
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefParse));
            //: stloc.2
            epBody.Instructions.Add(OpCodes.Stloc_2.ToInstruction());
            //: ldc.i4.5
            epBody.Instructions.Add(OpCodes.Ldc_I4_5.ToInstruction());
            //: newarr    [mscorlib]System.Object
            epBody.Instructions.Add(OpCodes.Newarr.ToInstruction(mod.CorLibTypes.Object));
            //: dup
            epBody.Instructions.Add(OpCodes.Dup.ToInstruction());
            //: ldc.i4.0
            epBody.Instructions.Add(OpCodes.Ldc_I4_0.ToInstruction());
            //: ldloc.1
            epBody.Instructions.Add(OpCodes.Ldloc_1.ToInstruction());
            //: box       [mscorlib]System.Int32
            epBody.Instructions.Add(OpCodes.Box.ToInstruction(mod.CorLibTypes.Int32));
            //: stelem.ref
            epBody.Instructions.Add(OpCodes.Stelem_Ref.ToInstruction());
            //: dup
            epBody.Instructions.Add(OpCodes.Dup.ToInstruction());
            //: ldc.i4.1
            epBody.Instructions.Add(OpCodes.Ldc_I4_1.ToInstruction());
            //: ldstr     "+"
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("+"));
            //: stelem.ref
            epBody.Instructions.Add(OpCodes.Stelem_Ref.ToInstruction());
            //: dup
            epBody.Instructions.Add(OpCodes.Dup.ToInstruction());
            //: ldc.i4.2
            epBody.Instructions.Add(OpCodes.Ldc_I4_2.ToInstruction());
            //: ldloc.2
            epBody.Instructions.Add(OpCodes.Ldloc_2.ToInstruction());
            //: box       [mscorlib]System.Int32
            epBody.Instructions.Add(OpCodes.Box.ToInstruction(mod.CorLibTypes.Int32));
            //: stelem.ref
            epBody.Instructions.Add(OpCodes.Stelem_Ref.ToInstruction());
            //: dup
            epBody.Instructions.Add(OpCodes.Dup.ToInstruction());
            //: ldc.i4.3
            epBody.Instructions.Add(OpCodes.Ldc_I4_3.ToInstruction());
            //: ldstr     "="
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("="));
            //: stelem.ref
            epBody.Instructions.Add(OpCodes.Stelem_Ref.ToInstruction());
            //: dup
            epBody.Instructions.Add(OpCodes.Dup.ToInstruction());
            //: ldc.i4.4
            epBody.Instructions.Add(OpCodes.Ldc_I4_4.ToInstruction());
            //: ldloc.1
            epBody.Instructions.Add(OpCodes.Ldloc_1.ToInstruction());
            //: ldloc.2
            epBody.Instructions.Add(OpCodes.Ldloc_2.ToInstruction());
            //: add
            epBody.Instructions.Add(OpCodes.Add.ToInstruction());
            //: box       [mscorlib]System.Int32
            epBody.Instructions.Add(OpCodes.Box.ToInstruction(mod.CorLibTypes.Int32));
            //: stelem.ref
            epBody.Instructions.Add(OpCodes.Stelem_Ref.ToInstruction());
            //: call      string [mscorlib]System.String::Concat(object[])
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefSystemStringConcat));
            //: call      void [mscorlib]System.Console::WriteLine(string)
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            //: nop
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            //: ldstr     "xyzs"
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("xyzs"));
            //: call      void [mscorlib]System.Console::WriteLine(string)
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            //: nop
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            //: call      string [mscorlib]System.Console::ReadLine()
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(memberRefReadLine));
            //: stloc.3
            epBody.Instructions.Add(OpCodes.Stloc_3.ToInstruction());
            //: ldloc.3
            epBody.Instructions.Add(OpCodes.Ldloc_3.ToInstruction());
            //: call      void [mscorlib]System.Console::WriteLine(string)
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            //: nop
            epBody.Instructions.Add(OpCodes.Nop.ToInstruction());
            //: ret  
            epBody.Instructions.Add(OpCodes.Ret.ToInstruction());

            // 将程序集保存到磁盘上的文件中
            mod.Write(@"saved-assembly31.exe");
        }
    }
}
