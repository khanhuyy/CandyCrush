using System;

namespace Controller
{
    public class GameEvent
    {
        public static Action<int, int> DestroyPiece;
        public static Action<int> Retrylevel;
        public static Action<int> GoToLevel;
    }
}