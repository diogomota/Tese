﻿using System;
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

        const double pt_mutation_prob = 0.3;

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

        public static Individual Breed(Individual a, Individual b,int gen) {
            Individual x = new Individual();

            x._DNA = a._DNA;
            /*if (a.fitness >= b.fitness)
            {
                for (int i = 0; i < a._DNA.pt_cloud.Length / 5; i++) // no final fazer 1º crossover de alguns genes (pts) e so depois mutação em alguns genes do novo Genome
                {
                    x._DNA.pt_cloud[1, i] = (2/(gen+1))*rand.Next(-1, 1)*rand.NextDouble()*0.3+(7*a._DNA.pt_cloud[1, i] + 3*b._DNA.pt_cloud[1,i]) / 10;//0.7*(a._DNA.pt_cloud[1, i] + 0.3*b._DNA.pt_cloud[1, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[2, i] = (2 / (gen + 1)) *rand.Next(-1, 1) * rand.NextDouble() * 0.3+(7*a._DNA.pt_cloud[2, i] + 3*b._DNA.pt_cloud[2, i]) / 10;//0.7*(a._DNA.pt_cloud[2, i] + 0.3*b._DNA.pt_cloud[2, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[3, i] = (2 / (gen + 1)) * rand.Next(-1, 1) * rand.NextDouble() * 0.3+(7*a._DNA.pt_cloud[3, i] + 3*b._DNA.pt_cloud[3, i]) / 10;//0.7*(a._DNA.pt_cloud[3, i] + 0.3*b._DNA.pt_cloud[3, i]) + 0.01 * rand.NextDouble();
                }
            }else {
                for (int i = 0; i < a._DNA.pt_cloud.Length / 5; i++)
                {
                    x._DNA.pt_cloud[1, i] = (2 / (gen + 1)) * rand.Next(-1, 1) * rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[1, i] + 7*b._DNA.pt_cloud[1, i]) / 10;//0.3 * (a._DNA.pt_cloud[1, i] + 0.7 * b._DNA.pt_cloud[1, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[2, i] = (2 / (gen + 1)) * rand.Next(-1, 1) * rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[2, i] + 7*b._DNA.pt_cloud[2, i]) / 10;//0.3 * (a._DNA.pt_cloud[2, i] + 0.7 * b._DNA.pt_cloud[2, i]) + 0.01 * rand.NextDouble();
                    x._DNA.pt_cloud[3, i] = (2 / (gen + 1)) * rand.Next(-1, 1) * rand.NextDouble() * 0.3+(3*a._DNA.pt_cloud[3, i] + 7*b._DNA.pt_cloud[3, i]) / 10;//0.3 * (a._DNA.pt_cloud[3, i] + 0.7 * b._DNA.pt_cloud[3, i]) + 0.01 * rand.NextDouble();
                }

            }*/
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
                    x._DNA.pt_cloud[1, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble())*(1/(gen+1));
                    x._DNA.pt_cloud[2, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble()) * (1 / (1+gen));
                    x._DNA.pt_cloud[3, i] += x._DNA.pt_cloud[4, i] * (Population.rand.Next(-1, 1) * Population.rand.NextDouble()) * (1 / (1+gen));
                }
            }

            ///BARS
            ///

            ///CrossOver
            ///
            CrossOver_pt = Population.rand.Next(0, Genome.towerBar_cnt);

            for (int i = CrossOver_pt; i < Genome.towerBar_cnt; i++)
            {
                x._DNA.bars[1, 4] = b._DNA.bars[1, 4];
            }

            return x;
        }
    }
}
