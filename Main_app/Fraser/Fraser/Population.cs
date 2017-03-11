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
        public static int Pop_size=0;

        // constructor for first population
        public Population (int max_pop, Genome _baseDNA){
            if (Pop_size == 0) { Pop_size = max_pop; }

            this.ind = new Individual[max_pop];

            //ind = new List<Individual>();
            for (int i = 0; i < max_pop; i++)
            {
                
                ind[i]=new Individual(_baseDNA, ref rand);

            }
        }
        //constructor for iterative process /Evolve()
        public Population(Individual[] new_pop)
        {
            this.ind = new Individual[new_pop.Length];
            this.ind = new_pop;
        }

        public static Individual Select(Individual[] pop)
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
            //the selection
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

            return pop[i];
        }

        public static /*Population*/ Individual[] Evolve(Individual[] pop)
        {
            //Population Temp_pop = new Population();
            Individual[] _ind = new Individual[Pop_size];
            for(int i =0; i < Pop_size; i++)
            {
                _ind[i] = Breed(Select(pop),Select(pop));
            }
            return _ind;
        }

        public static Individual Breed(Individual a, Individual b) {
            Individual x = new Individual();

            x._DNA = a._DNA;
            if (a.fitness >= b.fitness)
            {
                for (int i = 0; i < a._DNA.pt_cloud.Length / 5; i++)
                {
                    x._DNA.pt_cloud[1, i] = rand.NextDouble()*0.3+(7*a._DNA.pt_cloud[1, i] + 3*b._DNA.pt_cloud[1,i]) / 10;//0.7*(a._DNA.pt_cloud[1, i] + 0.3*b._DNA.pt_cloud[1, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[2, i] = rand.NextDouble() * 0.3+(7*a._DNA.pt_cloud[2, i] + 3*b._DNA.pt_cloud[2, i]) / 10;//0.7*(a._DNA.pt_cloud[2, i] + 0.3*b._DNA.pt_cloud[2, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[3, i] = rand.NextDouble() * 0.3+(7*a._DNA.pt_cloud[3, i] + 3*b._DNA.pt_cloud[3, i]) / 10;//0.7*(a._DNA.pt_cloud[3, i] + 0.3*b._DNA.pt_cloud[3, i]) + 0.01 * rand.NextDouble();
                }
            }else {
                for (int i = 0; i < a._DNA.pt_cloud.Length / 5; i++)
                {
                    x._DNA.pt_cloud[1, i] = rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[1, i] + 7*b._DNA.pt_cloud[1, i]) / 10;//0.3 * (a._DNA.pt_cloud[1, i] + 0.7 * b._DNA.pt_cloud[1, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[2, i] = rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[2, i] + 7*b._DNA.pt_cloud[2, i]) / 10;//0.3 * (a._DNA.pt_cloud[2, i] + 0.7 * b._DNA.pt_cloud[2, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[3, i] = rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[3, i] + 7*b._DNA.pt_cloud[3, i]) / 10;//0.3 * (a._DNA.pt_cloud[3, i] + 0.7 * b._DNA.pt_cloud[3, i]) + 0.01 * rand.NextDouble();
                }

            }
                return x;
        }
    }
}
