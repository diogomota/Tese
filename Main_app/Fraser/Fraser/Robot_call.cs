﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotOM;

namespace Fraser
{
    static class Robot_call
    {
        static IRobotApplication robApp;
        static int instances = 0;
        static Robot_call()
        {
            if (robApp == null)
            {
                robApp = new RobotApplication();
                robApp.Project.New(IRobotProjectType.I_PT_FRAME_3D);
                if (robApp.Visible == 0) { robApp.Interactive = 1; robApp.Visible = 1; }
                instances = 1;
            }
        }

        public static void update_pts(Genome geometry)
        {
           
            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                robApp.Project.Structure.Nodes.Create((int)geometry.pt_cloud[0, i] + 1, geometry.pt_cloud[1, i], geometry.pt_cloud[2, i], geometry.pt_cloud[3, i]);
            }//+1 porque robot nao aceita barra 0 e pt 0
        }
        public static void update_bars(Genome geometry)
        {
            for (int i = 0; i < robApp.Project.Structure.Bars.FreeNumber; i++)
            {
                robApp.Project.Structure.Bars.Delete(i);
            }

            for (int i = 0; i < Genome.bar_cnt; i++)
            {
                robApp.Project.Structure.Bars.Create((int)geometry.bars[0, i] + 1, (int)geometry.bars[1, i] + 1, (int)geometry.bars[2, i] + 1);
            }
        }
        public static void Refresh()
        {
            robApp.Project.ViewMngr.Refresh();
        }
        /*static double[,] Run_analysis()
        {
            return;
        }*/
    }
}
