using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterChat
{
    public class PlayerChat : MonoBehaviour
    {

        private int maxMessages = 30;
        private readonly Queue<string> messages = new Queue<string>();
        private TextMeshProUGUI chatText;
        public ScrollRect scrollRect;
        private bool shouldAutoScroll = true;

        void Start()
        {
            Transform textContainer = transform.Find("Viewport/Content");

            var textObject = new GameObject(
           "ChatText",
           typeof(RectTransform),
           typeof(TextMeshProUGUI)
       );


            textObject.transform.SetParent(textContainer.transform, false);

            chatText = textObject.GetComponent<TextMeshProUGUI>();
            chatText.fontSize = 20f;

            scrollRect = GetComponent<ScrollRect>();

            scrollRect.onValueChanged.AddListener(OnScroll);


            fadeGroup = transform.GetComponent<CanvasGroup>();
        }

        void OnScroll(Vector2 position)
        {
            shouldAutoScroll = position.y <= 0.25f;
        }

        public void AddMessage(string message)
        {
            messages.Enqueue(message);

            while (messages.Count > maxMessages)
                messages.Dequeue();

            chatText.text = string.Join("\n", messages);

            Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);

            if (shouldAutoScroll)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
                scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                scrollRect.verticalNormalizedPosition = 0;
            }

            this.ResetFadeTimer();

        }

        private float fadeTimer;
        public CanvasGroup fadeGroup;
        public float fadeDuration = 5f;
        public float fadeWait = 5f;
        private float alpha;
        private void UpdateFade(float deltaTime)
        {
            this.fadeTimer -= deltaTime;
            if (!this.fadeGroup)
            {
                return;
            }
            if (ChatBox.instance && ChatBox.instance.showInput)
            {
                this.ResetFadeTimer();
            }
            else if (this.fadeTimer < 0f)
            {
                alpha = 0f;
            }
            else if (this.fadeTimer < this.fadeDuration)
            {
                alpha = Mathf.Sqrt(Util.Remap(this.fadeTimer, this.fadeDuration, 0f, 1f, 0f));
            }
            else
            {
                alpha = 1f;
            }
            this.fadeGroup.alpha = alpha;
        }

        private void ResetFadeTimer()
        {
            alpha = 1f;
            this.fadeTimer = this.fadeDuration + this.fadeWait;
        }

        private void Update()
        {
            UpdateFade(Time.deltaTime);
        }


       


    }
}
