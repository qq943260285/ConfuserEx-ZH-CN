using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Confuser.Core;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Compress {
	internal class StubProtection : Protection {
		readonly CompressorContext ctx;
		readonly ModuleDef originModule;

		internal StubProtection(CompressorContext ctx, ModuleDef originModule) {
			this.ctx = ctx;
			this.originModule = originModule;
		}

		public override string Name {
			get { return "压缩机保护存根"; }
		}

		public override string Description {
			get { return "在受保护的存根上做一些额外的工作。"; }
		}

		public override string Id {
			get { return "Ki.Compressor.Protection"; }
		}

		public override string FullId {
			get { return "Ki.Compressor.Protection"; }
		}

		public override ProtectionPreset Preset {
			get { return ProtectionPreset.无; }
		}

		protected override void Initialize(ConfuserContext context) {
			//
		}

		protected override void PopulatePipeline(ProtectionPipeline pipeline) {
			if (!ctx.CompatMode)
				pipeline.InsertPreStage(PipelineStage.Inspection, new InjPhase(this));
			pipeline.InsertPostStage(PipelineStage.BeginModule, new SigPhase(this));
		}

		class InjPhase : ProtectionPhase {
			public InjPhase(StubProtection parent)
				: base(parent) { }

			public override ProtectionTargets Targets {
				get { return ProtectionTargets.Modules; }
			}

			public override bool ProcessAll {
				get { return true; }
			}

			public override string Name {
				get { return "模块注入"; }
			}

			protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
				// Hack the origin module into the assembly to make sure correct type resolution
				var originModule = ((StubProtection)Parent).originModule;
				originModule.Assembly.Modules.Remove(originModule);
				context.Modules[0].Assembly.Modules.Add(((StubProtection)Parent).originModule);
			}
		}

		class SigPhase : ProtectionPhase {
			public SigPhase(StubProtection parent)
				: base(parent) { }

			public override ProtectionTargets Targets {
				get { return ProtectionTargets.Modules; }
			}

			public override string Name {
				get { return "打包器信息编码"; }
			}

			protected override void Execute(ConfuserContext context, ProtectionParameters parameters) {
				var field = context.CurrentModule.Types[0].FindField("DataField");
				Debug.Assert(field != null);
				context.Registry.GetService<INameService>().SetCanRename(field, true);

				context.CurrentModuleWriterListener.OnWriterEvent += (sender, e) => {
					if (e.WriterEvent == ModuleWriterEvent.MDBeginCreateTables) {
						// Add key signature
						var writer = (ModuleWriterBase)sender;
						var prot = (StubProtection)Parent;
						uint blob = writer.MetaData.BlobHeap.Add(prot.ctx.KeySig);
						uint rid = writer.MetaData.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(blob));
						Debug.Assert((0x11000000 | rid) == prot.ctx.KeyToken);

						if (prot.ctx.CompatMode)
							return;

						// Add File reference
						byte[] hash = SHA1.Create().ComputeHash(prot.ctx.OriginModule);
						uint hashBlob = writer.MetaData.BlobHeap.Add(hash);

						MDTable<RawFileRow> fileTbl = writer.MetaData.TablesHeap.FileTable;
						uint fileRid = fileTbl.Add(new RawFileRow(
							                           (uint)FileAttributes.ContainsMetaData,
							                           writer.MetaData.StringsHeap.Add("koi"),
							                           hashBlob));
					}
				};
			}
		}
	}
}