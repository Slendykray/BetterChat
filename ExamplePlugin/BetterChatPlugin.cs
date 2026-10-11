using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using R2API;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
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
        public RectTransform defaultRect;
        public RectTransform allyCardRect;

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
        }

  
        public static ConfigEntry<float> playerChatPosY;
        public static ConfigEntry<float> playerChatHeight;

        public static ConfigEntry<float> pingChatPosY;
        public static ConfigEntry<float> pingChatHeight;

        //public static ConfigEntry<float> allyCardPosY;
        //public static ConfigEntry<float> allyCardHeight;
        private void InitConfig()
        {
            Sprite icon = Asset.mainBundle.LoadAsset<Sprite>("icon.png");
            ModSettingsManager.SetModIcon(icon);

           
            playerChatPosY = Config.Bind<float>("PlayerChat", "playerChatPosY", 50f, "default: 50");
            playerChatHeight = Config.Bind<float>("PlayerChat", "playerChatHeight", 170f, "default: 170");


            pingChatPosY = Config.Bind<float>("PingChat", "pingChatPosY", 210f, "default: 210\nvanila: 50");
            pingChatHeight = Config.Bind<float>("PingChat", "pingChatHeight", 140f, "vanila: 140");


            //allyCardPosY = Config.Bind<float>("AllyCard", "allyCardPosY", 210f, "default: 210\nvanila: 50");
            //allyCardHeight = Config.Bind<float>("AllyCard", "allyCardHeight", 140f, "vanila: 140");


            ModSettingsManager.AddOption(new SliderOption(playerChatPosY, new SliderConfig() { min = 50f, max = 250f }));
            ModSettingsManager.AddOption(new SliderOption(playerChatHeight, new SliderConfig() { min = 50f, max = 250f }));

            ModSettingsManager.AddOption(new SliderOption(pingChatPosY, new SliderConfig() { min = 50f, max = 250f }));
            ModSettingsManager.AddOption(new SliderOption(pingChatHeight, new SliderConfig() { min = 50f, max = 250f }));


            //ModSettingsManager.AddOption(new SliderOption(allyCardPosY, new SliderConfig() { min = 50f, max = 250f }));
            //ModSettingsManager.AddOption(new SliderOption(allyCardHeight, new SliderConfig() { min = 50f, max = 250f }));


            playerChatPosY.SettingChanged += (obj, args) => SetShit();
            playerChatHeight.SettingChanged += (obj, args) => SetShit();

            pingChatPosY.SettingChanged += (obj, args) => SetShit();
            pingChatHeight.SettingChanged += (obj, args) => SetShit();


            //allyCardPosY.SettingChanged += (obj, args) => SetShit();
            //allyCardHeight.SettingChanged += (obj, args) => SetShit();
        }

        private void SetShit()
        {
            RectTransform rectTransform = currentChat.transform.GetComponent<RectTransform>();

            rectTransform.anchorMax = new Vector2(1f, 0.45f);

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playerChatHeight.Value);
            rectTransform.anchoredPosition = new Vector2(-155, playerChatPosY.Value);

            defaultRect.anchoredPosition = new Vector2(0, pingChatPosY.Value);
            defaultRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pingChatHeight.Value);

            //allyCardRect.anchoredPosition = new Vector2(0, allyCardPosY.Value);
            //allyCardRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, allyCardHeight.Value);
        }

        private void ChatBox_Start(On.RoR2.UI.ChatBox.orig_Start orig, ChatBox self)
        {
            orig(self);

            if (self.transform.name.Contains("ChatBox, In Run"))
            {
                defaultRect = self.transform.Find("StandardRect").GetComponent<RectTransform>();

                SetShit();
            }
        }

        private void AllyCardManager_Awake(On.RoR2.UI.AllyCardManager.orig_Awake orig, AllyCardManager self)
        {
            orig(self);
            
            //allyCardRect = self.GetComponent<RectTransform>();
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
