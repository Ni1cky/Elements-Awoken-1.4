using ElementsAwoken.EASystem.EAPlayer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ElementsAwoken.EASystem.UI.UIIIII
{
    class UISystemSettings : ModSystem
    {
        public static bool Panel = false;
        public override void ClearWorld()
        {
            Panel = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            if (Panel) tag["panel"] = Panel;
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Panel);
        }
        public override void NetReceive(BinaryReader reader)
        {
            Panel = reader.ReadBoolean();
        }
        public override void LoadWorldData(TagCompound tag)
        {
            Panel = tag.GetBool("panel");
        }
        private GameTime _lastUpdateUiGameTime;
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("UIIIII: UI", delegate
                {
                    if (_lastUpdateUiGameTime != null && MyInterface?.CurrentState != null)
                    {
                        MyInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            // Panel is a world flag (boss prompts have been activated in this world). Whether this character has already
            // closed the info popup is stored per player, so closing it is not undone by the next world sync in multiplayer.
            bool showPromptInfo = Panel && !Main.gameMenu
                && !ModContent.GetInstance<ServerConfig>().promptsDisabled
                && !Main.LocalPlayer.GetModPlayer<BossPromptInfoPlayer>().seenBossPromptInfo;
            if (showPromptInfo)
            {
                if (MyInterface?.CurrentState == null)
                {
                    MyInterface?.SetState(MyUI);
                }
            }
            else
            {
                if (MyInterface?.CurrentState != null)
                {
                    MyInterface?.SetState(null);
                }
            }
            MyInterface?.Update(gameTime);
        }

        internal UserInterface MyInterface;
        internal Penal MyUI;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                MyInterface = new UserInterface();

                MyUI = new Penal();
                MyUI.Activate();
            }
        }
    }
}