using System;
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

        static List<String> section_names = new List<String>();

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
        public static void Start()
        {
            // define section Db
            robApp.Project.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "DIN");
            //define materials Db
            robApp.Project.Preferences.Materials.Load("Eurocode");
            //set default material S235
            robApp.Project.Preferences.Materials.SetDefault(IRobotMaterialType.I_MT_STEEL, "S 235");
        }
        public static void update_pts(Genome geometry)
        {
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_SUPPORT).Get(2).ToString());
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(2).ToString());
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
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, section_names[0]);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0,i]+1).SetLabel(IRobotLabelType.I_LT_MATERIAL, "AÇO");
            }
        }
        public static void Refresh()
        {
            robApp.Project.ViewMngr.Refresh();
        }
        public static void Addsupports()
        {
            
            for (int i = 1; i <= 4; i++)
            {
                robApp.Project.Structure.Nodes.Get(i).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Fixed");
            }

        }
        public static void Get_sections()
        {
           for (int i = 1;i<= robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Count; i++)
            {
                section_names.Add(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(i).ToString());
            }
        }
        public static void Run_analysis()
        {
            robApp.Project.CalcEngine.Calculate();
        }
    }
}
