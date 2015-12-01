﻿/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.DataModels;

namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// Handles all Passages Facet functionality. 
    /// </summary>
    public class Passages : Facet
    {
        private float m_RectTransformPosX = -555f;
        private float m_RectTransformPosY = -77;
        private float m_RectTransformPosZ = 375f;
        private float m_RectTransformZSpacing = -50f;

        [SerializeField]
        private GameObject m_PassageItemPrefab;
        [SerializeField]
        private RectTransform m_PassageCanvasRectTransform;

        private List<PassageItem> m_PassageItems = new List<PassageItem>();

        private DataModels.XRAY.Answers m_AnswerData = null;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData);
        }

        private void OnAnswerData(object[] args)
        {
            if (Focused )
            {
                if ( m_PassageItemPrefab == null )
                    throw new WatsonException( "m_PassageItemPrefab is null." );
                if ( m_PassageCanvasRectTransform == null )
                    throw new WatsonException( "m_PassageCanvasRectTransform is null." );

                m_AnswerData = args != null && args.Length > 0 ? args[0] as DataModels.XRAY.Answers : null;
                while (m_PassageItems.Count > 0)
                {
                    Destroy(m_PassageItems[0].gameObject);
                    m_PassageItems.RemoveAt( 0 );
                }

                if ( m_AnswerData != null )
                {
                    for (int i = 0; i < m_AnswerData.answers.Length; i++)
                    {
                        if ( string.IsNullOrEmpty( m_AnswerData.answers[i].answerText ))
                            break;

                        Log.Debug("Passages", "adding passage " + i);
                        GameObject PassageItemGameObject = Instantiate(m_PassageItemPrefab, 
                            new Vector3(m_RectTransformPosX, m_RectTransformPosY, m_RectTransformPosZ + m_RectTransformZSpacing * (m_AnswerData.answers.Length - i)), Quaternion.identity) as GameObject;
                        PassageItemGameObject.name = "PassageItem_" + i.ToString("00");
                        RectTransform PassageItemRectTransform = PassageItemGameObject.GetComponent<RectTransform>();
                        PassageItemRectTransform.SetParent(m_PassageCanvasRectTransform, false);
                        PassageItem PassageItem = PassageItemGameObject.GetComponent<PassageItem>();
                        PassageItemRectTransform.pivot = new Vector2(0.0f, 0.5f);   //setting pivot as left middle
                        PassageItemRectTransform.SetAsFirstSibling();
					    PassageItem.PassageString = m_AnswerData.answers[i].evidence.Length > 0 ? "<b><size=27>" + m_AnswerData.answers[i].evidence[0].title + "</size></b>\n\n" + m_AnswerData.answers[i].answerText + "\n\n" : m_AnswerData.answers[i].answerText;
                        PassageItem.MaxConfidence = m_AnswerData.answers[0].confidence;
                        PassageItem.MinConfidence = m_AnswerData.answers[m_AnswerData.answers.Length - 1].confidence;
                        PassageItem.Confidence = m_AnswerData.answers[i].confidence;

                        m_PassageItems.Add(PassageItem);
                    }
                }

                Log.Debug("Passages", "m_PassageItems.count: " + m_PassageItems.Count);
            }
        }
    }
}