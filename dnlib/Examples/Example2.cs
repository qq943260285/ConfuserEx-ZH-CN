using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace dnlib.Examples
{
    public class Example2
    {
        // 这将打开当前程序集，向其添加新类和方法，然后将程序集保存到磁盘。
        public static void Run()
        {
            // 打开当前模块
            ModuleDefMD mod = ModuleDefMD.Load(typeof(Example2).Module);

            // 创建一个派生自System.Object的新公共类
            TypeDef typeDef = new TypeDefUser("My.Namespace", "MyType", mod.CorLibTypes.Object.TypeDefOrRef);
            typeDef.Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass;
            // 确保将其添加到模块或模块中的任何其他类型。
            // 这不是嵌套类型，因此将其添加到mod.Types。
            mod.Types.Add(typeDef);

            // 创建一个名为MyField的公共静态System.Int32字段
            FieldDef fieldDef = new FieldDefUser("MyField", new FieldSig(mod.CorLibTypes.Int32), FieldAttributes.Public | FieldAttributes.Static);
            // 将其添加到我们之前创建的类型中
            typeDef.Fields.Add(fieldDef);

            // 添加一个静态方法，添加输入和静态字段并返回结果
            MethodImplAttributes methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
            MethodAttributes methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
            MethodDef methodDef = new MethodDefUser(
                "MyMethod",
                MethodSig.CreateStatic(mod.CorLibTypes.Int32, mod.CorLibTypes.Int32, mod.CorLibTypes.Int32),
                methImplFlags,
                methFlags
                );
            typeDef.Methods.Add(methodDef);

            // 创建CIL方法体
            CilBody body = new CilBody();
            methodDef.Body = body;
            // 分别命名第一和第二个参数a和b
            methodDef.ParamDefs.Add(new ParamDefUser("a", 1));
            methodDef.ParamDefs.Add(new ParamDefUser("b", 2));

            // 创建一个本地。我们真的不需要它，但无论如何我们要加一个
            Local local1 = new Local(mod.CorLibTypes.Int32);
            body.Variables.Add(local1);

            // 添加说明，并使用无用的本地
            body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
            body.Instructions.Add(OpCodes.Add.ToInstruction());
            body.Instructions.Add(OpCodes.Ldsfld.ToInstruction(fieldDef));
            body.Instructions.Add(OpCodes.Add.ToInstruction());
            body.Instructions.Add(OpCodes.Stloc.ToInstruction(local1));
            body.Instructions.Add(OpCodes.Ldloc.ToInstruction(local1));
            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            // 将程序集保存到磁盘上的文件中
            mod.Write(@"saved-assembly.dll");
        }
    }
}
