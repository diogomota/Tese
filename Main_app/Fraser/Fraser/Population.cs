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
        static int Pop_size=0;

        public Population (int max_pop, Genome _baseDNA){
            if (Pop_size == 0) { Pop_size = max_pop; }

            this.ind = new Individual[max_pop];

            //ind = new List<Individual>();
            for (int i = 0; i < max_pop; i++)
            {
                
                ind[i]=new Individual(_baseDNA, ref rand);

            }
        }

       public static int Select(Individual[] pop)
        {
            int i=-1;
            double total_fitness = new double();
            // get total fitness for roulette wheel selection

            for (int a=0; a < Pop_size; a++)
            {
                total_fitness += pop[a].fitness;
            }

            double Sel = rand.NextDouble();
            double previous_fit = 0.0;
            double current_fit = 0.0;

            for (int b=0; b< Pop_size; b++)
            {
                current_fit += pop[b].fitness;
                if ( Sel >=previous_fit/total_fitness && Sel <= current_fit / total_fitness) {
                    i = b;  break;
                }
                else { 
                    previous_fit = current_fit;
                }

            }

            //return pop[i];
            return i;
        }

    }
}
