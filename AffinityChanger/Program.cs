//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="nick4eva's software">
//     Copyright (c) nick4eva's software. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace AffinityChanger
{
    using System.ServiceProcess;

    /// <summary>
    /// Класс программы
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] { new AffinityChangerService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}