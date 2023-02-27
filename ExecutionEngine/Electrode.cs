using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExecutionEngine
{
    internal class Electrode
    {
        public int Id { get; }
        private int PositionX;
        private int PositionY;
        public Shape Shape { get; set; } = default!;

        public Electrode(int id, int positionX, int positionY)
        {
            this.Id = id;
            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public Electrode(int id, int positionX, int positionY, Shape shape)
        {
            this.Id = id;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.Shape = shape;
        }
    }
}
