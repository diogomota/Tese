using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    public class Individual
    {
        public Genome _DNA;
        public double fitness;
        public double ton;
        public double[,] results;
        static int bb = 0;
        public Individual(Genome _baseDNA,ref Random rndm)
        {
            this.fitness = 0.0;

            _DNA = new Genome(); // create new Genome

            //if the following is not done the same matrix is always changing (need to create copies):

            _DNA.pt_cloud = (double[,])_baseDNA.pt_cloud.Clone(); // copy by value the pt cloud[]
            _DNA.bars = (double[,])_baseDNA.bars.Clone(); //copy by value the bars[]

           for (int i = 4; i < Genome.pt_cnt; i++) // start at 4 to fix supports
            {
               //this._DNA.pt_cloud[0, i] = _baseDNA.pt_cloud[0, i];
               this._DNA.pt_cloud[1, i] +=  rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//X
               this._DNA.pt_cloud[2, i] +=  rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//Y
               this._DNA.pt_cloud[3, i] +=  rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//Z
            }
            for (int i = 1; i < Genome.towerBar_cnt; i++)
            {
                if (this._DNA.bars[3, i] == 1) //if can be deactivated
                {
                    this._DNA.bars[4, i] = rndm.Next(0, Sections.count-1);
                }else { this._DNA.bars[4, i] = rndm.Next(1, Sections.count - 1);}
            }
            bb++;
           // Console.Write(bb);

           // Evaluate();  //tirar este comentario quando for para automatizar a avaliação
        }
        

        public void Evaluate()
        {
            Robot_call.Robot_interactive(false);

            Robot_call.Get_sections();
            Robot_call.Update_pts(this._DNA);

            Robot_call.Update_bars(this._DNA);

            Robot_call.Addsupports();
            this.results = Robot_call.Run_analysis();
            Robot_call.Refresh();
            Robot_call.Robot_interactive(true);
            //call GetWeight() get tons
            //get matrix with N V MY Mz for each bar
            //plug that matrix in the EC3 check
        }
        public void get_ton()
        {
            this.ton = new double();
            for(int i = 0;i< Genome.towerBar_cnt; i++)
            {
                if (this._DNA.bars[4, i] != 0) // contar so as activas
                {
                    this.ton = this.ton + results[1, i] * Robot_call.sec_prop.Area[(int)this._DNA.bars[4, i]]*7.849; //7.849 = ton / m3 steel
                }
            }
        }
    }
}
