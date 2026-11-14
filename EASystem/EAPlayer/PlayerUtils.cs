using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social;

namespace ElementsAwoken.EASystem
{
    public class PlayerUtils : ModPlayer
    {
        private float playerBrightestLight = 0;
        public float playerLight = 0;
        public float playerMaxLight = 1.3f;

        public int potionsConsumedLastMin = 0;
        public int salesLastMin = 0;
        public int buysLastMin = 0;
        public int bossesKilledLastMin = 0;
        public int bossesKilledLastFiveMin = 0;
        public int enemiesKilledLastMin = 0;
        public int enemiesKilledLast10Secs = 0;

        // Recent events are stored as the update count at which they happened, and dropped once they are older than
        // the longest window they are counted in. This replaces fixed-size countdown arrays (up to 9999 entries each)
        // that were fully scanned every tick for every player, even when empty.
        private const uint TenSeconds = 600;
        private const uint OneMinute = 3600;
        private const uint FiveMinutes = 18000;
        private uint updateCount = 0;
        private readonly Queue<uint> potionConsumedTimes = new();
        private readonly Queue<uint> saleTimes = new();
        private readonly Queue<uint> buyTimes = new();
        private readonly Queue<uint> bossKillTimes = new();
        private readonly Queue<uint> enemyKillTimes = new();

        public bool pressingQuickBuff = false;
        public int pressingQuickBuffCD = 0;

        public int placingAutoDriller = 0;
        public override void ResetEffects()
        {
            playerLight = playerBrightestLight;
            playerBrightestLight = 0;

            potionsConsumedLastMin = 0;
            bossesKilledLastMin = 0;
            bossesKilledLastFiveMin = 0;
            salesLastMin = 0;
            buysLastMin = 0;
            enemiesKilledLast10Secs = 0;
            enemiesKilledLastMin = 0;
        }
        public override void PreUpdate()
        {
            #region check if pressing the buff key, not needed anymore
            bool flag = false;
            bool flag2 = false;
            Keys[] pressedKeys = Main.keyState.GetPressedKeys();
            for (int k = 0; k < pressedKeys.Length; k++)
            {
                if (pressedKeys[k] == Keys.LeftShift || pressedKeys[k] == Keys.RightShift)
                {
                    flag = true;
                }
                else if (pressedKeys[k] == Keys.LeftAlt || pressedKeys[k] == Keys.RightAlt)
                {
                    flag2 = true;
                }

                string a = string.Concat(pressedKeys[k]);
                if (pressedKeys[k] != Keys.Tab || !((flag && SocialAPI.Mode == SocialMode.Steam) | flag2))
                {
                    if (a == Main.cBuff)
                    {
                        pressingQuickBuff = true;
                        pressingQuickBuffCD = 1;
                    }
                }
            }
            if (pressingQuickBuff)
            {
                QuickBuff();
            }
            pressingQuickBuffCD--;
            if (pressingQuickBuffCD <= 0)
            {
                pressingQuickBuff = false;
            }
            #endregion

            #region darkness
            // Lighting is not available on a dedicated server.
            if (!Main.dedServ)
            {
                Point topLeft = ((Player.TopLeft) / 16).ToPoint();
                Point bottomRight = ((Player.BottomRight) / 16).ToPoint();

                for (int i = topLeft.X; i <= bottomRight.X; i++)
                {
                    for (int j = topLeft.Y; j <= bottomRight.Y; j++)
                    {
                        if (Lighting.Brightness(i, j) > playerBrightestLight)
                        {
                            playerBrightestLight = Lighting.Brightness(i, j);
                        }
                    }
                }
            }
            #endregion

            if (placingAutoDriller > 0)
            {
                placingAutoDriller--;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            updateCount++;
            potionsConsumedLastMin = CountRecent(potionConsumedTimes, OneMinute, OneMinute);
            salesLastMin = CountRecent(saleTimes, OneMinute, OneMinute);
            buysLastMin = CountRecent(buyTimes, OneMinute, OneMinute);
            bossesKilledLastFiveMin = CountRecent(bossKillTimes, FiveMinutes, FiveMinutes);
            bossesKilledLastMin = CountRecent(bossKillTimes, OneMinute, FiveMinutes);
            enemiesKilledLastMin = CountRecent(enemyKillTimes, OneMinute, OneMinute);
            enemiesKilledLast10Secs = CountRecent(enemyKillTimes, TenSeconds, OneMinute);
        }
        /// <summary>
        /// Drops events older than <paramref name="keepWindow"/> and returns how many happened within the last <paramref name="window"/> updates.
        /// </summary>
        private int CountRecent(Queue<uint> times, uint window, uint keepWindow)
        {
            while (times.Count > 0 && updateCount - times.Peek() > keepWindow)
            {
                times.Dequeue();
            }
            int count = 0;
            foreach (uint time in times)
            {
                if (updateCount - time <= window)
                {
                    count++;
                }
            }
            return count;
        }
        private void QuickBuff() { }
        public bool CheckValidInvPotion(int invSlot)
        {
            if (Player.CountBuffs() == 22)
            {
                return false;
            }
            if (Player.inventory[invSlot].stack > 0 && Player.inventory[invSlot].type > 0 && Player.inventory[invSlot].buffType > 0 && Player.inventory[invSlot].consumable)
            {
                int num = Player.inventory[invSlot].buffType;
                for (int j = 0; j < 22; j++)
                {
                    if (Player.buffType[j] == num)
                    {
                        return false;
                    }
                    if (Main.meleeBuff[num] && Main.meleeBuff[Player.buffType[j]])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RecordPotionConsumed()
        {
            potionConsumedTimes.Enqueue(updateCount);
        }
        public void RecordBossKill()
        {
            bossKillTimes.Enqueue(updateCount);
            ElementsAwoken.DebugModeText("boss killed");
            ElementsAwoken.DebugModeText("bosses killed last min: " + (bossesKilledLastMin + 1));
            ElementsAwoken.DebugModeText("bosses killed last five min: " + (bossesKilledLastFiveMin + 1));
        }
        public void RecordEnemyKill()
        {
            enemyKillTimes.Enqueue(updateCount);
        }
        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            saleTimes.Enqueue(updateCount);
            ElementsAwoken.DebugModeText("sales last min: " + (salesLastMin + 1));
        }
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            buyTimes.Enqueue(updateCount);
            ElementsAwoken.DebugModeText("buys last min: " + (buysLastMin + 1));
        }
    }

    class PlayerUtilsItem : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            PlayerUtils modPlayer = player.GetModPlayer<PlayerUtils>();

            if (item.buffType != 0 && item.useStyle == 2 && item.consumable && item.type != ItemID.Ale)
            {
                modPlayer.RecordPotionConsumed();
                ElementsAwoken.DebugModeText("potions consumed last min: " + (modPlayer.potionsConsumedLastMin + 1));
            }
            return base.UseItem(item, player);
        }
    }
    class PlayerUtilsNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            // OnKill only runs in singleplayer or on the server.
            // Boss kills are recorded for every player by SanityNPC (AwakenedPlayer.SanityEventType.BossKilled).
            if (!npc.boss && !npc.friendly && !npc.SpawnedFromStatue)
            {
                // The kill is credited to the player who last hit the NPC.
                int killer = Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : npc.lastInteraction;
                if (AwakenedPlayer.IsValidPlayer(killer))
                {
                    AwakenedPlayer.SendSanityEvent(AwakenedPlayer.SanityEventType.EnemyKilled, killer);
                }
            }
        }
    }
}