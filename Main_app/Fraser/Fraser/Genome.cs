using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    class Genome
    {
        //vars
        public double[,] pt_cloud;
        //[numero,x,y,z]
        public double[,] arm_cld;
        //[numero,x,y,z]
        public double[,] bars;
        //[numero,pt1,pt2,secçao,activo]
        public int pt_cnt;

        public int bar_cnt;
        //methods
        //constructor
        public Genome(double Largura,int Altura, double horiz_div,double subdiv, int N_cabos, int[] h_cabos,double[] dist_centro)
        {
            // init matrix dim
            pt_cloud = new double[4, (int)(17*N_cabos + 4 + (4 * subdiv) * (horiz_div - 1))]; // 17*cabos para os pts dos braços
            arm_cld = new double[4, 17 * N_cabos];
            bars = new double[4, (int)((4 * horiz_div - 8) * (subdiv * subdiv) + (12 * horiz_div - 12) * subdiv - 8 * horiz_div + 36 * N_cabos + 20)];
            //bars = new double[...]

            pt_add_tower(ref pt_cloud, Largura, Altura,horiz_div,subdiv,ref pt_cnt);
            pt_add_arms(ref pt_cloud, Largura, Altura, horiz_div, subdiv, N_cabos, h_cabos, dist_centro, ref pt_cnt);
           bar_cnt= connect_bars(ref pt_cloud, ref bars,(int)subdiv,(int)horiz_div);
            //
            
        }

        private void pt_add_tower(ref double[,] pt, double Largura, int Altura, double horiz_div, double subdiv,ref int _pt_cnt)
        {

            //tower pts
            int x = 0;
            int y = 1;
            int h = 1;
            bool reverse = false;
            int pt_num = 4; //numero do 1º pt criado em loop for

            //#######################//
            //Tilt calc + N of rings //
            //#######################//
            double ring_z_step = (Altura) / horiz_div; // ok

            double tilt;
            tilt = (Largura * 0.5) / (Altura) * ring_z_step; //ok

            

            // ADD support pts
            addPt(ref pt, 0, 0, 0, 0);
            addPt(ref pt, 1, Largura, 0, 0);
            addPt(ref pt, 2, Largura, Largura, 0);
            addPt(ref pt, 3, 0, Largura, 0);


            //##################//
            //main pt cloud loop//
            //##################//

            for (h = 1; h <= horiz_div - 1; h++)
            { // ring loop

                double scale_factor = (1 - (h / horiz_div));
                double step = h * tilt;

                if (!reverse)
                { // x++ y++

                    for (x = 0; x <= subdiv; x++)
                    {

                        addPt(ref pt, pt_num, step + x * (Largura / subdiv) * scale_factor, step, h * ring_z_step);
                        pt_num++;

                        if (x == subdiv)
                        {

                            for (y = 1; y <= subdiv; y++)
                            {
                                addPt(ref pt, pt_num, Largura - step, step + y * (Largura / subdiv) * scale_factor, h * ring_z_step);
                                pt_num++;

                                if (x == subdiv && y == subdiv) { reverse = true; } // start backwards loop
                            }
                        }
                    }

                }

                if (reverse)
                { // x-- y--
                    for (x = (int)subdiv - 1; x >= 0; x--)
                    {
                        addPt(ref pt, pt_num, step + x * (Largura / subdiv) * scale_factor, Largura - step, h * ring_z_step);
                        pt_num++;

                        if (x == 0)
                        {
                            for (y = (int)subdiv - 1; y >= 1; y--)
                            {
                                addPt(ref pt, pt_num, step, step + y * (Largura / subdiv) * scale_factor, h * ring_z_step);
                                pt_num++;


                            }
                        }
                    }
                }

                reverse = false;
            }
            _pt_cnt = pt_num; // para começar a numerar correctamente os pts dos braços

        }
        
        private void pt_add_arms(ref double[,] pt, double Largura, int Altura, double horiz_div, double subdiv, int N_cabos, int[] h_cabos, double[] dist_centro, ref int _pt_cnt) {

            double ring_z_step = (Altura) / horiz_div; // ok

            double tilt;
            tilt = (Largura * 0.5) / (Altura) * ring_z_step; //ok

            ///////////////////////////
            //#######################//
            //######## Arms #########//
            //#######################//
            ///////////////////////////

            List<Int32> con_ring_set = new List<Int32>();

            for (int n = 0; n < (N_cabos / 2); n++)
            {   //for each set of cables

                int init_h = find_nearest(h_cabos[n], horiz_div, ring_z_step); //encontrar os horiz ring + perto
                con_ring_set.Add(init_h);
                double arm_lenght = Math.Abs(dist_centro[n] - (Largura * 0.5 - init_h * tilt));

                //lower arm angle
                double XY_m = (Largura * 0.5 - init_h * tilt) / arm_lenght; //inclinação da reta Y=mx em XY
                double XZ_m = (h_cabos[n] - init_h * ring_z_step) / arm_lenght; //inclinação da reta Z=mx em XZ

                //upper arm angle
                double XY_m_upp = (Largura * 0.5 - (init_h + 1) * tilt) / arm_lenght;
                double XZ_m_upp = (h_cabos[n] - (init_h + 1) * ring_z_step) / arm_lenght;


                //############//
                //#Right arm#//
                //###########//

                //1st lower arm
                for (int _subdiv = 1; _subdiv <= 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço

                    double _x = (Largura - init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    if (_subdiv == 5)
                    {
                        addPt(ref pt, pt_cnt, _x, Largura * 0.5, h_cabos[n]);
                        pt_cnt++;
                        break;
                    }
                    addPt(ref pt, pt_cnt, _x, init_h * tilt + XY_m * (_x - (Largura - init_h * tilt)), init_h * ring_z_step + XZ_m * (_x - (Largura - init_h * tilt))); // 1st lower arm
                    pt_cnt++;
                }

                //2nd lower arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (Largura - init_h * tilt) + (arm_lenght / 5) * _subdiv;
                    addPt(ref pt, pt_cnt, _x, Largura - init_h * tilt - XY_m * (_x - (Largura - init_h * tilt)), init_h * ring_z_step + XZ_m * (_x - (Largura - init_h * tilt)));
                    pt_cnt++;
                }

                //1st upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço
                    double _x = (Largura - init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    addPt(ref pt, pt_cnt, _x, (init_h + 1) * tilt + XY_m_upp * (_x - (Largura - init_h * tilt)), (init_h + 1) * ring_z_step + XZ_m_upp * (_x - (Largura - init_h * tilt)));
                    pt_cnt++;
                }
                //2nd upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (Largura - init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    addPt(ref pt, pt_cnt, _x, Largura - (init_h + 1) * tilt - XY_m_upp * (_x - (Largura - init_h * tilt)), (init_h + 1) * ring_z_step + XZ_m_upp * (_x - (Largura - init_h * tilt)));
                    pt_cnt++;
                }

                //############//
                //##Left arm##//
                //############//

                //left
                //1st lower arm
                for (int _subdiv = 1; _subdiv <= 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço

                    double _x = (init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    if (_subdiv == 5)
                    {
                        addPt(ref pt, pt_cnt, _x, Largura * 0.5, h_cabos[n]);
                        pt_cnt++;
                        break;
                    }

                    addPt(ref pt, pt_cnt, _x, init_h * tilt - XY_m * (_x - init_h * tilt), init_h * ring_z_step - XZ_m * (_x - init_h * tilt));//1st lower arm
                    pt_cnt++;
                }

                //2nd lower arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addPt(ref pt, pt_cnt, _x, Largura - init_h * tilt + XY_m * (_x - init_h * tilt), init_h * ring_z_step - XZ_m * (_x - init_h * tilt));
                    pt_cnt++;
                }

                //1st upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço
                    double _x = (init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addPt(ref pt, pt_cnt, _x, (init_h + 1) * tilt - XY_m_upp * (_x - init_h * tilt), (init_h + 1) * ring_z_step - XZ_m_upp * (_x - init_h * tilt));
                    pt_cnt++;
                }
                //2nd upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addPt(ref pt, pt_cnt, _x, Largura - (init_h + 1) * tilt + XY_m_upp * (_x - init_h * tilt), (init_h + 1) * ring_z_step - XZ_m_upp * (_x - init_h * tilt));
                    pt_cnt++;
                }
            }
        }

        private int connect_bars(ref double[,] pt, ref double[,] bars,int subdiv, int horiz_div)
        {
            int bar_num = 0;
            //Support pts
            for (int i = 0; i <= 4; i++)
            {

                if (i == 0)
                {
                    for (int j = 4; j <= 4 + subdiv; j++)
                    {
                        addBar(ref bars, bar_num, 0, j);
                        bar_num++;
                    }
                    for (int j = 8 + 4 * (subdiv - 1) - 1; j >= 8 + 4 * (subdiv - 1) - subdiv; j--)
                    {
                        addBar(ref bars, bar_num, 0, j);
                        bar_num++;
                    }

                }
                if (i == 1)
                {
                    for (int j = 4; j <= 4 + 2 * subdiv; j++)
                    {
                        addBar(ref bars, bar_num, 1, j);
                        bar_num++;
                    }
                }
                if (i == 2)
                {
                    for (int j = 4 + subdiv; j <= 4 + 3 * subdiv; j++)
                    {
                        addBar(ref bars, bar_num, 2, j);
                        bar_num++;
                    }
                }
                if (i == 3)
                {
                    for (int j = 4 + subdiv * 2; j <= 4 + 4 * subdiv - 1; j++)
                    {
                        addBar(ref bars, bar_num, 3, j);
                        bar_num++;
                    }
                    addBar(ref bars, bar_num, 3, 4);
                    bar_num++;
                }
            }

            //###############//
            //Main tower bars//
            //###############//

            //add Lcr nesta fase !!!
            int ring_pt = 4 + (subdiv - 1) * 4;
            //init and end pts
            for (int h = 0; h <= horiz_div - 3; h++)
            {

                // Lado +XX
                for (int i = 4 + ring_pt * h; i < 4 + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados
                    if (i == 4 + ring_pt * h)
                    {//cantos
                        for (int j = 4 + ring_pt * (h + 1); j <= subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        { //-1 para nao conectar a diagonal oposta
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;
                        }
                    }
                    else if (i == 4 + ring_pt * h + subdiv)
                    {
                        for (int j = 4 + ring_pt * (h + 1) + 1; j <= subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        { //+1 para nao conectar a diagonal oposta
                            addBar(ref bars, bar_num, i, j); //quando i = j-ring_pt  id bar as active(always)
                            bar_num++; 
                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + ring_pt * h && i < 4 + ring_pt * h + subdiv)
                        {
                            for (int j = 4 + ring_pt * (h + 1); j <= subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j);
                                bar_num++;
                            }
                        }
                    }
                }
                // Lado +YY
                for (int i = 4 + subdiv + ring_pt * h; i < 4 + subdiv + ring_pt * (h + 1); i++)
                {//init coord.

                    //lados
                    if (i == 4 + subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + subdiv + ring_pt * (h + 1); j <= 2 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;
                        }
                    }
                    else if (i == 4 + ring_pt * h + 2 * subdiv)
                    {
                        for (int j = 4 + subdiv + ring_pt * (h + 1) + 1; j <= 2 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;
                        }

                    }
                    else
                    { //pts centrais
                        if (i > 4 + subdiv + ring_pt * h && i < 4 + ring_pt * h + 2 * subdiv)
                        {
                            for (int j = 4 + subdiv + ring_pt * (h + 1); j <= 2 * subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j);
                                bar_num++;
                            }
                        }
                    }
                }

                // Lado -XX
                for (int i = 4 + 2 * subdiv + ring_pt * h; i < 4 + 2 * subdiv + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados
                    if (i == 4 + 2 * subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + 2 * subdiv + ring_pt * (h + 1); j <= 3 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;
                        }
                    }
                    else if (i == 4 + ring_pt * h + 3 * subdiv)
                    {
                        for (int j = 4 + 2 * subdiv + ring_pt * (h + 1) + 1; j <= 3 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;
                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + 2 * subdiv + ring_pt * h && i < 4 + ring_pt * h + 3 * subdiv)
                        {
                            for (int j = 4 + 2 * subdiv + ring_pt * (h + 1); j <= 3 * subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j);
                                bar_num++;
                            }
                        }
                    }
                }

                // Lado -YY
                for (int i = 4 + 3 * subdiv + ring_pt * h; i < 4 + 3 * subdiv + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados
                    if (i == 4 + 3 * subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + 3 * subdiv + ring_pt * (h + 1); j <= 4 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j);
                            bar_num++;

                            if (j != 4 + 3 * subdiv + ring_pt * (h + 1))
                            { //exceto lado oposto
                                addBar(ref bars, bar_num, 4+ring_pt*h, j);
                                bar_num++;
                            } //conect first corner w/ side -YY

                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + 3 * subdiv + ring_pt * h && i < 4 + ring_pt * h + 4 * subdiv)
                        {
                            for (int j = 4 + 3 * subdiv + ring_pt * (h + 1); j <= 4 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                            {
                                addBar(ref bars, bar_num, i, j);
                                bar_num++;
                            }
                            addBar(ref bars, bar_num, i, 4+ring_pt*(h+1)); //conect to init pt
                            bar_num++;
                        }
                    }
                }

            }

            // Horizontal connections
            //add Lcr nesta fase!!!
            for (int h = 0; h <= horiz_div - 2; h++)
            {
                for (int i = 4 + h * ring_pt; i < 4 + (h + 1) * ring_pt - 1; i++)
                {
                    addBar(ref bars, bar_num, i, i+1);
                    bar_num++;
                    if (i == 4 + (h + 1) * ring_pt - 2)
                    {
                        addBar(ref bars, bar_num, i+1, 4+h*ring_pt);
                        bar_num++;
                    }
                }
            }
            return bar_num;
        }
        private int find_nearest(int altura, double horiz_div, double ring_z_step)
        {
            int nearest = -1;
            double dist = 10000;

            for (int h = 1; h <= horiz_div - 2; h++)
            {
                if (dist > Math.Abs(altura - h * ring_z_step))
                {
                    dist = Math.Abs(altura - h * ring_z_step);
                    nearest = h;
                }
            }

            return nearest;
        }
        //for fast pt add
        private void addPt (ref double[,] matrix ,int pt_number, double x, double y, double z){

                matrix[0, pt_number] = pt_number;
                matrix[1, pt_number] = x;
                matrix[2, pt_number] = y;
                matrix[3, pt_number] = z;


        }
        private void addBar(ref double[,] bar, int numb,int start,int end)
        {
            bar[0, numb] = numb;
            bar[1, numb] = start;
            bar[2, numb] = end;
            //add here more options as needed (active,section,Lcr etc etc)
            bar[3, numb] = 0;
        }
    }

    
}
