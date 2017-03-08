using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
   public class Population
    {
        public Individual[] ind;
        //public List<Individual> ind;
        static public Random rand = new Random(0); //o random seeder so se inicia uma vez para a população

        public Population (int max_pop, Genome _baseDNA){
            this.ind = new Individual[max_pop];

            //ind = new List<Individual>();
            for (int i = 0; i < max_pop; i++)
            {
                
                ind[i]=new Individual(_baseDNA, ref rand);

            }
        }
    }
}
