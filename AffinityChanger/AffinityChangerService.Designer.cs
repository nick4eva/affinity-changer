﻿namespace AffinityChanger
{
	partial class AffinityChangerService
	{
		private System.Timers.Timer timer;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.timer = new System.Timers.Timer();
			//
			// timer
			//
			this.timer.Enabled = true;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerElapsed);
			// 
			// AffinityChanger
			// 
			this.ServiceName = "AffinityChanger";
		}
		#endregion
	}
}
