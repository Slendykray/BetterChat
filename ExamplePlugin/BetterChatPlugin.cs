using BepInEx;
using HarmonyLib;
using R2API;
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
        public const string PluginName = "ItemChat";
        public const string PluginVersion = "1.0.0";

        public static PlayerChat currentChat;

        public void Awake()
        {
            Log.Init(Logger);

            Asset.Init();
;
            On.RoR2.Chat.AddMessage_ChatMessageBase += Chat_AddMessage_ChatMessageBase;

            On.RoR2.UI.HUD.Awake += HUD_Awake;

            On.RoR2.UI.AllyCardManager.Awake += AllyCardManager_Awake;
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



            RectTransform rectTransform = chatObj.GetComponent<RectTransform>();

          
            rectTransform.anchoredPosition = new Vector2(-155, 210);

            rectTransform.anchorMax = new Vector2(1f, 0.45f);


            currentChat = chatObj.AddComponent<PlayerChat>();
          
        }

    }
}
