using System;

namespace ObjectClasses.SpawningClasses
{
    [Serializable]
    public class QueryWordPosition
    {
        public float x;
        public float y;
        public bool isTaken;

        public QueryWordPosition()
        {
            x = y = 0;
            isTaken = false;
        }
    }
}