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


        //methods
        //constructor
        public Genome(double Largura,int Altura, double horiz_div,double subdiv, int N_cabos)
        {
            // init matrix dim
            pt_cloud = new double[4, (int)(4 + (4 * subdiv) * (horiz_div - 1))];
            arm_cld = new double[4, 17 * N_cabos];
            //bars = new double[...]
            pt_cloud_add(ref pt_cloud, Largura, Altura,horiz_div,subdiv);
            //
            
        }
        private void pt_cloud_add(ref double[,] pt, double Largura, int Altura, double horiz_div, double subdiv)
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


        }
        //for fast pt add
        private void addPt (ref double[,] matrix ,int pt_number, double x, double y, double z){

                matrix[0, pt_number] = pt_number;
                matrix[1, pt_number] = x;
                matrix[2, pt_number] = y;
                matrix[3, pt_number] = z;


        }

    }

    
}
