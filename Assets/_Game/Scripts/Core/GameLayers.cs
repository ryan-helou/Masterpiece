namespace Velocity
{
    /// <summary>
    /// Central place for the physics layer indices we declare in TagManager.asset.
    /// Keep these in sync with Project Settings > Tags and Layers.
    /// </summary>
    public static class GameLayers
    {
        public const int Default = 0;
        public const int Ground = 6;
        public const int Player = 7;
        public const int Hazard = 8;
        public const int OneWay = 9;
        public const int MovingPlatform = 10;

        /// <summary>Everything the player can stand on.</summary>
        public static int GroundMask =>
            (1 << Ground) | (1 << OneWay) | (1 << MovingPlatform);

        /// <summary>Solid geometry only (ignores one-way platforms — used when moving upward).</summary>
        public static int SolidMask =>
            (1 << Ground) | (1 << MovingPlatform);

        /// <summary>Surfaces you can wall-slide / wall-jump off (solid vertical faces).</summary>
        public static int WallMask =>
            (1 << Ground) | (1 << MovingPlatform);
    }

    public static class GameTags
    {
        public const string Finish = "Finish";
        public const string Hazard = "Hazard";
    }
}
