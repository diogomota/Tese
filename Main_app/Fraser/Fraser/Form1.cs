using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotOM;

namespace Fraser
{
    public partial class Form1 : Form
    {
        static private Population CurrentPop;
        private Population LastPop;
        private Genome BaseDNA;
        private int _individual = 0;

        public Form1()
        {
            InitializeComponent();
            Robot_call.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void subdiv_int_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void geom_tab_Click(object sender, EventArgs e)
        {

        }

        private void Start_geom_Click(object sender, EventArgs e)
        {
           //robApp = new RobotApplication();
           // if (robApp.Visible == 0) { robApp.Interactive = 1;robApp.Visible = 1; }
            
            //create []hcabos
            int[] h_cabos = new int[3] {(int)h_cabo1_int.Value, (int)h_cabo2_int.Value, (int)h_cabo3_int.Value };
            //create []dist_centro
            double[] dist_centro = new double[3] { (double)w_cabo1_int.Value, (double)w_cabo2_int.Value, (double)w_cabo3_int.Value };

            if( BaseDNA != null || CurrentPop != null) { BaseDNA = null; CurrentPop = null; }
            BaseDNA = new Fraser.Genome((double)Largura_ap_int.Value,(int)Altura_int.Value,(double)h_div_int.Value,(double)subdiv_int.Value,(int)n_cabos_int.Value,h_cabos,dist_centro);

            Robot_call.Start_pts(BaseDNA);
            Robot_call.Start_bars(BaseDNA);
            Robot_call.Refresh();


            CurrentPop = new Population((int)Population_cnt.Value, BaseDNA);

            
            
        }

        private void draw_Click(object sender, EventArgs e)
        {
            Robot_call.Robot_interactive(false);

            Robot_call.Get_sections();
            Robot_call.Update_pts(CurrentPop.ind[_individual]._DNA);


                Robot_call.Update_bars(CurrentPop.ind[_individual]._DNA);

            Robot_call.Addsupports();
            CurrentPop.ind[_individual].results=Robot_call.Run_analysis();
            Robot_call.Refresh();
            Robot_call.Robot_interactive(true);
            CurrentPop.ind[_individual].get_ton();
           // Console.Write(CurrentPop.ind[_individual].results[1, 5]);
            _individual++;

        }

        private void btn_get_sec_Click(object sender, EventArgs e)
        {
            Robot_call.Get_sections();
            // add to the matrix the area of each section in the get_sections
            Sec_list.DataSource = Robot_call.sec_prop.section_names;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
