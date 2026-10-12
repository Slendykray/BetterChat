using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace BetterChat
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BetterChatPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Slendykray";
        public const string PluginName = "BetterChat";
        public const string PluginVersion = "1.0.0";

        public static PlayerChat currentChat;
        public static ChatBox defaultChatbox;


        public void Awake()
        {
            Log.Init(Logger);

            Asset.Init();

            InitConfig();
;
            On.RoR2.Chat.AddMessage_ChatMessageBase += Chat_AddMessage_ChatMessageBase;

            On.RoR2.UI.HUD.Awake += HUD_Awake;

            On.RoR2.UI.AllyCardManager.Awake += AllyCardManager_Awake;

            On.RoR2.UI.ChatBox.Start += ChatBox_Start;

            On.RoR2.UI.ChatBox.Awake += ChatBox_Awake;
        }

        Vector2 defaultOffsetMax;
        Vector2 defaultOffsetMin;
        private void ChatBox_Awake(On.RoR2.UI.ChatBox.orig_Awake orig, ChatBox self)
        {
            if (self.transform.name.Contains("ChatBox, In Run"))
            {
                defaultOffsetMax = self.standardChatboxRect.offsetMax;
                defaultOffsetMin = self.standardChatboxRect.offsetMin;
            }
         
            orig(self);
        }


        public static ConfigEntry<float> playerChatPosY;
        public static ConfigEntry<float> playerChatHeight;
        public static ConfigEntry<float> defaultChatPosY;
        public static ConfigEntry<float> defaultChatHeight;

        private void InitConfig()
        {
            Sprite icon = Asset.mainBundle.LoadAsset<Sprite>("icon.png");
            ModSettingsManager.SetModIcon(icon);


            playerChatPosY = Config.Bind<float>("PlayerChat", "playerChatPosY", 50f, "default: 50");
            playerChatHeight = Config.Bind<float>("PlayerChat", "playerChatHeight", 170f, "default: 170");

            defaultChatPosY = Config.Bind<float>("DefaultChat", "defaultChatPosY", 210f, "default: 210");
            defaultChatHeight = Config.Bind<float>("DefaultChat", "defaultChatHeight", 140f, "default: 140");

            ModSettingsManager.AddOption(new SliderOption(playerChatPosY, new SliderConfig() { min = 40f, max = 260f }));
            ModSettingsManager.AddOption(new SliderOption(playerChatHeight, new SliderConfig() { min = 40f, max = 260f }));

            ModSettingsManager.AddOption(new SliderOption(defaultChatPosY, new SliderConfig() { min = 40f, max = 260f }));
            ModSettingsManager.AddOption(new SliderOption(defaultChatHeight, new SliderConfig() { min = 40, max = 260f }));

            playerChatPosY.SettingChanged += (obj, args) => SetShit();
            playerChatHeight.SettingChanged += (obj, args) => SetShit();

            defaultChatPosY.SettingChanged += (obj, args) => SetShit();
            defaultChatHeight.SettingChanged += (obj, args) => SetShit();
        }

        private void SetShit()
        {
            RectTransform defaultRect = defaultChatbox.standardChatboxRect;

            RectTransform playerRect = currentChat.transform.GetComponent<RectTransform>();

            playerRect.anchorMax = new Vector2(1f, 0.45f);

            //anti wolfo qol
            defaultRect.offsetMax = defaultOffsetMax;
            defaultRect.offsetMin = defaultOffsetMin;
            defaultChatbox.allowExpandedChatbox = false;


            playerRect.anchoredPosition = new Vector2(-155, playerChatPosY.Value);
            defaultRect.anchoredPosition = new Vector2(0, defaultChatPosY.Value);

            playerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playerChatHeight.Value);
            defaultRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultChatHeight.Value);
        }

        private void ChatBox_Start(On.RoR2.UI.ChatBox.orig_Start orig, ChatBox self)
        {
            orig(self);

            if (self.transform.name.Contains("ChatBox, In Run"))
            {
                defaultChatbox = self;

                SetShit();
            }
        }

        private void AllyCardManager_Awake(On.RoR2.UI.AllyCardManager.orig_Awake orig, AllyCardManager self)
        {
            orig(self);
            
            var rect = self.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.11f, 0.45f);
            }
        }


        private void Chat_AddMessage_ChatMessageBase(On.RoR2.Chat.orig_AddMessage_ChatMessageBase orig, ChatMessageBase message)
        {
            if (message is Chat.UserChatMessage && ChatBox.instance.transform.name.Contains("ChatBox, In Run"))
            {
                if (currentChat)
                {
                    currentChat.AddMessage(message.ConstructChatString());
                }
            }
            else
            {
                orig(message);
            }
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);


            GameObject pref = Asset.mainBundle.LoadAsset<GameObject>("Chat");

            Transform chatParent = self.mainContainer.transform.Find("MainUIArea/SpringCanvas/BottomLeftCluster/ChatBoxRoot");

            GameObject chatObj = UnityEngine.Object.Instantiate<GameObject>(pref, chatParent);

            currentChat = chatObj.AddComponent<PlayerChat>();

        }


    }
}
