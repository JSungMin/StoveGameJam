using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game;
using CoreSystem.Game.Dialogue.Behaviour;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystem.Game.Dialogue
{
    public class ScriptObject : MonoBehaviour, ICallListener
    {
        [HideInInspector]
        public TrackManager trackManager;
        public PortraitManager PortraitManager => trackManager.portraitManager;
        public BaseScript script;
        public DialogueActionVM actionVm;
        public GameActor stagedActor;
        
        //  대사 출력에 필요한 프로퍼티 영역
        public GameObject panelObj;
        public RectTransform panelRect;
        public Image panelImage;

        public Text nameText;
        public Text contentText;
        //  대사 출력 및 오브젝트에 사용되는 각종 속도조절 변수
        public float playSpeed = 1.0f;
        public float typeDuration = 0.1f;
        private int skipMode = 0;
        
        public static readonly float DefaultDuration = 0.1f;
        public static readonly float SkipDuration = 0.001f;

        public FadeController fadeController;

        public BehaviourJob printJob;
        public BehaviourJob FadeJob => fadeController.job;

        #region Utility Funcs
        public void Activate()
        {
            panelObj.SetActive(true);
        }
        public void UnActivate()
        {
            panelObj.SetActive(false);
        }

        public void SetSkip()
        {
            if (skipMode == 0)
            {
                typeDuration = SkipDuration;
                playSpeed = 100f;
                skipMode = 1;
            }
            else if (skipMode == 1)
            {
                skipMode = 2;
            }
        }

        public void SetDefault()
        {
            typeDuration = DefaultDuration;
            playSpeed = 1f;
            skipMode = 0;
        }
        public void StartPrint()
        {
            actionVm.OnEventInvoke(this, "OnPlay");
        }

        public void PausePrint()
        {
            actionVm.OnEventInvoke(this, "OnPause");
            playSpeed = 0f;
        }

        public void ResumePrint()
        {
            actionVm.OnEventInvoke(this, "OnResume");
            if (skipMode == 0)
                SetDefault();
        }

        public void Stop()
        {
            actionVm.OnEventInvoke(this, "OnStop");
        }
        
        private BehaviourJob PrintText()
        {
            SetDefault();
            contentText.text = "";
            printJob = BehaviourJob.Make(IPrintText(contentText, script.content));
            printJob.OnComplete += OnPrintComplete;
            return printJob;
        }

        private BehaviourJob FadePanel(Color bColor, Color eColor, float duration)
        {
            fadeController.Stop();
            fadeController.endColor = eColor;
            fadeController.duration = duration;
            fadeController.affectionGroup.Add(panelRect);
            fadeController.Play(OnFadeComplete);
            return FadeJob;
        }

        private BehaviourJob FadeText(Color eColor, float duration)
        {
            fadeController.Stop();
            fadeController.endColor = eColor;
            fadeController.duration = duration;
            fadeController.affectionGroup.Add(nameText.rectTransform);
            fadeController.affectionGroup.Add(contentText.rectTransform);
            fadeController.Play(OnFadeComplete);
            return FadeJob;
        }

        private BehaviourJob FadeAll(Color eColor, float duration)
        {
            fadeController.Stop();
            fadeController.endColor = eColor;
            fadeController.duration = duration;
            fadeController.useRGB = false;
            fadeController.affectionGroup.Add(panelRect);
            fadeController.affectionGroup.Add(nameText.rectTransform);
            fadeController.affectionGroup.Add(contentText.rectTransform);
            fadeController.Play(OnFadeComplete);
            return FadeJob;
        }

        private void SetPanelAlpha(float a)
        {
            UIAlphaController.SetAlphaColor<MaskableGraphic>(panelImage.rectTransform, a);
        }
        public bool IsPrinting => (null != printJob) && printJob.IsRunning;
        
        #endregion
        #region Callback Funcs Region
        public void OnPrintComplete(bool comp)
        {
            trackManager.OnPrintComplete(this, comp);
        }

        public void OnFadeComplete(bool comp)
        {

        }
        public BehaviourJob OnCall(string methodName, List<TaggedData> parameters)
        {
            if (methodName == "UnActivate")
            {
                UnActivate();
            }
            else if (methodName == "Activate")
            {
                Activate();
            }
            else if (methodName == "PushPortrait")
            {
                var actorName = parameters[0].StringData;
                var actor = ActorProfileSet.GetElement(actorName);
                var descName = parameters[1].StringData;
                var pivot =  (PortraitManager.PivotType)parameters[2].IntData;
                var portrait = PortraitManager.FindPortrait(actor);
                if (null == portrait)
                    PortraitManager.AddPortrait(actor, descName, pivot);
                else
                {
                    var desc = actor.GetPortraitDesc(descName);
                    PortraitManager.SetToTalker(actor, desc, pivot);
                }
            }
            else if (methodName == "PopPortrait")
            {
                var actorName = parameters[0].StringData;
                var actor = ActorProfileSet.GetElement(actorName);
                var descName = parameters[1].StringData;
                var portrait = PortraitManager.FindPortrait(actor);
                if (null != portrait)
                    PortraitManager.RemovePortrait(portrait);
            }
            else if (methodName == "ShakePortrait")
            {
                var actorName = parameters[0].StringData;
                var actor = ActorProfileSet.GetElement(actorName);
                var index = parameters[1].IntData;
                var amp = parameters[2].FloatData;
                var freq = parameters[3].FloatData;
                var duration = parameters[4].FloatData;
                PortraitManager.ShakePortrait(actor,index,amp,freq,duration,null);
            }
            else if (methodName == "SetPanelAlpha")
            {
                SetPanelAlpha(parameters[0].FloatData);
            }
            else if (methodName == "FadeIn")
            {
                Color endColor = Color.white;
                return FadeAll(endColor, parameters[0].FloatData);
            }
            else if (methodName == "FadeOut")
            {
                Color endColor = new Color(0, 0, 0, 0);
                return FadeAll(endColor, parameters[0].FloatData);
            }
            else if (methodName == "PrintText")
            {
                return PrintText();
            }
            else if (methodName == "PausePrint")
            {
                PausePrint();
            }
            else if (methodName == "ResumePrint")
            {
                ResumePrint();
            }

            return null;
        }
        
        //  Called In Game
        public void OnInitializeGame()
        {
            //  All Script Behaviour Components Initialized
            var scriptCompList = new List<ScriptBehaviour>(GetComponents<ScriptBehaviour>());
            foreach (var comp in scriptCompList)
            {
                comp.Initialize(this);
                if (comp.isPlaying)
                    comp.Play();
            }
            //  Initialize VM and Invoke AwakeRun
            actionVm = ScriptableObject.CreateInstance<DialogueActionVM>();
            actionVm.Initialize(script.actionScripts, this);
        }
        #endregion
        #region Utility Coroutines

        private IEnumerator IPrintText(Text t, string con)
        {
            t.text = "";

            char[] speechArr = con.ToCharArray();
            int i = 0;
            for (i = 0; i < speechArr.Length; i++)
            {
                //EventCall
                if (speechArr[i] == '%')
                {
                    //Call Event => Invoke Action Script
                    if (speechArr[i + 1] == 'c')
                    {
                        int j = i + 3;
                        string eventName = "";
                        while (speechArr[j] != ' ')
                        {
                            eventName += speechArr[j];
                            j++;
                        }
                        i = j - 1;
                        actionVm.OnEventInvoke(this, eventName);
                    }
                    continue;
                }
                //RichText 처리하기
                if (speechArr[i] == '<')
                {
                    string richText = "";
                    string insideText = "";
                    char[] insideTextArr;
                    int insideLength;
                    int insidePosition;
                    while (speechArr[i] != '>')
                    {
                        richText += speechArr[i];
                        i++;
                    }
                    richText += '>';
                    t.text += richText;
                    i++;
                    insideLength = i;
                    while (speechArr[insideLength] != '<')
                    {
                        insideText += speechArr[insideLength];
                        insideLength++;
                    }
                    i = insideLength;
                    insideLength--;
                    richText = "";
                    while (speechArr[i] != '>')
                    {
                        richText += speechArr[i];
                        i++;
                    }
                    richText += '>';
                    t.text += richText;
                    i++;
                    insidePosition = t.text.Length - richText.Length;
                    insideTextArr = insideText.ToCharArray();

                    for (int idx = 0; idx < insideTextArr.Length; idx++)
                    {
                        string tempText = insideTextArr[idx].ToString();
                        t.text = t.text.Insert(insidePosition, tempText);
                        insidePosition++;
                        var inTimer = 0f;
                        while (inTimer <= typeDuration)
                        {
                            inTimer += Time.deltaTime * playSpeed;
                            yield return null;
                        }
                    }
                }
                t.text += speechArr[i];
                AudioManager.DelayedPlay(stagedActor,
                    trackManager.targetTrack.typingSoundProfile,
                    1,
                    false
                    );
                if(skipMode == 2)
                    continue;
                var timer = 0f;
                while (timer <= typeDuration)
                {
                    timer += Time.deltaTime * playSpeed;
                    yield return null;
                }
            }
        }

        #endregion
        //  Initialize Process Funcs
        #region Initialize Region
        private void StageBubbleTalk(BaseScript s)
        {
            var cachedActor = StagedObjectManager.FindGameActor(s.speaker);
            if (null == cachedActor)
                stagedActor = trackManager.StagingActor(s.speaker, GameActor.ActorType.Player);
            else
                stagedActor = cachedActor;
            contentText.text = "";
            UIAlphaController.SetAlphaColor<Image>(panelImage.rectTransform, 0f);
        }
        private void StageCutSceneTalk(BaseScript s)
        {
            nameText.text = s.speaker.actorName;
            nameText.color = s.speaker.actorNameColor;
            contentText.text = "";
        }
        private void StageCutSceneOptionTalk(BaseScript s)
        {

        }
        public void Stage(TrackManager m, BaseScript s)
        {
            trackManager = m;
            script = s;
            gameObject.name = s.type.ToString() + "_" + m.stagedScripts.Count + "_" + s.speaker.actorName;
            panelRect = GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelRect.localScale = Vector3.one;
            
            switch (s.type)
            {
                case ScriptType.말풍선_대화:
                    StageBubbleTalk(s);
                    break;
                case ScriptType.컷씬_대화:
                    StageCutSceneTalk(s);
                    break;
                case ScriptType.컷씬_대화_선택지:
                    StageCutSceneOptionTalk(s);
                    break;
            }
            gameObject.SetActive(false);
        }
        #endregion
    }
}
