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
        double fitness;

        public Individual(Genome _baseDNA,ref Random rndm)
        {
            //o _baseDNA tambe esta a ser mudado... o constructor tem de fixar o base dna e depois outro metodo faz a mutaçao inicial
            //que sera chamada da population 
            this.fitness = 0.0;

            _DNA = new Genome(); // create new Genome
            //if the following is not done the same matrix is allways changing (need to create copies)

            _DNA.pt_cloud = (double[,])_baseDNA.pt_cloud.Clone(); // copy by value the pt cloud[]
            _DNA.bars = (double[,])_baseDNA.bars.Clone(); //copy by value the bars[]

           for (int i = 4; i < Genome.pt_cnt; i++)
            {
               //this._DNA.pt_cloud[0, i] = _baseDNA.pt_cloud[0, i];
               this._DNA.pt_cloud[1, i] +=  rndm.Next(-1, 1) * rndm.NextDouble();
               this._DNA.pt_cloud[2, i] +=  rndm.Next(-1, 1) * rndm.NextDouble();
               this._DNA.pt_cloud[3, i] +=  rndm.Next(-1, 1) * rndm.NextDouble();
            }

            fitness = rndm.Next(-1, 1) * rndm.NextDouble();//Ok (tambem dentro do for)
        }
        

        public void Evaluate()
        {

        }
    }
}
