using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Additional.CommandAction
{
    public class CommandActionProcessor : SingletonGameObject<CommandActionProcessor>
    {
        public SkillCommandTree commandTreeFactory;
        public SkillCommandTree.Tree commandTree;

        public int maxBufferCount = 2;
        public int deltaFrame = 0;
        public int depth = 0;
        public int nextThreshold = 0;

        [SerializeField]
        private SkillCommandTree.TreeNode curTreeNode;
        [SerializeField]
        private List<SkillProfile> skillBuffer = new List<SkillProfile>();

        private BehaviourJob commandActionJob = null;

        public Action<SkillProfile> onCompleteProcess;
        
        public void Initialize(string treeName, List<SkillProfile> skills)
        {
            commandTree = commandTreeFactory.GetCommandTree(treeName, skills.ToArray());
            curTreeNode = commandTree.root;
        }

        public SkillProfile GetBufferTop()
        {
            return CountOfBuffer > 0 ? skillBuffer[0] : null;
        }

        public void ClearBuffer()
        {
            skillBuffer.Clear();
        }

        public SkillProfile ConsumeBuffer()
        {
            var top = GetBufferTop();
            if (null == top) return null;
            skillBuffer.RemoveAt(0);
            return top;
        }
        public void AddSkillToBuffer(SkillProfile skill)
        {
            if (skill == null) return;
            skillBuffer.Add(skill);
        }
        public int CountOfBuffer => skillBuffer.Count;

        public void InputCommandAction(string actionName)
        {
            if (curTreeNode.CountOfChild == 0)
            {
                //  리프 스킬 노드인 경우, 탐색이 필요없으며
                //  자연스럽게 사용되고 끝나길 기다린다.
                return;
            }
            var nextTreeNode = curTreeNode;
            for (var i = 0; i < curTreeNode.CountOfChild; i++)
            {
                var child = curTreeNode.children[i];
                var actionCond = child.inputDesc.actionName == actionName;
                var frameCond = child.inputDesc.frameThreshold >= deltaFrame;
                if (!actionCond || !frameCond) continue;
                nextTreeNode = child;
                break;
            }

            var foundMatchedNode = curTreeNode != nextTreeNode;
            var canLoadBuffer = CountOfBuffer < maxBufferCount;
            //  Updated In 20-07-07 It can occur Error
            if (!foundMatchedNode && canLoadBuffer)
            {
                for (var i = 0; i < commandTree.root.CountOfChild; i++)
                {
                    var child = commandTree.root.children[i];
                    var actionCond = child.inputDesc.actionName == actionName;
                    var frameCond = child.inputDesc.frameThreshold >= deltaFrame;
                    if (!actionCond || !frameCond) continue;
                    nextTreeNode = child;
                    break;
                }
                foundMatchedNode = curTreeNode != nextTreeNode;
            }
            
            if (!foundMatchedNode||!canLoadBuffer)
            {
                Reset();
                return;
            }
            

            curTreeNode = nextTreeNode;
            deltaFrame = 0;
            nextThreshold = curTreeNode.inputDesc.frameThreshold;
            if(curTreeNode.skillProfile != null)
                onCompleteProcess?.Invoke(curTreeNode.skillProfile);
            depth = curTreeNode.Depth;

            commandActionJob?.Kill();
            commandActionJob = BehaviourJob.Make(IProcessJob());
        }

        private void OnFrameOverCommandProcess()
        {
            Reset();
        }
        private IEnumerator IProcessJob()
        {
            while (++deltaFrame < nextThreshold)
            {
                yield return new WaitForEndOfFrame();
            }
            OnFrameOverCommandProcess();
        }

        public void Reset()
        {
            curTreeNode = commandTree.root;
            deltaFrame = 0;
            depth = 0;
            nextThreshold = 0;
            commandActionJob?.Kill();
            ClearBuffer();
        }
    }
}
