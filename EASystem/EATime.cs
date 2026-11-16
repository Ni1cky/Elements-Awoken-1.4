using Terraria;
using Terraria.ModLoader;

namespace ElementsAwoken.EASystem
{
    public class AETime : ModSystem
    {
        // which encounter this machine has already done the one-time setup for (0 = none)
        private int setUpEncounter = 0;

        public override void ClearWorld()
        {
            setUpEncounter = 0;
        }
        public override void PostUpdateTime()
        {
            // Encounters (the Abraxis scenes after the first kill of Infernace, Permafrost and Void Leviathan).
            // ElementsAwoken.encounter and encounterTimer are started in the boss's OnKill on the server (or in singleplayer)
            // and synced to clients with the world data (MyWorld.NetSend). This runs once per tick on every machine, so clients
            // advance the encounter on their own between syncs. The per-player part (text, screen shake, Darkness) is in MyPlayer.
            if (Main.gameMenu) return;
            int encounter = ElementsAwoken.encounter;
            if (encounter == 0)
            {
                setUpEncounter = 0;
                return;
            }
            if (setUpEncounter != encounter)
            {
                setUpEncounter = encounter;
                if (encounter >= 2)
                {
                    Main.rainTime = 3600;
                    Main.raining = true;
                    Main.maxRaining = 0.8f;
                }
            }

            ElementsAwoken.encounterTimer--;
            if (encounter == 1 && ElementsAwoken.encounterTimer <= 1600) ElementsAwoken.encounterTimer = 0; // the first encounter is shorter
            if (ElementsAwoken.encounterTimer <= 0)
            {
                ElementsAwoken.encounterTimer = 0;
                ElementsAwoken.encounter = 0;
                return;
            }

            if (encounter == 2)
            {
                Main.time += Main.rand.Next(8, 25);
                if (Main.time > 32400.0 - 30)
                {
                    Main.time = 0;
                }
            }
            if (encounter == 3)
            {
                Main.time = 16220;
                Main.dayTime = false;
            }
        }
    }
}
