using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleChatbox : MonoBehaviour {

    public InputField Textbox;
    public ScrollRect Messagebox;
    public Text text;
    public bool Chatting;

    // Use this for initialization
    void Start () {
		
	}

    void AppendMessage(string message) {
        //Messagebox.content
    }
	
	// Update is called once per frame
    void Update() {
        if (Input.GetButtonUp("Enter")) {
            if (Chatting) {
                AppendMessage(Textbox.text);
            }

            Chatting = !Chatting;
        }
		
        if (Chatting && !Textbox.isFocused)
            Textbox.Select();
        else
            Textbox.OnDeselect(new BaseEventData(EventSystem.current));
	}
}
