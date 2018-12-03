using System;

namespace DiscoDataRetriever.Wrappers.Business
{
    public class StarSystem : IComparable<StarSystem>
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }

        #region ctor
        public StarSystem(string nickName, string name, float x, float y)
        {
            this.NickName = nickName;
            this.Name = name;
            this.PosX = x;
            this.PosY = y;
        }
        #endregion

        public int CompareTo(StarSystem source)
        {
            return this.NickName.CompareTo(source.NickName);
        }
    }
}
