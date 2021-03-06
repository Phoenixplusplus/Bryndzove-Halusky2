﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_TextBlockSizer : MonoBehaviour {

    public void UpdateWidth(bool IsLobbyChat)
    {
        RectTransform rectTransform = this.GetComponent<RectTransform>();

        // NOT ALL THE WIDTH POSITIONS ARE RIGHT ! THE RIGHT POSITIONS ARE ONLY FOR RESOLUTIONS 800,1024 and 1920
        // OTHER RESOLUTIONS ARE NOT TESTED, THE TEXT CONTENT WILL ON 90% NOT SUIT THE CHAT CONTENT PROPERLY !!
        // IT MEAN THAT TEXT WILL GO OUT OF THE CHAT OR WILL START NEXT LINE NOT IN THE END EDGE OF THE CHAT
        if (IsLobbyChat)
        {
            switch (Screen.width)
            {
                case 800: rectTransform.sizeDelta = new Vector2(307.0f, rectTransform.sizeDelta.y); break;
                case 1024: rectTransform.sizeDelta = new Vector2(400.0f, rectTransform.sizeDelta.y); break;
                case 1152: rectTransform.sizeDelta = new Vector2(460.0f, rectTransform.sizeDelta.y); break;
                case 1360: rectTransform.sizeDelta = new Vector2(500.0f, rectTransform.sizeDelta.y); break;
                case 1366: rectTransform.sizeDelta = new Vector2(500.0f, rectTransform.sizeDelta.y); break;
                case 1400: rectTransform.sizeDelta = new Vector2(560.0f, rectTransform.sizeDelta.y); break;
                case 1440: rectTransform.sizeDelta = new Vector2(560.0f, rectTransform.sizeDelta.y); break;
                case 1600: rectTransform.sizeDelta = new Vector2(560.0f, rectTransform.sizeDelta.y); break;
                case 1680: rectTransform.sizeDelta = new Vector2(560.0f, rectTransform.sizeDelta.y); break;
                case 1920: rectTransform.sizeDelta = new Vector2(763.0f, rectTransform.sizeDelta.y); break;
            }
        }
        else
        {
            switch (Screen.width)
            {
                case 800: rectTransform.sizeDelta = new Vector2(180.0f, rectTransform.sizeDelta.y); break;
                case 1024: rectTransform.sizeDelta = new Vector2(230.0f, rectTransform.sizeDelta.y); break;
                case 1152: rectTransform.sizeDelta = new Vector2(290.0f, rectTransform.sizeDelta.y); break;
                case 1360: rectTransform.sizeDelta = new Vector2(310.0f, rectTransform.sizeDelta.y); break;
                case 1366: rectTransform.sizeDelta = new Vector2(315.0f, rectTransform.sizeDelta.y); break;
                case 1400: rectTransform.sizeDelta = new Vector2(324.0f, rectTransform.sizeDelta.y); break;
                case 1440: rectTransform.sizeDelta = new Vector2(333.0f, rectTransform.sizeDelta.y); break;
                case 1600: rectTransform.sizeDelta = new Vector2(375.0f, rectTransform.sizeDelta.y); break;
                case 1680: rectTransform.sizeDelta = new Vector2(380.0f, rectTransform.sizeDelta.y); break;
                case 1920: rectTransform.sizeDelta = new Vector2(450.0f, rectTransform.sizeDelta.y); break;
            }
        }
    }
}
