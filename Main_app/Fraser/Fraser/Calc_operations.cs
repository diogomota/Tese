using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    class Calc_operations
    {
        int start_bar;
        List<Int32> next_bars;
        int section_id;
        int bar_id;

        public Calc_operations(int start,List<Int32> barlist,int s_id,int b_id)
        {
            this.start_bar = start;
            this.next_bars = barlist;
            this.section_id = s_id;
            this.bar_id = b_id;
        }

        private void verify()
        {
           // ?? é aqui?
        }
        private void repair()
        {
            //?? é aqui?
        }
    }
}
