using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
   public class Population
    {
       // public Individual[] ind;
        public List<Individual> ind;
        static public Random rand= new Random(0);

        public Population (int max_pop, Genome _baseDNA){
            //this.ind = new Individual[max_pop];
            ind = new List<Individual>();
            for (int i = 0; i < max_pop; i++)
            {
                
                ind.Add(new Individual(_baseDNA, ref rand));
                //this.ind[i] =  new Individual(_baseDNA, ref rand);
                //KO
            }
        }
    }
}
