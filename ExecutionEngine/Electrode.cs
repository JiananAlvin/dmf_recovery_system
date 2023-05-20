namespace ExecutionEngine
{
    public class Electrode
    {
        public int Id { get; }
        public int PositionX { get; }
        public int PositionY { get; }
        public Triangle Triangle { get; set; } = default!;

        public Electrode(int id, int positionX, int positionY)
        {
            this.Id = id;
            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public Electrode(int id, int positionX, int positionY, Triangle triangle)
        {
            this.Id = id;
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.Triangle = triangle;
        }


        public override string ToString()
        {
            return $"[Id:{Id}, PositionX:{PositionX}, PositionY:{PositionY}"+ (Triangle==null?"]": $",[Triangle:{Triangle.ToString()}]");
        }
    }
}
