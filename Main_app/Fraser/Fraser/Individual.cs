using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    public class Individual : IComparable<Individual>
    {
        public Genome _DNA;
        public double fitness;
        public double ton;
        public double[,] results;
        static int bb = 0;

        private List<Calc_operations> Leg_ops = new List<Calc_operations>(); // lista leg calcs
        private List<Calc_operations> Bracing_ops = new List<Calc_operations>(); // list bracing calcs
        private List<Calc_operations> Horiz_ops_plane_bracing = new List<Calc_operations>(); // list off plane bracing 
        private List<Calc_operations> Horiz_ops_Offplane_bracing;

        public Individual()
        {

        }

        public Individual(Genome _baseDNA, ref Random rndm)
        {
            this.fitness = 0.0;

            _DNA = new Genome(); // create new Genome

            //if the following is not done the same matrix is always changing (need to create copies):

            _DNA.pt_cloud = (double[,])_baseDNA.pt_cloud.Clone(); // copy by value the pt cloud[]
            _DNA.bars = (double[,])_baseDNA.bars.Clone(); //copy by value the bars[]

            for (int i = 4; i < Genome.pt_cnt; i++) // start at 4 to fix supports
            {
                //mutate initial pt coord.
                this._DNA.pt_cloud[1, i] += rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//X
                this._DNA.pt_cloud[2, i] += rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//Y
                this._DNA.pt_cloud[3, i] += rndm.Next(-1, 1) * rndm.NextDouble() * this._DNA.pt_cloud[4, i];//Z
            }
            //define init sections 
            for (int i = 0; i < Genome.bar_cnt; i++)
            {
                #region previous code

                /*if (this._DNA.bars[3, i] == 1) //if can be deactivated
                {
                    this._DNA.bars[4, i] = 1;//rndm.Next(0, Sections.count - 1);
                } else { this._DNA.bars[4, i] = rndm.Next(1, Sections.count - 1); }*/
                #endregion

                if (this._DNA.bars[4,i] == -1)
                {
                    this._DNA.bars[4, i] = 0; //a primeira secção a definir será a dos braçoss
                    //secção de braços
                }else
                {
                    this._DNA.bars[4, i] = Sections.count - 1; // começar com a secção maxima
                }
                
            }
            bb++;
            // Console.Write(bb);

        }


        public void Evaluate()
        {
            Robot_call.Robot_interactive(false);

            Robot_call.Update_pts(this._DNA);
            Robot_call.Update_bars(this._DNA);

            Robot_call.Addsupports();

            // create calculation lists //
            Leg_calc_list();
            HorizBar_calc_list();
            Bracing_calc_list();
            /////////////////////////////

            // Robot_call.Robot_interactive(true);
            //Robot_call.Refresh();

            this.results = Robot_call.Run_analysis();

            // NOTA: definir a primeira geração com a secção maxima em todas as barras e meter uma rotina que para tudo se 
            // mesmo com essa secção falhar

            // analisar todas as calcOps
            // com os resultados reparar as barras
            //calc fitness  = peso

            this.fitness = calc_fitess();

            Robot_call.Refresh(); // é mesmo necessario ?? (fica mais rapido sem)

            //Robot_call.Robot_interactive(true);

            //call GetWeight() get tons
            //get matrix with N V MY Mz for each bar
            //plug that matrix in the EC3 check
        }

        public double calc_fitess()
        {
            double sum = new double();
            int cnt = 0;
            int totalPenalty = 0;
            for (int i = 0; i < Genome.towerBar_cnt; i++)
            {
                if (this._DNA.bars[4, i] != 0)
                {
                    if (this.results[2, i] / 90000 < 1.0)
                    {
                        sum += this.results[2, i] / 90000;
                        cnt += 1;
                        Console.WriteLine(" force" + this.results[2, i] + "Count:" + cnt);
                    }
                    else
                    {
                        if (totalPenalty <= 5)
                        {
                            sum -= 0.05; //penalty
                            totalPenalty++;
                            cnt += 1;
                        }
                    }
                }
            }
            Console.WriteLine(" Fitness:" + sum / cnt + "; ");
            return sum / cnt;
        }
        public void get_ton()
        {
            this.ton = new double();
            for (int i = 0; i < Genome.towerBar_cnt; i++)
            {
                if (this._DNA.bars[4, i] != 0) // contar so as activas
                {
                    this.ton = this.ton + results[1, i] * Sections.Area[(int)this._DNA.bars[4, i]] * 7.849; //7.849 = ton / m3 steel
                }
            }
            this.fitness = this.ton;
        }


        /////////////////////////////////////
        //Define Calc Lists (Virtual Model)//
        /////////////////////////////////////

        private void Leg_calc_list()
        {
            //#########################//
            //      Leg 1of4           //
            //#########################//
            List<Int32> temp = new List<Int32>();

            for (int i = 0; i < Genome.horizd-1; i++)
            {
                if (i == 0) // barra inicial
                {
                    temp.Add(i + 1);
                    //se tiver barras horiz ou bracing no no sup e/ou mudar a secção-> add to list jump to leg 2of4
                    if (v_braced(1, new List<Int32>() { 1, 2 })) {
                        this.Leg_ops.Add(new Calc_operations(1, temp, (int)this._DNA.bars[4, 0], (int)this._DNA.bars[5, 0])); // se a 1 barra esta braced add logo ao calculo
                        temp = new List<Int32>();
                    } else if (this._DNA.bars[4, 0] != this._DNA.bars[4, (8 * (int)Genome.subd) + 4]) //se secção diferente "braced"
                    {
                        this.Leg_ops.Add(new Calc_operations(1, temp, (int)this._DNA.bars[4, 0], (int)this._DNA.bars[5, 0]));
                        temp = new List<Int32>();
                    }else
                    {
                        //nada ( passa para a outra barra )
                    }

                }

                else {

                    int bar_ind = 8 * (int)Genome.subd + 5 + (i - 1) * (4 * (int)Genome.subd * (int)Genome.subd + 8 * (int)Genome.subd - 8);
                    if (v_braced(bar_ind, new List<Int32>() { 1, 2 }))
                    {
                        if (temp != null) // se nao for null o start bar é o temp[0]
                        {
                            temp.Add(bar_ind); // add esta barra
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind-1], (int)this._DNA.bars[5, bar_ind-1]));
                            temp = new List<Int32>();
                        } else
                        {
                            temp.Add(bar_ind);
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind-1], (int)this._DNA.bars[5, bar_ind-1]));
                            temp = new List<Int32>();
                        }
                    } else if (this._DNA.bars[4,bar_ind-1] != this._DNA.bars[4,bar_ind+ (4 * (int)Genome.subd * (int)Genome.subd - 8 * (int)Genome.subd - 8)-1]) //secção =/=
                    {
                        temp.Add(bar_ind);
                        this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                        temp = new List<Int32>();

                    } else { temp.Add(bar_ind); } // se nao esta braced vai para a lista

                }
            }

            temp = new List<Int32>(); // reset for next leg


            //#########################//
            //      Leg 2of4           //
            //#########################//

            for (int i = 0; i < Genome.horizd - 1; i++)
            {
                if (i == 0) // barra inicial
                {
                    temp.Add(3*(int)Genome.subd+2); // barra inicial funçao de subdiv

                    //se tiver barras horiz ou bracing no no sup e/ou mudar a secção-> add to list jump to leg 2of4
                    if (v_braced(3 * (int)Genome.subd + 2, new List<Int32>() { 1, 2 }))
                    {
                        this.Leg_ops.Add(new Calc_operations(3 * (int)Genome.subd + 2, temp, (int)this._DNA.bars[4, 3 * (int)Genome.subd + 2-1], (int)this._DNA.bars[5, 3 * (int)Genome.subd + 2-1])); // se a 1 barra esta braced add logo ao calculo
                        temp = new List<Int32>();
                    }
                    else if (this._DNA.bars[4, 3 * (int)Genome.subd + 2-1] != this._DNA.bars[4, (int)Genome.subd*(int)Genome.subd + 10*(int)Genome.subd+2]) //se secção seguinte diferente -> "braced"
                    {
                        this.Leg_ops.Add(new Calc_operations(3 * (int)Genome.subd + 2, temp, (int)this._DNA.bars[4, 3 * (int)Genome.subd + 2-1], (int)this._DNA.bars[5, 3 * (int)Genome.subd + 2-1]));
                        temp = new List<Int32>();
                    }
                    else
                    {
                        //nada ( passa para a outra barra )
                    }

                }

                else
                {

                    int bar_ind = (int)Genome.subd * (int)Genome.subd + 10 * (int)Genome.subd + 3   + (i - 1) * (4 * (int)Genome.subd * (int)Genome.subd + 8 * (int)Genome.subd - 8);
                    if (v_braced(bar_ind, new List<Int32>() { 1, 2 }))
                    {
                        if (temp != null) // se nao for null o start bar é o temp[0]
                        {
                            temp.Add(bar_ind); // add esta barra
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                        else
                        {
                            temp.Add(bar_ind);
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                    }
                    else if (this._DNA.bars[4, bar_ind - 1] != this._DNA.bars[4, bar_ind + (4 * (int)Genome.subd * (int)Genome.subd - 8 * (int)Genome.subd - 8) - 1]) //secção =/=
                    {
                        temp.Add(bar_ind);
                        this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                        temp = new List<Int32>();

                    }
                    else { temp.Add(bar_ind); } // se nao esta braced vai para a lista

                }
            }

            temp = new List<Int32>(); // reset for next leg

            //#########################//
            //      Leg 3of4           //
            //#########################//

            for (int i = 0; i < Genome.horizd - 1; i++)
            {
                if (i == 0) // barra inicial
                {
                    temp.Add(5 * (int)Genome.subd + 3); // barra inicial funçao de subdiv

                    //se tiver barras horiz ou bracing no no sup e/ou mudar a secção-> add to list jump to leg 2of4
                    if (v_braced(5 * (int)Genome.subd + 3, new List<Int32>() { 1, 2 }))
                    {
                        this.Leg_ops.Add(new Calc_operations(5 * (int)Genome.subd + 3, temp, (int)this._DNA.bars[4, 5 * (int)Genome.subd + 3 - 1], (int)this._DNA.bars[5, 5 * (int)Genome.subd + 3 - 1])); // se a 1 barra esta braced add logo ao calculo
                        temp = new List<Int32>();
                    }
                    else if (this._DNA.bars[4, 5 * (int)Genome.subd + 3 - 1] != this._DNA.bars[4, 2*(int)Genome.subd * (int)Genome.subd + 12 * (int)Genome.subd]) //se secção seguinte diferente -> "braced"
                    {
                        this.Leg_ops.Add(new Calc_operations(5 * (int)Genome.subd + 3, temp, (int)this._DNA.bars[4, 5 * (int)Genome.subd + 3 - 1], (int)this._DNA.bars[5, 5 * (int)Genome.subd + 3 - 1]));
                        temp = new List<Int32>();
                    }
                    else
                    {
                        //nada ( passa para a outra barra )
                    }

                }

                else
                {

                    int bar_ind = 2 * (int)Genome.subd * (int)Genome.subd + 12 * (int)Genome.subd + 1   + (i - 1) * (4 * (int)Genome.subd * (int)Genome.subd + 8 * (int)Genome.subd - 8);
                    if (v_braced(bar_ind, new List<Int32>() { 1, 2 }))
                    {
                        if (temp != null) // se nao for null o start bar é o temp[0]
                        {
                            temp.Add(bar_ind); // add esta barra
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                        else
                        {
                            temp.Add(bar_ind);
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                    }
                    else if (this._DNA.bars[4, bar_ind - 1] != this._DNA.bars[4, bar_ind + (4 * (int)Genome.subd * (int)Genome.subd - 8 * (int)Genome.subd - 8) - 1]) //secção =/=
                    {
                        temp.Add(bar_ind);
                        this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                        temp = new List<Int32>();

                    }
                    else { temp.Add(bar_ind); } // se nao esta braced vai para a lista

                }
            }

            temp = new List<Int32>(); // reset for next leg

            //#########################//
            //      Leg 4of4           //
            //#########################//

            for (int i = 0; i < Genome.horizd - 1; i++)
            {
                if (i == 0) // barra inicial
                {
                    temp.Add(7 * (int)Genome.subd + 4); // barra inicial funçao de subdiv

                    //se tiver barras horiz ou bracing no no sup e/ou mudar a secção-> add to list jump to leg 2of4
                    if (v_braced(7 * (int)Genome.subd + 4, new List<Int32>() { 1, 2 }))
                    {
                        this.Leg_ops.Add(new Calc_operations(7 * (int)Genome.subd + 4, temp, (int)this._DNA.bars[4, 7 * (int)Genome.subd + 4 - 1], (int)this._DNA.bars[5, 7 * (int)Genome.subd + 4 - 1])); // se a 1 barra esta braced add logo ao calculo
                        temp = new List<Int32>();
                    }
                    else if (this._DNA.bars[4, 7 * (int)Genome.subd + 4 - 1] != this._DNA.bars[4, 3 * (int)Genome.subd * (int)Genome.subd + 14 * (int)Genome.subd - 2]) //se secção seguinte diferente -> "braced"
                    {
                        this.Leg_ops.Add(new Calc_operations(7 * (int)Genome.subd + 4, temp, (int)this._DNA.bars[4, 7 * (int)Genome.subd + 4 - 1], (int)this._DNA.bars[5, 7 * (int)Genome.subd + 4 - 1]));
                        temp = new List<Int32>();
                    }
                    else
                    {
                        //nada ( passa para a outra barra )
                    }

                }

                else
                {

                    int bar_ind = 3 * (int)Genome.subd * (int)Genome.subd + 14 * (int)Genome.subd - 1 + (i - 1) * (4 * (int)Genome.subd * (int)Genome.subd + 8 * (int)Genome.subd - 8);
                    if (v_braced(bar_ind, new List<Int32>() { 1, 2 }))
                    {
                        if (temp != null) // se nao for null o start bar é o temp[0]
                        {
                            temp.Add(bar_ind); // add esta barra
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                        else  ///nao e necessario este else (nem nas outras legs) -> para a fase de CLEAN UP
                        {
                            temp.Add(bar_ind);
                            this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                            temp = new List<Int32>();
                        }
                    }
                    else if (this._DNA.bars[4, bar_ind - 1] != this._DNA.bars[4, bar_ind + (4 * (int)Genome.subd * (int)Genome.subd - 8 * (int)Genome.subd - 8) - 1]) //secção =/=
                    {
                        temp.Add(bar_ind);
                        this.Leg_ops.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                        temp = new List<Int32>();

                    }
                    else { temp.Add(bar_ind); } // se nao esta braced vai para a lista

                }
            }

        }
        private void Bracing_calc_list()
        {
            List<Calc_operations> x = new List<Calc_operations>();
            for (int i = 0; i < Genome.towerBar_cnt; i++){
                if (this._DNA.bars[5, i] == 1 && this._DNA.bars[4,i] !=0) //if bracing && not deactivated
                {
                   Bracing_ops.Add(new Calc_operations(i, new List<Int32>() { i }, (int)this._DNA.bars[4, i], (int)this._DNA.bars[5, i]));
                }
            }
        }
        private void HorizBar_calc_list()
        {
            List<Int32> temp = new List<Int32>();

            int bar_start_num = (8*(int)Genome.subd+4) + ((int)Genome.horizd-2)*(4*(int)Genome.subd*(int)Genome.subd+8*(int)Genome.subd-8)+1;

            for(int i = 0; i < Genome.horizd-1; i++) //percorrer em altura as horiz div
            {
                for(int b=0; b <= 3; b++) //percorrer cada lado da horiz div
                {
                    for(int c=0; c <Genome.subd; c++) // percorrer o nº do barras em cada lado
                    {
                        int bar_ind = bar_start_num + (int)Genome.subd * b + c + i*4*(int)Genome.subd;

                        if (this._DNA.bars[4, bar_ind - 1] != 0) { // se a barra não esta desactiva

                            if (h_plane_braced(bar_start_num + (int)Genome.subd * b + c, new List<Int32>() { 0, 1 })) //EN 1993-3-1:2006 H.3.9 (2)
                            {
                                temp.Add(bar_ind); // add esta barra
                                this.Horiz_ops_plane_bracing.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                                temp = new List<Int32>();
                            }else if (this._DNA.bars[4,bar_ind - 1] != this._DNA.bars[4, bar_ind]) //se a proxima tem secção diferente
                            {
                                temp.Add(bar_ind); // add esta barra
                                this.Horiz_ops_plane_bracing.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, bar_ind - 1], (int)this._DNA.bars[5, bar_ind - 1]));
                                temp = new List<Int32>();
                            }
                            else
                            {
                                temp.Add(bar_ind);
                            }

                        }else if (temp!= null) //se a barra anterior nao estava braced e saltou para esta que nao esta activa nao procurar add mais barras, adicionar a lista de calc
                            //provavelmente será aqui que se vao eliminar estas barras que a partida como nao esta braced ficam instaveis (ver 1º como o robot reage)
                        {
                            this.Horiz_ops_plane_bracing.Add(new Calc_operations(temp[0], temp, (int)this._DNA.bars[4, temp[0] - 1], (int)this._DNA.bars[5, temp[0] - 1]));
                        }
                    }

                }

            }


        }

        //Virtual Model auxiliary functions
        private bool v_braced(int bar_num, List<Int32> tmp)
        {
            int h_bracing = 0;
            int d_bracing = 0;

            for (int i = 0; i<Genome.bar_cnt; i++) //procurar o indice da barra a analisar
            {
                

                if ( this._DNA.bars[0,i] == bar_num - 1) // quando encontrar a barra a analisar na lista começar o loop que procura barras adjacentes com id que premita bracing
                {
                   
                    for (int b=0; b < Genome.bar_cnt; b++) // loop em todas as barras
                    {
                        if(tmp.Contains((int)this._DNA.bars[5, b])) // verificar nas barras c/ id que podem ser bracing
                        {
                            if(this._DNA.bars[4,b] != 0) //se nao esta desactivada
                            {
                                // se partilha nós com a barra em analise
                                if(this._DNA.bars[1,b]==this._DNA.bars[2,i] || this._DNA.bars[2, b] == this._DNA.bars[2, i])
                                {
                                    if (this._DNA.bars[5, b] == 2) { h_bracing++; }
                                    if (this._DNA.bars[5, b] == 1) { d_bracing++; }
                                }
                            }
                        }
                              
                    }
                   
                }
               
            }
            if (h_bracing >= 2 || d_bracing >= 2) { return true; } else { return false; }
        }
        private bool h_plane_braced(int bar_num, List<Int32> tmp){

            int bracing = 0;

            for (int i = 0; i < Genome.bar_cnt; i++) //procurar o indice da barra a analisar
            {


                if (this._DNA.bars[0, i] == bar_num - 1) // quando encontrar a barra a analisar na lista começar o loop que procura barras adjacentes com id que premita bracing
                {

                    for (int b = 0; b < Genome.bar_cnt; b++) // loop em todas as barras
                    {
                        if (tmp.Contains((int)this._DNA.bars[5, b])) // verificar nas barras c/ id que podem ser bracing
                        {
                            if (this._DNA.bars[4, b] != 0) //se nao esta desactivada
                            {
                                // se partilha nós com a barra em analise
                                if (this._DNA.bars[1, b] == this._DNA.bars[2, i] || this._DNA.bars[2, b] == this._DNA.bars[2, i])
                                {
                                    if (this._DNA.bars[5, b] == 0) { bracing++; }
                                    if (this._DNA.bars[5, b] == 1) { bracing++; }
                                }
                            }
                        }

                    }

                }

            }
            if (bracing>=1) { return true; } else { return false; }
        }

        int IComparable<Individual>.CompareTo(Individual other) // sorting algorithm
        {
            Individual iToCompare = (Individual)other;
            if(fitness < iToCompare.fitness)
            {
                return 1;
            }
            else if(fitness > iToCompare.fitness)
            {
                return -1;
            }
            return 0;
        }

    }
}
