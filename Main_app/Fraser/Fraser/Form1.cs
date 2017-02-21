using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fraser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            //create []hcabos
            int[] h_cabos = new int[3] {(int)h_cabo1_int.Value, (int)h_cabo2_int.Value, (int)h_cabo3_int.Value };
            //create []dist_centro
            double[] dist_centro = new double[3] { (double)w_cabo1_int.Value, (double)w_cabo2_int.Value, (double)w_cabo3_int.Value };


            Genome geometry = new Fraser.Genome((double)Largura_ap_int.Value,(int)Altura_int.Value,(double)h_div_int.Value,(double)subdiv_int.Value,(int)n_cabos_int.Value,h_cabos,dist_centro);
        }
    }
}
