using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lib
{
    public static class Util
    {
        public static T OpenUI<T>(OpenUIMode mode = OpenUIMode.Single) where T : UIBase
        {
            var name       = typeof(T).Name;
            var rootUIRoot = GameControllerBase.Instance.root.uiRoot;
            var uiExist    = rootUIRoot.Find(name);
            if (uiExist != null)
            {
                return uiExist.GetComponent<T>();
            }
            if (mode == OpenUIMode.Single) 
                rootUIRoot.DestroyChildren();

            var uiPrefab = Resources.Load<GameObject>(Const.UI_FOLDER + name);
            var uiInstance      = Object.Instantiate(uiPrefab, rootUIRoot);
            uiInstance.name = name;
            return uiInstance.GetComponent<T>();
        }

        public static bool CloseUI<T>() where T : UIBase
        {
            var name    = typeof(T).Name;
            var uiRoot  = GameControllerBase.Instance.root.uiRoot;
            var uiCount = uiRoot.childCount;
            
            for (var i = 0; i < uiCount; i++)
            {
                var child = uiRoot.GetChild(i);
                if (child.name != name) continue;
                Object.Destroy(child.gameObject);
                return true;
            }

            return false;
        }
        public static async UniTaskVoid ShowWordsFade(List<string> words, float showTime, float fadeTime, TextMeshProUGUI tmp)
        {
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                tmp.text = word;
                await tmp.DOFade(1, fadeTime);
                await UniTask.Delay(TimeSpan.FromSeconds(showTime));
                await tmp.DOFade(0, fadeTime); 
            }
        }

        public static async UniTask ShowWordsOnClick(List<string> words, float showAnimDuration, TextMeshProUGUI tmp, CancellationToken token)
        {
            CancellationTokenSource wordsCts    = null;
            var                     listenCts   = new CancellationTokenSource();
            var                     listenToken = CancellationTokenSource.CreateLinkedTokenSource(listenCts.Token, token);
            ListenToSkip().Forget();

            for (var i = 0; i < words.Count; i++)
            {
                wordsCts = new CancellationTokenSource();
                var wordsToken = CancellationTokenSource.CreateLinkedTokenSource(wordsCts.Token, token);
                var isSkip = false;
                tmp.text = "";
                var word = words[i];
        
                tmp.DOKill();
                isSkip = await tmp.DOText(word, showAnimDuration).ToUniTask(TweenCancelBehaviour.CompleteWithSequenceCallbackAndCancelAwait, wordsToken.Token).SuppressCancellationThrow();
                while (!isSkip)
                {
                    isSkip = await UniTask.NextFrame(wordsToken.Token).SuppressCancellationThrow();
                }
                wordsCts.Dispose();
            }

            if (!listenToken.IsCancellationRequested)
            {
                listenToken.Cancel();
                listenToken.Dispose();     
            }
            

            async UniTaskVoid ListenToSkip()
            {
                var isCancelled = false;
                while (!isCancelled)
                {
                    if (Input.GetMouseButtonUp(0)) 
                        wordsCts?.Cancel();
                    isCancelled = await UniTask.NextFrame(listenToken.Token).SuppressCancellationThrow();
                }

                Debug.Log("listenerIsCancelled");
                listenToken.Dispose();
            }
        }
    }
}