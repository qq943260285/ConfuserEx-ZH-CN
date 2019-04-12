using System;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace dnlib.Examples {
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
    public class Example3 {
		public static void Run() {
            // 创建一个新模块。 传入的字符串是模块的名称，而不是文件名。
            ModuleDef mod = new ModuleDefUser("MyModule.exe");
            // 这是一个控制台应用程序
            mod.Kind = ModuleKind.Console;

            //将模块添加到装配中
            AssemblyDef asm = new AssemblyDefUser("MyAssembly", new Version(1, 2, 3, 4), null,"");
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
				MethodSig.CreateStatic(mod.CorLibTypes.Int32, new SZArraySig(mod.CorLibTypes.String)));
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
            // 创建方法ref为'System.ConsoleKeyInfo 
            //System.Console::ReadKey()'
            MemberRef consoleReadKey = new MemberRefUser(mod, "ReadKey",
                        MethodSig.CreateStatic(mod.CorLibTypes.Void),
                        consoleRef);

            // 将CIL方法体添加到入口点方法
            CilBody epBody = new CilBody();
			entryPoint.Body = epBody;
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("小宇专属"));
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            epBody.Instructions.Add(OpCodes.Ldstr.ToInstruction("xiaoyu"));
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleWrite1));
            epBody.Instructions.Add(OpCodes.Call.ToInstruction(consoleReadKey));
            epBody.Instructions.Add(OpCodes.Ldc_I4_0.ToInstruction());
			epBody.Instructions.Add(OpCodes.Ret.ToInstruction());

            // 将程序集保存到磁盘上的文件中
            mod.Write(@"C:\saved-assembly.exe");
		}
	}
}
