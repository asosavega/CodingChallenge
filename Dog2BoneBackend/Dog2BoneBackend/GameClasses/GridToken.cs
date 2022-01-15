namespace Dog2BoneBackend.GameClasses
{
    public abstract class GridToken
    {
        public int tXLoc { get; protected set;}
        public int tYLoc { get; protected set;}

        public GridToken(int x, int y)
        {
            this.tXLoc = x;
            this.tYLoc = y;
        }
    }
}
