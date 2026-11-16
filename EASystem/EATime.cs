using ElementsAwoken.EASystem.EAPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace ElementsAwoken.EASystem
{
    public class AETime : ModSystem
    {
        public override void PostUpdateTime()
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

            if (!Main.gameMenu)
            {
                // encounterSetup must be changed in the static field, otherwise the setup (which resets the encounter text) runs every tick.
                // The encounter's progress and its end are handled by MyPlayer through UISystemSettings.VoidTime.
                int encounter = ElementsAwoken.encounter;
                if (encounter == 0)
                {
                    ElementsAwoken.encounterSetup = false; // ready for the next encounter
                }
                else
                {
                    if (!ElementsAwoken.encounterSetup)
                    {
                        ElementsAwoken.encounterSetup = true;
                        modPlayer.encounterTextTimer = 0;
                        if (encounter >= 2)
                        {
                            Main.rainTime = 3600;
                            Main.raining = true;
                            Main.maxRaining = 0.8f;
                        }
                    }
                    if (encounter == 2)
                    {
                        if (!Main.gameMenu)
                        {
                            Main.time += Main.rand.Next(8, 25);
                            if (Main.time > 32400.0 - 30)
                            {
                                Main.time = 0;
                            }
                        }
                    }
                    if (encounter == 3)
                    {
                        Main.time = 16220;
                        Main.dayTime = false;
                    }
                    if (modPlayer.encounterTextTimer > 0 || encounter == 3)
                    {
                        ElementsAwoken.DebugModeText("Encounter Text Timer: " + modPlayer.encounterTextTimer);
                        float intensity = MathHelper.Clamp((1 + (float)Math.Sin((float)modPlayer.encounterTextTimer / 20f)) * 0.25f, 0f, 1f);
                        if (encounter == 3) intensity += 0.3f;
                        if (modPlayer.finalText) intensity = 1f;
                    }
                }
            }
        }
    }
}