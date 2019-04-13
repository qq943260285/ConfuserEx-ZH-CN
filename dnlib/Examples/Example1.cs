using System;
using dnlib.DotNet;

namespace dnlib.Examples {
    // 此示例将打开mscorlib.dll，然后打印出程序集中的所有类型，
    // 包括每种类型具有的方法，字段，属性和事件的数量。
    public class Example1 {
		public static void Run() {
            // 加载mscorlib.dll
            string filename = typeof(void).Module.FullyQualifiedName;
			ModuleDefMD mod = ModuleDefMD.Load(filename);

			int totalNumTypes = 0;
            // mod.Types仅返回非嵌套类型。
            //mod.GetTypes（）返回所有类型，包括嵌套类型。
            foreach (TypeDef type in mod.GetTypes()) {
				totalNumTypes++;
				Console.WriteLine();
				Console.WriteLine("类型: {0}", type.FullName);
				if (type.BaseType != null)
					Console.WriteLine("  基础类型: {0}", type.BaseType.FullName);

				Console.WriteLine("  方法: {0}", type.Methods.Count);
				Console.WriteLine("  字段: {0}", type.Fields.Count);
				Console.WriteLine("  属性: {0}", type.Properties.Count);
				Console.WriteLine("  事件: {0}", type.Events.Count);
				Console.WriteLine("  嵌套类型: {0}", type.NestedTypes.Count);

				if (type.Interfaces.Count > 0) {
					Console.WriteLine("  接口:");
					foreach (InterfaceImpl iface in type.Interfaces)
						Console.WriteLine("    {0}", iface.Interface.FullName);
				}
			}
			Console.WriteLine();
			Console.WriteLine("类型总数： {0}", totalNumTypes);
            Console.ReadKey();
		}
	}
}
