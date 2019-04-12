using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

/*

此示例显示如何从头创建程序集并通过调用其构造函数创建两个实例。
一个默认构造函数，另一个使用两个参数。


ILSpy创建文件的输出：

using System;
namespace Ctor.Test
{
   internal class BaseClass
   {
       public BaseClass()
       {
           Console.WriteLine("BaseClass: Default .ctor called", null);
       }
   }
   internal class Main : BaseClass
   {
       public static void Main()
       {
           new Main();
           new Main(12345, null);
       }
       public Main()
       {
           Console.WriteLine("Default .ctor called", null);
       }
       public Main(int count, string name)
       {
           Console.WriteLine(".ctor(Int32) called with arg {0}", count);
       }
   }
}

peverify output:

	C:\>peverify ctor-test.exe /IL /MD

	Microsoft (R) .NET Framework PE Verifier.  Version  4.0.30319.1
	Copyright (c) Microsoft Corporation.  All rights reserved.

	All Classes and Methods in ctor-test.exe Verified.


Output of program:

	C:\>ctor-test.exe
	BaseClass: Default .ctor called
	Default .ctor called
	BaseClass: Default .ctor called
	.ctor(Int32) called with arg 12345

*/

namespace dnlib.Examples {
	public class Example4 {
		public static void Run() {
            // 这是将要创建的文件
            string newFileName = @"C:\ctor-test.exe";

            // 创建模块
            var mod = new ModuleDefUser("ctor-test", Guid.NewGuid(),
				new AssemblyRefUser(new AssemblyNameInfo(typeof(int).Assembly.GetName().FullName)));
            // 这是一个控制台应用程序
            mod.Kind = ModuleKind.Console;
            // 创建程序集并将创建的模块添加到它
            new AssemblyDefUser("ctor-test", new Version(1, 2, 3, 4)).Modules.Add(mod);

            // 创建System.Console类型引用
            var systemConsole = mod.CorLibTypes.GetTypeRef("System", "Console");
            // 创建'void System.Console.WriteLine（string，object）'方法引用
            var writeLine2 = new MemberRefUser(mod, "WriteLine",
							MethodSig.CreateStatic(mod.CorLibTypes.Void, mod.CorLibTypes.String,
												mod.CorLibTypes.Object),
							systemConsole);
            // 创建System.Object ::.ctor方法引用。这是默认构造函数
            var objectCtor = new MemberRefUser(mod, ".ctor",
							MethodSig.CreateInstance(mod.CorLibTypes.Void),
							mod.CorLibTypes.Object.TypeDefOrRef);

			CilBody body;
            // 创建基类
            var bclass = new TypeDefUser("Ctor.Test", "BaseClass", mod.CorLibTypes.Object.TypeDefOrRef);
            // 将其添加到模块中
            mod.Types.Add(bclass);
            // 创建Ctor.Test.Base类构造函数：Base Class（）
            var bctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void),
							MethodImplAttributes.IL | MethodImplAttributes.Managed,
							MethodAttributes.Public |
							MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            // 将方法添加到BaseClass
            bclass.Methods.Add(bctor);
            // 创建方法体并添加一些指令
            bctor.Body = body = new CilBody();
            // 确保我们调用基类的构造函数
            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
			body.Instructions.Add(OpCodes.Call.ToInstruction(objectCtor));
			body.Instructions.Add(OpCodes.Ldstr.ToInstruction("BaseClass: Default .ctor called"));
			body.Instructions.Add(OpCodes.Ldnull.ToInstruction());
			body.Instructions.Add(OpCodes.Call.ToInstruction(writeLine2));
			body.Instructions.Add(OpCodes.Ret.ToInstruction());

            // 创建从Ctor.Test.BaseClass派生的Ctor.Test.Main类型
            var main = new TypeDefUser("Ctor.Test", "Main", bclass);
            //将其添加到模块中
            mod.Types.Add(main);
            //创建静态'void Main（）'方法
            var entryPoint = new MethodDefUser("Main", MethodSig.CreateStatic(mod.CorLibTypes.Void),
							MethodImplAttributes.IL | MethodImplAttributes.Managed,
							MethodAttributes.Public | MethodAttributes.Static);
			// Set entry point to entryPoint and add it as a Ctor.Test.Main method
			mod.EntryPoint = entryPoint;
			main.Methods.Add(entryPoint);

			// Create first Ctor.Test.Main constructor: Main()
			var ctor0 = new MethodDefUser(".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void),
							MethodImplAttributes.IL | MethodImplAttributes.Managed,
							MethodAttributes.Public |
							MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
			// Add the method to Main
			main.Methods.Add(ctor0);
			// Create method body and add a few instructions
			ctor0.Body = body = new CilBody();
			// Make sure we call the base class' constructor
			body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
			body.Instructions.Add(OpCodes.Call.ToInstruction(bctor));
			body.Instructions.Add(OpCodes.Ldstr.ToInstruction("Default .ctor called"));
			body.Instructions.Add(OpCodes.Ldnull.ToInstruction());
			body.Instructions.Add(OpCodes.Call.ToInstruction(writeLine2));
			body.Instructions.Add(OpCodes.Ret.ToInstruction());

			// Create second Ctor.Test.Main constructor: Main(int,string)
			var ctor1 = new MethodDefUser(".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, mod.CorLibTypes.Int32, mod.CorLibTypes.String),
							MethodImplAttributes.IL | MethodImplAttributes.Managed,
							MethodAttributes.Public |
							MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
			// Add the method to Main
			main.Methods.Add(ctor1);
			// Create names for the arguments. This is optional. Since this is an instance method
			// (it's a constructor), the first arg is the 'this' pointer. The normal arguments
			// begin at index 1.
			ctor1.Parameters[1].CreateParamDef();
			ctor1.Parameters[1].ParamDef.Name = "count";
			ctor1.Parameters[2].CreateParamDef();
			ctor1.Parameters[2].ParamDef.Name = "name";
			// Create method body and add a few instructions
			ctor1.Body = body = new CilBody();
			// Make sure we call the base class' constructor
			body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
			body.Instructions.Add(OpCodes.Call.ToInstruction(bctor));
			body.Instructions.Add(OpCodes.Ldstr.ToInstruction(".ctor(Int32) called with arg {0}"));
			body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
			body.Instructions.Add(OpCodes.Box.ToInstruction(mod.CorLibTypes.Int32));
			body.Instructions.Add(OpCodes.Call.ToInstruction(writeLine2));
			body.Instructions.Add(OpCodes.Ret.ToInstruction());

			// Create the entry point method body and add instructions to allocate a new Main()
			// object and call the two created ctors.
			entryPoint.Body = body = new CilBody();
			body.Instructions.Add(OpCodes.Newobj.ToInstruction(ctor0));
			body.Instructions.Add(OpCodes.Pop.ToInstruction());
			body.Instructions.Add(OpCodes.Ldc_I4.ToInstruction(12345));
			body.Instructions.Add(OpCodes.Ldnull.ToInstruction());
			body.Instructions.Add(OpCodes.Newobj.ToInstruction(ctor1));
			body.Instructions.Add(OpCodes.Pop.ToInstruction());
			body.Instructions.Add(OpCodes.Ret.ToInstruction());

			// Save the assembly
			mod.Write(newFileName);
		}
	}
}
