using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace AffinityChanger
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}