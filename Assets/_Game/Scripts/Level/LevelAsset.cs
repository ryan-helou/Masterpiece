using System.Collections.Generic;
using UnityEngine;

namespace Velocity
{
    /// <summary>
    /// An Inspector-editable level. Create via right-click → Create → Velocity → Level, then put
    /// the asset in a "Resources/Levels" folder and it appears in-game as a "Custom" world.
    /// The ASCII legend matches <see cref="LevelDefinition"/> (#, P, G, ^, x, -, M, V, o, b, c, &gt;, &lt;, s, S).
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "Velocity/Level", order = 0)]
    public class LevelAsset : ScriptableObject
    {
        public string id = "C-1";
        public string displayName = "Custom Level";
        public string hint = "";
        public double goldTime = 10.0;
        public double silverTime = 15.0;

        [Tooltip("One line per row, top row first. See the legend in the class summary.")]
        [TextArea(4, 24)]
        public string ascii =
            "##############\n" +
            "#            #\n" +
            "#  P      G  #\n" +
            "##############";

        public LevelDefinition ToDefinition()
        {
            var rows = (ascii ?? "").Replace("\r", "").Split('\n');
            return new LevelDefinition(
                string.IsNullOrEmpty(id) ? name : id,
                string.IsNullOrEmpty(displayName) ? name : displayName,
                hint, goldTime, silverTime, rows);
        }
    }

    /// <summary>Loads any <see cref="LevelAsset"/>s found under a Resources/Levels folder.</summary>
    public static class LevelAssetLoader
    {
        public static World LoadCustomWorld()
        {
            var assets = Resources.LoadAll<LevelAsset>("Levels");
            if (assets == null || assets.Length == 0) return null;

            var levels = new List<LevelDefinition>();
            foreach (var a in assets)
                if (a != null) levels.Add(a.ToDefinition());

            return levels.Count > 0 ? new World("C", "Custom", levels) : null;
        }
    }
}
