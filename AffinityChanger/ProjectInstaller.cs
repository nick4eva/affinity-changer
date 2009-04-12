namespace AffinityChanger
{
	using System;
	using System.ComponentModel;
	using System.Configuration.Install;

	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		/// <summary>
		/// 
		/// </summary>
		public ProjectInstaller()
		{
			this.InitializeComponent();
		}
	}
}