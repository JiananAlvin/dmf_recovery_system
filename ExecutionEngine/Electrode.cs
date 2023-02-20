using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExecutionEngine
{
    public class Electrode
    {
        public int Id { get; }
        public int PositionX { get; }
        public int PositionY { get; }

        public Electrode(int id, int positionX, int positionY)
        {
            Id = id;
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}
