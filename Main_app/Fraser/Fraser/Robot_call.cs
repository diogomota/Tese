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

        public static Sections sec_prop;

        static IRobotBarForceServer results;

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
        public static void Robot_interactive(bool a)
        {
            if (a) {
                robApp.Interactive = 1;
            } else {
                robApp.Interactive = 0;
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
        public static void Start_pts(Genome geometry)
        {
            if (robApp.Project.Structure.Nodes.GetAll().Count != 0) // delete any existing pts
            {
                for (int i = robApp.Project.Structure.Nodes.GetAll().Count; i > 0; i--)
                {
                    robApp.Project.Structure.Nodes.Delete(i);
                }
            }

            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                robApp.Project.Structure.Nodes.Create((int)geometry.pt_cloud[0, i] + 1, geometry.pt_cloud[1, i], geometry.pt_cloud[2, i], geometry.pt_cloud[3, i]);
            }//+1 porque robot nao aceita pt 0

        }

        public static void Start_bars(Genome geometry)
        {
            for (int i = 0; i < robApp.Project.Structure.Bars.FreeNumber; i++)
            {
                robApp.Project.Structure.Bars.Delete(i);
            }
            for (int i = 0; i < Genome.bar_cnt; i++)
            {
                robApp.Project.Structure.Bars.Create((int)geometry.bars[0, i] + 1, (int)geometry.bars[1, i] + 1, (int)geometry.bars[2, i] + 1);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, sec_prop.section_names[0]);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_MATERIAL, "AÇO");
            }
        }

        public static void Update_pts(Genome geometry)
        {
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_SUPPORT).Get(2).ToString());
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(2).ToString());

            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                robApp.Project.Structure.Nodes.Create((int)geometry.pt_cloud[0, i] + 1, geometry.pt_cloud[1, i], geometry.pt_cloud[2, i], geometry.pt_cloud[3, i]);
            }//+1 porque robot nao aceita barra 0 e pt 0

        }
        public static void Update_bars(Genome geometry)
        {
            for (int i = 0; i < Genome.towerBar_cnt; i++)
            {
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, sec_prop.section_names[0]);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_MATERIAL, "AÇO");
            }
            // robApp.Project.Structure.Bars.SetInactive("3"); funciona
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
            List<String> _section_names = new List<String>();
            List<double> _Area = new List<double>();
            List<double> _Ix = new List<double>();
            List<double> _Iy = new List<double>();

            for (int i = 1; i <= robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Count; i++)
            {
                
                _section_names.Add(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(i).ToString());

                // API needs to copy labels from robot at runtime for the properties to be accessible without being assigned to any bar
                RobotLabel label = robApp.Project.Structure.Labels.Get(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i-1]) as RobotOM.RobotLabel;
                bool available = robApp.Project.Structure.Labels.IsAvailable(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i-1]);

                if (label == null && available)
                {
                    label = robApp.Project.Structure.Labels.CreateLike(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i - 1], _section_names[i - 1]) as RobotOM.RobotLabel;
                }
                
                IRobotBarSectionData dt = (IRobotBarSectionData)robApp.Project.Structure.Labels.Get(IRobotLabelType.I_LT_BAR_SECTION,_section_names[i-1]).Data;
                Console.Write(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_AX));

                _Area.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_AX));
                _Ix.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_IX));
                _Iy.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_IY));
            }
            sec_prop = new Sections(_section_names, _Area, _Ix, _Iy);
        }

        public static double[,] Run_analysis()
        {
            robApp.Project.CalcEngine.Calculate();
            results = robApp.Project.Structure.Results.Bars.Forces;
            double[,] a = new double[6, Genome.towerBar_cnt];
            for (int i = 0; i < Genome.towerBar_cnt; i++)
            {
                IRobotBar current_bar = (IRobotBar)robApp.Project.Structure.Bars.Get(i + 1);
                a[0, i] = i + 1;
                a[1, i] = current_bar.Length;
                a[2, i] = Max(results.Value(i + 1, 1, 0).FX, results.Value(i + 1, 1, 0.5).FX, results.Value(i + 1, 1, 1).FX);
                a[3, i] = Max(results.Value(i + 1, 1, 0).MY, results.Value(i + 1, 1, 0.5).MY, results.Value(i + 1, 1, 1).MY);
                a[4, i] = Max(results.Value(i + 1, 1, 0).MZ, results.Value(i + 1, 1, 0.5).MZ, results.Value(i + 1, 1, 1).MZ);
                a[5, i] = 1; //decide how to efficiently select the worst V (Vy ou Vz)
            }

            return a;
        }
        private static double Max(double start, double middle, double end){

            if (Math.Abs(start) >= Math.Abs(middle) && Math.Abs(start) >= Math.Abs(end)) { return start; }
            if (Math.Abs(middle) >= Math.Abs(start) && Math.Abs(middle) >= Math.Abs(end)) { return middle; }
            if (Math.Abs(end) >= Math.Abs(start) && Math.Abs(end) >= Math.Abs(middle)) { return end; }
            return -1;
       }
    }
}
