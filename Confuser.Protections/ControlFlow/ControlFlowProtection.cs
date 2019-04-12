using System;
using Confuser.Core;
using Confuser.Protections.ControlFlow;
using dnlib.DotNet;

namespace Confuser.Protections {
	public interface IControlFlowService {
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}

	internal class ControlFlowProtection : Protection, IControlFlowService {
		public const string _Id = "ctrl flow";
		public const string _FullId = "Ki.ControlFlow";
		public const string _ServiceId = "Ki.ControlFlow";

		public override string Name {
			get { return "控制流保护"; }
		}

		public override string Description {
			get { return "这种保护破坏了方法中的代码，使反编译器无法反编译方法。"; }
		}

		public override string Id {
			get { return _Id; }
		}

		public override string FullId {
			get { return _FullId; }
		}

		public override ProtectionPreset Preset {
			get { return ProtectionPreset.正常; }
		}

		public void ExcludeMethod(ConfuserContext context, MethodDef method) {
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		protected override void Initialize(ConfuserContext context) {
			context.Registry.RegisterService(_ServiceId, typeof(IControlFlowService), this);
		}

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}
	}
}