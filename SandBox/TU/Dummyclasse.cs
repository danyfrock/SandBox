using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox.TU
{
    internal class Dummyclasse
    {
        private string name = string.Empty;
        private int type;

        public Dummyclasse(string name, int type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get => name; set => name = value; }
        public int Type { get => type; set => type = value; }
    }
}
