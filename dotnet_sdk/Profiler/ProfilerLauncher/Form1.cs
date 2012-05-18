/*****************************************************************************
 * ProfilerTest
 * 
 * Copyright (c) 2006 Scott Hackett
 * 
 * This software is provided 'as-is', without any express or implied warranty. 
 * In no event will the author be held liable for any damages arising from the
 * use of this software.
 * 
 * Permission to use, copy, modify, distribute and sell this software for any 
 * purpose is hereby granted without fee, provided that the above copyright 
 * notice appear in all copies and that both that copyright notice and this 
 * permission notice appear in supporting documentation.
 * 
 * Scott Hackett (code@scotthackett.com)
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ProfilerLauncher
{
    public partial class Form1 : Form
    {
        private const string PROFILER_GUID = "{71EDB19D-4F69-4A2C-A2F5-BE783F543A7E}";
        private const string EXECUTABLE_TO_RUN = "TestProfilerApplication.exe";

        //private const string PROFILER_GUID = "{9E2B38F2-7355-4C61-A54F-434B7AC266C0}";
        //// executable to run
        //private const string EXECUTABLE_TO_RUN = "HelloWorld.exe";


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi;

            // make sure the executable exists
            if (File.Exists(EXECUTABLE_TO_RUN) == false)
            {
                MessageBox.Show("The executable '" + EXECUTABLE_TO_RUN + "' does not exist.\nCheck the EXECUTABLE_TO_RUN constant in the program.");
                return;
            }

            // create a process executor
            psi = new ProcessStartInfo(EXECUTABLE_TO_RUN);

            // ----- SET THE CLR ENVIRONMENT VARIABLES -------------------
            
            // set the Cor_Enable_Profiling env var. This indicates to the CLR whether or
            // not we are using the profiler at all.  1 = yes, 0 = no.
            if (psi.EnvironmentVariables.ContainsKey("COR_ENABLE_PROFILING") == true)
                psi.EnvironmentVariables["COR_ENABLE_PROFILING"] = "1";
            else
                psi.EnvironmentVariables.Add("COR_ENABLE_PROFILING", "1");

            // set the COR_PROFILER env var. This indicates to the CLR which COM object to
            // instantiate for profiling.
            if (psi.EnvironmentVariables.ContainsKey("COR_PROFILER") == true)
                psi.EnvironmentVariables["COR_PROFILER"] = PROFILER_GUID;
            else
                psi.EnvironmentVariables.Add("COR_PROFILER", PROFILER_GUID);

            //if (psi.EnvironmentVariables.ContainsKey("MONITIS_ASSEMBLY") == true)
            //    psi.EnvironmentVariables["MONITIS_ASSEMBLY"] = "TestProfilerApplication";
            //else
            //    psi.EnvironmentVariables.Add("MONITIS_ASSEMBLY", "TestProfilerApplication");

            //if (psi.EnvironmentVariables.ContainsKey("MONITIS_CONFIG") == true)
            //    psi.EnvironmentVariables["MONITIS_CONFIG"] = "MonitisConfig.xml";
            //else
            //    psi.EnvironmentVariables.Add("MONITIS_CONFIG", "MonitisConfig.xml");

            psi.UseShellExecute = false;
            Process p = Process.Start(psi);
        }

    }
}