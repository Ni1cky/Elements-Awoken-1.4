using ElementsAwoken.Content.NPCs.Bosses.Ancients;
using ElementsAwoken.Content.NPCs.Bosses.Aqueous;
using ElementsAwoken.Content.NPCs.Bosses.Azana;
using ElementsAwoken.Content.NPCs.Bosses.Infernace;
using ElementsAwoken.Content.NPCs.Bosses.Permafrost;
using ElementsAwoken.Content.NPCs.Bosses.Regaroth;
using ElementsAwoken.Content.NPCs.Bosses.TheGuardian;
using ElementsAwoken.Content.NPCs.Bosses.VoidLeviathan;
using ElementsAwoken.Content.NPCs.Bosses.Volcanox;
using ElementsAwoken.EASystem.EAPlayer;
using Terraria;
using Terraria.ModLoader;

namespace ElementsAwoken.EASystem.Biome
{
    public class EABiomes : ModBiome
    {
        // Boss
        public bool _useAncients = false;        
        public bool _useAzana = false;
        public bool _leviathan = false;
        public bool _volcanox = false;
        public bool _useGuardian = false;                  
        public bool _useAqueous = false;
        public bool _useAqueousSky = false;
        public bool _usePermafrost = false;
        public bool _useInfernace = false;
        // Event
        public bool _useVoidEvent = false;
        public bool _useVoidEventDark = false;
        public bool _useRadRain = false;
        // Encounter
        public bool _useEncounter1 = false;
        public bool _useEncounter2 = false;
        public bool _useEncounter3 = false;
        // Biomes
        public bool _temple = false;
        public override bool IsBiomeActive(Player player)
        {
            #region Boss
            _useAncients = NPC.AnyNPCs(ModContent.NPCType<Izaris>()) || NPC.AnyNPCs(ModContent.NPCType<Kirvein>()) || NPC.AnyNPCs(ModContent.NPCType<Krecheus>()) || NPC.AnyNPCs(ModContent.NPCType<Xernon>()) || NPC.AnyNPCs(ModContent.NPCType<AncientAmalgam>());
            _useAzana = NPC.AnyNPCs(ModContent.NPCType<Azana>());
            _leviathan = NPC.AnyNPCs(ModContent.NPCType<VoidLeviathanHead>());
            _volcanox = NPC.AnyNPCs(ModContent.NPCType<Volcanox>());
            _useGuardian = NPC.AnyNPCs(ModContent.NPCType<TheGuardianFly>());
            _useAqueous = NPC.AnyNPCs(ModContent.NPCType<Aqueous>());
            _usePermafrost = NPC.AnyNPCs(ModContent.NPCType<Permafrost>());
            _useInfernace = NPC.AnyNPCs(ModContent.NPCType<Infernace>());      
            _temple = BiomeTileCounterSystem._lizardTiles >= 50;

            // Every flag is updated on every call. With early returns, a flag further down kept its old value while
            // anything above it was active (a boss alive, the player in the temple, an event...), so its screen filter stayed on.
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            modPlayer.useAncients = _useAncients;
            modPlayer.useAzana = _useAzana;
            modPlayer.useLeviathan = _leviathan;
            modPlayer.useVolcanox = _volcanox;
            modPlayer.useGuardian = _useGuardian;
            modPlayer.useAqueous = _useAqueous;
            modPlayer.usePermafrost = _usePermafrost;
            modPlayer.useInfernace = _useInfernace;
            MyPlayer.zoneTemple = _temple;
            #endregion

            #region Event
            _useVoidEvent = MyWorld.voidInvasionUp && Main.time <= 16220 && !Main.dayTime;
            _useVoidEventDark = MyWorld.voidInvasionUp && Main.time > 16220 && !Main.dayTime;
            _useRadRain = MyWorld.radiantRain && player.position.Y / 16 < Main.worldSurface;

            modPlayer.useVoidEvent = _useVoidEvent;
            modPlayer.useVoidEventDark = _useVoidEventDark;
            modPlayer.useRadRain = _useRadRain;
            #endregion

            #region Regaroth
            int regarothType = ModContent.NPCType<RegarothHead>();
            int regaroth = NPC.FindFirstNPC(regarothType);
            if (regaroth >= 0)
            {
                NPC npc = Main.npc[regaroth];
                bool intense = npc.localAI[1] == 1;
                if (npc.life > npc.lifeMax / 2) modPlayer.useRegaroth = intense ? 3 : 1;
                else modPlayer.useRegaroth = intense ? 4 : 2;
            }
            else
            {
                // previously only reset when an inactive NPC slot still had Regaroth's type, which is not guaranteed once the slot is reused
                modPlayer.useRegaroth = 0;
            }
            #endregion

            #region Encounter
            _useEncounter1 = ElementsAwoken.encounter == 1;
            _useEncounter2 = ElementsAwoken.encounter == 2;
            _useEncounter3 = ElementsAwoken.encounter == 3;

            modPlayer.useEncounter1 = _useEncounter1;
            modPlayer.useEncounter2 = _useEncounter2;
            modPlayer.useEncounter3 = _useEncounter3;
            #endregion

            return _useAncients || _useAzana || _leviathan || _volcanox || _useGuardian || _useAqueous || _usePermafrost || _useInfernace || _temple
                || _useVoidEvent || _useVoidEventDark || _useRadRain
                || _useEncounter1 || _useEncounter2 || _useEncounter3;
        }
        public override string BestiaryIcon => "ElementsAwoken/Extra/Bestiary/icon_small";
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public class Emptiness : EABiomes
        {
            public override string BackgroundPath => "";
            public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        }
        public class RadiantRainBiome : ModBiome
        {
            public override string BestiaryIcon => "ElementsAwoken/Extra/Bestiary/RadiantRainIcon";
            public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        }
        public class DOTVBiome : ModBiome
        {
            public override string BestiaryIcon => "ElementsAwoken/Extra/Bestiary/DawnOfTheVoidBestiary";
            public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        }
    }
}