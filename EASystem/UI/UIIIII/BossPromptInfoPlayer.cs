using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ElementsAwoken.EASystem.UI.UIIIII
{
    /// <summary>
    /// Remembers per character whether the boss prompt info popup has been closed.
    /// </summary>
    public class BossPromptInfoPlayer : ModPlayer
    {
        public bool seenBossPromptInfo;
        public override void SaveData(TagCompound tag)
        {
            if (seenBossPromptInfo) tag["seenBossPromptInfo"] = true;
        }
        public override void LoadData(TagCompound tag)
        {
            seenBossPromptInfo = tag.GetBool("seenBossPromptInfo");
        }
    }
}
