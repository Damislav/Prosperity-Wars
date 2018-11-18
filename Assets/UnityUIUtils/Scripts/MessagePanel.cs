﻿using System;
using Nashet.EconomicSimulation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nashet.UnityUIUtils
{
    [RequireComponent(typeof(DragPanel))]
    internal class MessagePanel : MonoBehaviour, IDragHandler
    {
        ///<summary>Stores position of top-level message window. Used to correctly place next message window</summary>
        private static Vector3 previousWindowLastPosition;

        [SerializeField]
        private Text caption, message, closeText;

        [SerializeField]
        private Toggle showDefeatingAttackerMessage;      

        private MainCamera mainCamera;

        private static int howMuchPausedWindowsOpen;
        private Message messageSource;

        protected DragPanel dragPanel;

        protected static bool firstLaunch = true;

        ///<summary>How much shifts window if there is more than 1 window</summary>
        protected Vector3 offset = new Vector2(-10f, 30f);

        public void OnDrag(PointerEventData data) // need it to place windows in stair-order
        {
            previousWindowLastPosition = transform.localPosition;
        }

        /// <summary>
        /// From UI
        /// </summary>        
        public void OnShowMessagesChanged(bool value)
        {
            Message.SetShowDefeatingAttackersMessages(value);
        }

        public void OnFocusClicked()
        {
            if (messageSource.HasFocus)
                mainCamera.FocusOnPoint(messageSource.getFocus());
        }

        public void Show(Message mess, MainCamera mainCamera, int howMuchShift)
        {
            this.mainCamera = mainCamera;
            howMuchPausedWindowsOpen++;
            caption.text = mess.GetCaption();
            message.text = mess.GetText();
            closeText.text = mess.GetClosetext();
            messageSource = mess;

            dragPanel = GetComponent<DragPanel>();
            dragPanel.Hidden += OnHidden;
            GUIChanger.Apply(gameObject);
            showDefeatingAttackerMessage.isOn = Message.ShowDefeatingAttackersMessages;

            if (firstLaunch)
            {
                var rect = GetComponent<RectTransform>();
                rect.transform.position = new Vector3((Screen.width - rect.sizeDelta.x) / 2, (Screen.height - rect.sizeDelta.y) / 2, rect.position.z);
                previousWindowLastPosition = transform.localPosition;
            }
            else
            {
                transform.localPosition = previousWindowLastPosition - offset * howMuchShift;
            }
            
            firstLaunch = false;
            dragPanel.Show();
        }

        private void OnHidden(Hideable eventData)
        {
            Hide();
        }

        public void Hide()
        {           
            howMuchPausedWindowsOpen--;
            //previousWindowLastPosition = transform.localPosition;
            Destroy(gameObject);
        }

        public static bool IsOpenAny()
        {
            return howMuchPausedWindowsOpen > 0;
        }
    }
}