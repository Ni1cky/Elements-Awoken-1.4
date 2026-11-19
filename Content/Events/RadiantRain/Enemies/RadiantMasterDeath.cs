using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ElementsAwoken.Content.Events.RadiantRain.Enemies
{
    public class RadiantMasterDeath : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // not shown in the bestiary: only the death visual of the Radiant Master
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }
        public override string Texture { get { return "ElementsAwoken/Content/Projectiles/Blank"; } }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 50;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 5;
        }
        public override void AI()
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > 650)
            {
                NPC.active = false;
            }
        }
    }
}