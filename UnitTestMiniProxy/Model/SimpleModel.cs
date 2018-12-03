using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestMiniProxy.Model
{
    public class SimpleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public SimpleModel() { }

        public SimpleModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

    }
}
