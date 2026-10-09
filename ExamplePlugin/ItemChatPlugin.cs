using BepInEx;
using R2API;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ItemChat
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ItemChatPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Slendykray";
        public const string PluginName = "ItemChat";
        public const string PluginVersion = "1.0.0";



        private readonly Queue<string> messages = new Queue<string>();

        TextMeshProUGUI chatText;

        public void Awake()
        {

            Log.Init(Logger);

            On.RoR2.Chat.SendBroadcastChat_ChatMessageBase += Chat_SendBroadcastChat_ChatMessageBase;

            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);

            GameObject myObject = new GameObject("GameObjectName");
            myObject.transform.SetParent(self.mainContainer.transform);
            RectTransform rectTransform = myObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;


            var scrollObject = new GameObject(
           "ScrollView",
           typeof(RectTransform),
           typeof(ScrollRect)
       );

            scrollObject.transform.SetParent(rectTransform.transform, false);

            var textObject = new GameObject(
            "ChatText",
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

            textObject.transform.SetParent(scrollObject.transform, false);

            textObject.GetComponent<TextMeshProUGUI>().text = "sasite";
            chatText = textObject.GetComponent<TextMeshProUGUI>();

        }



        private void Chat_SendBroadcastChat_ChatMessageBase(On.RoR2.Chat.orig_SendBroadcastChat_ChatMessageBase orig, ChatMessageBase message)
        {

            

            if (message is Chat.UserChatMessage)
            {
              

                if (chatText)
                {
                    messages.Enqueue(message.ConstructChatString());

                    chatText.text = string.Join("\n", messages);

                    Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
                }
            }
            else
            {
                orig(message);
            }
            
        }

        


    }
}
