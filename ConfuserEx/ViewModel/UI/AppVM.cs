using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.ViewModel {
	public class AppVM : ViewModelBase {
		readonly IList<TabViewModel> tabs = new ObservableCollection<TabViewModel>();
		string fileName;
		bool navDisabled;
		bool firstSaved;

		ProjectVM proj;

		public bool NavigationDisabled {
			get { return navDisabled; }
			set { SetProperty(ref navDisabled, value, "NavigationDisabled"); }
		}

		public ProjectVM Project {
			get { return proj; }
			set {
				if (proj != null)
					proj.PropertyChanged -= OnProjectPropertyChanged;

				SetProperty(ref proj, value, "Project");

				if (proj != null)
					proj.PropertyChanged += OnProjectPropertyChanged;
			}
		}

		public string FileName {
			get { return fileName; }
			set {
				SetProperty(ref fileName, value, "Project");
				OnPropertyChanged("Title");
			}
		}

		public string Title {
			get {
				return string.Format("{0}{1} - {2}",
				                     Path.GetFileName(fileName),
				                     (proj.IsModified ? "*" : ""),
				                     ConfuserEngine.Version);
			}
		}

		public IList<TabViewModel> Tabs {
			get { return tabs; }
		}

		public ICommand NewProject {
			get { return new RelayCommand(NewProj, () => !NavigationDisabled); }
		}

		public ICommand OpenProject {
			get { return new RelayCommand(OpenProj, () => !NavigationDisabled); }
		}

		public ICommand SaveProject {
			get { return new RelayCommand(() => SaveProj(), () => !NavigationDisabled); }
		}

		public ICommand Decode {
			get { return new RelayCommand(() => new StackTraceDecoder { Owner = Application.Current.MainWindow }.ShowDialog(), () => !NavigationDisabled); }
		}

		public bool OnWindowClosing() {
			return PromptSave();
		}

		bool SaveProj() {
			if (!firstSaved || !File.Exists(FileName)) {
				var sfd = new VistaSaveFileDialog();
				sfd.FileName = FileName;
				sfd.Filter = "ConfuserEx 项目 (*.crproj)|*.crproj|所有文件 (*.*)|*.*";
				sfd.DefaultExt = ".crproj";
				sfd.AddExtension = true;
				if (!(sfd.ShowDialog(Application.Current.MainWindow) ?? false) || sfd.FileName == null)
					return false;
				FileName = sfd.FileName;
			}
			ConfuserProject proj = ((IViewModel<ConfuserProject>)Project).Model;
			proj.Save().Save(FileName);
			Project.IsModified = false;
			firstSaved = true;
			return true;
		}

		bool PromptSave() {
			if (!Project.IsModified)
				return true;
			switch (MessageBox.Show("当前项目有未保存的更改。你想保存它们吗？", "ConfuserEx", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)) {
				case MessageBoxResult.Yes:
					return SaveProj();
				case MessageBoxResult.No:
					return true;
				case MessageBoxResult.Cancel:
					return false;
			}
			return false;
		}

		void NewProj() {
			if (!PromptSave())
				return;

			Project = new ProjectVM(new ConfuserProject(), null);
			FileName = "未命名.crproj";
		}

		void OpenProj() {
			if (!PromptSave())
				return;

			var ofd = new VistaOpenFileDialog();
			ofd.Filter = "ConfuserEx 项目 (*.crproj)|*.crproj|所有文件 (*.*)|*.*";
			if ((ofd.ShowDialog(Application.Current.MainWindow) ?? false) && ofd.FileName != null) {
				string fileName = ofd.FileName;
				try {
					var xmlDoc = new XmlDocument();
					xmlDoc.Load(fileName);
					var proj = new ConfuserProject();
					proj.Load(xmlDoc);
					Project = new ProjectVM(proj, fileName);
					FileName = fileName;
				}
				catch {
					MessageBox.Show("项目无效！", "ConfuserEx", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "IsModified")
				OnPropertyChanged("Title");
		}

		protected override void OnPropertyChanged(string property) {
			base.OnPropertyChanged(property);
			if (property == "Project")
				LoadPlugins();
		}

		void LoadPlugins() {
			foreach (var plugin in Project.Plugins) {
				try {
					ComponentDiscovery.LoadComponents(Project.Protections, Project.Packers, plugin.Item);
				}
				catch {
					MessageBox.Show("无法加载插件'" + plugin + "'。");
				}
			}
		}
	}
}