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
        static public Random rand = new Random(System.DateTime.Now.Second*System.DateTime.Now.Millisecond); //o random seeder so se inicia uma vez para a população
        public static int Pop_size=0;

        const double pt_mutation_prob = 0.3;
        const double sec_mutation_prob = 0.1;

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
                total_fitness += pop[0].fitness-pop[a].fitness;
            }

            double Sel = rand.NextDouble(); // it's a minimization problem!!
            double previous_fit = 0.0;
            double current_fit = 0.0;

            // check to see if tournament selection is a better option for minimization problem!!!!!!!

            //the selection

            for (int b=0; b<Pop_size; b++) // check to see if tournament selection is a better option for minimization problem
            {
                current_fit += pop[0].fitness-pop[b].fitness; // minimizar = aumentar distancia entre max fitness da geração e os outros individuos
                

                if ( Sel >previous_fit/total_fitness && Sel <= current_fit / total_fitness) {
                    i = b;  break;
                }
                else {
                    previous_fit = current_fit;
                }

            }

            return pop[i];
        }

        public static Individual[] Evolve(Individual[] pop,int gen)
        {
            //Population Temp_pop = new Population();
            Individual[] _ind = new Individual[Pop_size];
            for(int i =0; i < Pop_size; i++)
            {
                _ind[i] = Breed(Select(pop),Select(pop),gen);
            }
            return _ind;
        }
        public static Individual Evolve_single(Individual[] pop, int gen)
        {
            //Population Temp_pop = new Population();
            Individual _ind = new Individual();
            //_ind = Breed(Select(pop), Select(pop), gen);
            _ind = Breed(Tournament_selection(pop), Tournament_selection(pop),gen);
            return _ind;

        }

        public static Individual Tournament_selection(Individual[] pop)
        {
            int selection_pressure = (int)Pop_size/2; // n individuos a concorrer
            int[] tournament = new int[selection_pressure];

            for (int i = 0; i < selection_pressure; i++)
            {
                tournament[i] = Population.rand.Next(0, Pop_size - 1);
            }

            Array.Sort(tournament);
            return pop[tournament.Last<int>()];
        }

        public static Individual Breed(Individual a, Individual b,int gen) {
            Individual x = new Individual();

            x._DNA = a._DNA;
            ///PTS
            ///

            ///CrossOver
            ///

            int CrossOver_pt = Population.rand.Next(0, Genome.pt_cnt);
            for(int i= CrossOver_pt; i < Genome.pt_cnt; i++)
            {
                x._DNA.pt_cloud[1, i] = b._DNA.pt_cloud[1, i];
                x._DNA.pt_cloud[2, i] = b._DNA.pt_cloud[2, i];
                x._DNA.pt_cloud[3, i] = b._DNA.pt_cloud[3, i];
            }

            ///Mutation
            ///
            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                double _rnd = Population.rand.NextDouble();
                if (_rnd < pt_mutation_prob)
                {
                    x._DNA.pt_cloud[1, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble())*0.25;
                    x._DNA.pt_cloud[2, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble())*0.25;
                    x._DNA.pt_cloud[3, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble())*0.25;
                }
            }

            ///BARS
            ///

            ///CrossOver
            ///

            CrossOver_pt = Population.rand.Next(0, Genome.towerBar_cnt);
            int end_crossover_pt = Population.rand.Next(CrossOver_pt, Genome.towerBar_cnt);

            for (int i = CrossOver_pt; i < end_crossover_pt; i++)
            {
                x._DNA.bars[4, i] = b._DNA.bars[4, i];
            }
            ///Mutation
            ///
            int cnt = 0;
            
            for (int i = 0; i < Genome.towerBar_cnt; i++)
            {
                double _rnd = Population.rand.NextDouble();
                if (_rnd < sec_mutation_prob)
                {
                    if (x._DNA.bars[3, i] == 1)
                    {
                        double sigma = Sections.count / 6;
                        double gene_val = gaussianMutation(x._DNA.bars[4, i], sigma);
                        gene_val = clamp(gene_val, 0, Sections.count - 1);
                        //x._DNA.bars[4, i] = Population.rand.Next(0, Sections.count - 1); //se pode ser descativada random de 0 ate sec count

                        x._DNA.bars[4, i] = (int)gene_val;
                        cnt++;
                    } else
                    {
                        double sigma = (Sections.count-1) / 6;
                        double gene_val = gaussianMutation(x._DNA.bars[4, i], sigma);
                        gene_val = clamp(gene_val, 1, Sections.count - 1);

                        // x._DNA.bars[4, i] = Population.rand.Next(1, Sections.count - 1); // se nao random de 1 ate sec count

                        x._DNA.bars[4, i] = (int)gene_val;
                        cnt++;
                    }

                }
            }

            return x;
        }

        /// <summary>
        /// Gaussian Mutation
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stddev"></param>
        /// <returns></returns>
        private static double gaussianMutation(double mean, double stddev)
        {
            double x1 = rand.NextDouble();
            double x2 = rand.NextDouble();

            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            // Thanks to Colin Green for catching this.
            if (x1 == 0)
                x1 = 1;
            if (x2 == 0)
                x2 = 1;

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return y1 * stddev + mean;
        }
        private static double clamp(double val, double min, double max)
        {
            if (val >= max)
                return max;
            if (val <= min)
                return min;
            return val;
        }
    }
}
