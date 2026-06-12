namespace Velocity
{
    /// <summary>
    /// Pure data describing one level. Geometry is an ASCII grid (top row first). Edit these
    /// in <see cref="LevelLibrary"/> or make your own — the builder turns them into a scene.
    ///
    /// Legend:
    ///   '#' solid ground / wall      ' ' or '.' empty
    ///   'P' player spawn             'G' goal / finish
    ///   '^' spike (kills)            'x' hazard block (kills)
    ///   '-' one-way platform         'M' moving platform (horizontal)
    ///   'V' moving platform (vertical)
    /// </summary>
    public class LevelDefinition
    {
        public string Id;
        public string DisplayName;
        public string Hint;
        public double GoldTime;     // medal target (seconds)
        public double SilverTime;
        public string[] Rows;

        public LevelDefinition(string id, string name, string hint,
            double gold, double silver, string[] rows)
        {
            Id = id;
            DisplayName = name;
            Hint = hint;
            GoldTime = gold;
            SilverTime = silver;
            Rows = rows;
        }
    }
}
