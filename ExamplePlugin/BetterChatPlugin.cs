using BepInEx;
using R2API;
using RoR2;
using RoR2.UI;
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



        private readonly Queue<string> messages = new Queue<string>();

        TextMeshProUGUI chatText;

        public static PlayerChat currentChat;

        public void Awake()
        {

            Log.Init(Logger);

            Asset.Init();

            //On.RoR2.Chat.SendBroadcastChat_ChatMessageBase += Chat_SendBroadcastChat_ChatMessageBase;
            On.RoR2.Chat.AddMessage_ChatMessageBase += Chat_AddMessage_ChatMessageBase;

            On.RoR2.UI.HUD.Awake += HUD_Awake;

            //On.RoR2.UI.AllyCardController.Awake += AllyCardController_Awake;
            On.RoR2.UI.AllyCardManager.Awake += AllyCardManager_Awake;

        }




        private void AllyCardManager_Awake(On.RoR2.UI.AllyCardManager.orig_Awake orig, AllyCardManager self)
        {
            orig(self);

            var rect = self.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.11f, 0.43f);
                //rect.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
        }

        private void AllyCardController_Awake(On.RoR2.UI.AllyCardController.orig_Awake orig, AllyCardController self)
        {
            
        }

        private void Chat_AddMessage_ChatMessageBase(On.RoR2.Chat.orig_AddMessage_ChatMessageBase orig, ChatMessageBase message)
        {
            if (message is Chat.UserChatMessage)
            {

                if (currentChat)
                {
                    currentChat.AddMessage(message.ConstructChatString());


                    //messages.Enqueue(message.ConstructChatString());

                    //chatText.text = string.Join("\n", messages);

                    //Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
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

            GameObject chatObj = UnityEngine.Object.Instantiate<GameObject>(pref, self.mainContainer.transform);

            Transform textObj = chatObj.transform.Find("Viewport/Content");
            //if (textObj)
            //{
            //    Log.Info("cool");
            //    chatText = textObj.GetComponent<TextMeshProUGUI>();
            //}


            RectTransform rectTransform = chatObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.118f, 0.45f);
            rectTransform.anchorMax = new Vector2(0.3f, 0.45f);

            rectTransform.sizeDelta = new Vector2(200f, 150f);
            //rectTransform.anchorMax = new Vector2(0.3f, 0.5f);

            currentChat = chatObj.AddComponent<PlayerChat>();

            //     GameObject myObject = new GameObject("GameObjectName");
            //     myObject.transform.SetParent(self.mainContainer.transform);
            //     RectTransform rectTransform = myObject.AddComponent<RectTransform>();
            //     rectTransform.anchorMin = new Vector2(0.05f, 0.35f);
            //     rectTransform.anchorMax = new Vector2(0.3f, 0.5f);
            //     rectTransform.sizeDelta = Vector2.zero;
            //     rectTransform.anchoredPosition = Vector2.zero;



            //     var background = scrollObject.AddComponent<Image>();
            //     background.color = new Color(0f, 0f, 0f, 0.5f);




        //    var textObject = new GameObject(
        //    "ChatText",
        //    typeof(RectTransform),
        //    typeof(TextMeshProUGUI)
        //);


        //    textObject.transform.SetParent(textObj.transform, false);

        //    textObject.GetComponent<TextMeshProUGUI>().text = "sasite";
        //    chatText = textObject.GetComponent<TextMeshProUGUI>();
        //    chatText.fontSize = 20f;
        }



        private void Chat_SendBroadcastChat_ChatMessageBase(On.RoR2.Chat.orig_SendBroadcastChat_ChatMessageBase orig, ChatMessageBase message)
        {

            

            if (message is Chat.UserChatMessage)
            {
             
                if (currentChat)
                {
                    currentChat.AddMessage(message.ConstructChatString());


                    //messages.Enqueue(message.ConstructChatString());

                    //chatText.text = string.Join("\n", messages);

                    //Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
                }
            }
            else
            {
                orig(message);
            }
            
        }

        


    }
}
