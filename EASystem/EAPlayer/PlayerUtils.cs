using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        public int[] potionConsumedCD = new int[60];

        public int salesLastMin = 0;
        public int[] salesCD = new int[9999];

        public int buysLastMin = 0;
        public int[] buysCD = new int[9999];

        public int bossesKilledLastMin = 0;
        public int bossesKilledLastFiveMin = 0;
        public int[] bossKilledCD = new int[999];

        public bool pressingQuickBuff = false;
        public int pressingQuickBuffCD = 0;

        public int enemiesKilledLastMin = 0;
        public int enemiesKilledLast10Secs = 0;
        public int[] enemiesKilledCD = new int[999];

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
            for (int i = 0; i < potionConsumedCD.Length; i++)
            {
                if (potionConsumedCD[i] > 0)
                {
                    potionsConsumedLastMin++;
                    potionConsumedCD[i]--;
                }
            }
            for (int i = 0; i < salesCD.Length; i++)
            {
                if (salesCD[i] > 0)
                {
                    salesLastMin++;
                    salesCD[i]--;
                }
            }
            for (int i = 0; i < buysCD.Length; i++)
            {
                if (buysCD[i] > 0)
                {
                    buysLastMin++;
                    buysCD[i]--;
                }
            }
            for (int i = 0; i < bossKilledCD.Length; i++)
            {
                if (bossKilledCD[i] > 14400)
                {
                    bossesKilledLastMin++;
                }
                if (bossKilledCD[i] > 0)
                {
                    bossesKilledLastFiveMin++;
                    bossKilledCD[i]--;
                }
            }
            for (int i = 0; i < enemiesKilledCD.Length; i++)
            {
                if (enemiesKilledCD[i] > 3600 - 600)
                {
                    enemiesKilledLast10Secs++;
                }
                if (enemiesKilledCD[i] > 0)
                {
                    enemiesKilledLastMin++;
                    enemiesKilledCD[i]--;
                }
            }
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
        public void RecordBossKill()
        {
            for (int i = 0; i < bossKilledCD.Length; i++)
            {
                if (bossKilledCD[i] <= 0)
                {
                    bossKilledCD[i] = 18000;
                    break;
                }
            }
            ElementsAwoken.DebugModeText("boss killed");
            ElementsAwoken.DebugModeText("bosses killed last min: " + (bossesKilledLastMin + 1));
            ElementsAwoken.DebugModeText("bosses killed last five min: " + (bossesKilledLastFiveMin + 1));
        }
        public void RecordEnemyKill()
        {
            for (int i = 0; i < enemiesKilledCD.Length; i++)
            {
                if (enemiesKilledCD[i] <= 0)
                {
                    enemiesKilledCD[i] = 3600;
                    break;
                }
            }
        }
        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            for (int i = 0; i < salesCD.Length; i++)
            {
                if (salesCD[i] <= 0)
                {
                    salesCD[i] = 3600;
                    break;
                }
            }
            ElementsAwoken.DebugModeText("sales last min: " + (salesLastMin + 1));
        }
        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            for (int i = 0; i < buysCD.Length; i++)
            {
                if (buysCD[i] <= 0)
                {
                    buysCD[i] = 3600;
                    break;
                }
            }
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
                for (int i = 0; i < modPlayer.potionConsumedCD.Length; i++)
                {
                    if (modPlayer.potionConsumedCD[i] <= 0)
                    {
                        modPlayer.potionConsumedCD[i] = 3600;
                        break;
                    }
                }
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