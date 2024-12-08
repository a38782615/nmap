namespace ET
{
    public enum LRSide
    {
        LEFT = 0,
        RIGHT
    }

    public class SideHelper
    {
        public static LRSide Other(LRSide leftRight)
        {
            return leftRight == LRSide.LEFT ? LRSide.RIGHT : LRSide.LEFT;
        }
    }
}