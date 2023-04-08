﻿using System;
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
