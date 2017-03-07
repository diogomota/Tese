using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    class Sections
    {
        public List<String> section_names;
        public List<double> Area;
        public List<double> Ix;
        public List<double> Iy;

        public Sections(List<string> name, List<double> A, List<double> x, List<double> y)
        {
            section_names = new List<String>();
            section_names = name;
            Area = new List<double>();
            Area = A;
            Ix = new List<double>();
            Ix = x;
            Iy = new List<double>();
            Iy = y;
        }
    }
}
