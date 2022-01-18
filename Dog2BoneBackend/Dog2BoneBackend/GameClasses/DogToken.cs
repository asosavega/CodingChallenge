namespace Dog2BoneBackend.GameClasses
{
    public class DogToken : GridToken
    {
        public enum Direction
        {
            Up = 1,
            Right = 2,
            Down = 3,
            Left = 4
        }

        public Direction dogDirection { get; private set; } = Direction.Up;
        public int newtXLoc { get; private set; }
        public int newtYLoc { get; private set; }
        public DogToken(int x, int y) : base(x, y)
        {
            this.newtXLoc = x;
            this.newtYLoc = y;
        }

        public void PerformDogAction(char action)
        {
            switch (Char.ToLower(action))
            {
                case 'm':
                    Move();
                    break;
                case 'r':
                    Rotate();
                    break;
                default:
                    break;
            }
        }

        private void Rotate()
        {
            int currentFacingPosition = (int)this.dogDirection;
            if (currentFacingPosition < 4)
                currentFacingPosition++;
            else
                currentFacingPosition = 1;

            this.dogDirection = (Direction)currentFacingPosition;
        }

        private void Move()
        {
            //Ensure TokenLocations are set with newTokenLocation before doing the move action
            tXLoc = newtXLoc;
            tYLoc = newtYLoc;

            //Perform Action
            switch(this.dogDirection)
            {
                case Direction.Up:
                    this.newtYLoc--;
                    break;
                case Direction.Right:
                    this.newtXLoc++;
                    break;
                case Direction.Down:
                    this.newtYLoc++;
                    break;
                case Direction.Left:
                    this.newtXLoc--;
                    break;
            }
        }
        
    }
}
 