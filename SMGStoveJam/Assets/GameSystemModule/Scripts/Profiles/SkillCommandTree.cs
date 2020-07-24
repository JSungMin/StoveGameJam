using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace CoreSystem.Additional.CommandAction
{
    [CreateAssetMenu(menuName = "GameData/CommandAction/SkillCommandTree")]
    public class SkillCommandTree : ScriptableObject
    {
        public string commandTreeName;
        public InputActionAsset mInputAsset;
        public SkillCommand[] skillWithCommands;

        public Tree GetCommandTree(string treeName, SkillProfile[] skillProfiles)
        {
            var commandList = new List<SkillCommand>();
            foreach (var profile in skillProfiles)
            {
                var command = skillWithCommands.FirstOrDefault(x => x.profile == profile);
                commandList.Add(command);
            }
            return new Tree(treeName, mInputAsset, commandList);
        }

        public Tree GetCommandTree()
        {
            return new Tree(commandTreeName, mInputAsset, skillWithCommands);
        }

        [System.Serializable]
        public class Tree
        {
            public string name;
            public InputActionAsset inputAsset;
            public TreeNode root;
            private Dictionary<TreeNode, bool> visit;

            public Tree(string n, InputActionAsset asset, IEnumerable<SkillCommand> skills)
            {
                name = n;
                inputAsset = asset;
                visit = new Dictionary<TreeNode, bool>();
                root = new TreeNode(0, null, null);
                foreach (var s in skills)
                {
                    RebuildWithAdd(root, s);
                }
            }

            public void RebuildWithAdd(TreeNode head, SkillCommand command, int depth = 0)
            {
                var targetDepth = command.CountOfCommand;
                var nextHead = head;
                for (var i = 0; i < targetDepth; i++)
                {
                    var prevHead = nextHead;
                    TreeNode child = null;
                    for (var c = 0; c < prevHead.CountOfChild; c++)
                    {
                        var ch = prevHead.children[c];
                        if (ch.inputDesc.Equal(command.GetDesc(depth)))
                            child = ch;
                    }
                    if (null == child)
                        child = new TreeNode(i + 1, prevHead, command, null);
                    nextHead = child;
                }
                nextHead.skillProfile = command.profile;
            }

            public List<TreeNode> FindMatched(string[] actionStream)
            {
                var result = new List<TreeNode>();

                bool FilterToFindMatchedCommand(int d, TreeNode o)
                {
                    return actionStream[d] == o.actionName;
                }
                void AddElementAction(int d, TreeNode o)
                {
                    result.Add(o);
                }

                TravelTree(actionStream.Length, FilterToFindMatchedCommand, AddElementAction);
                return result;
            }
            public List<TreeNode> FindMatched(InputAction[] actionStream)
            {
                var result = new List<TreeNode>();

                bool FilterToFindMatchedCommand(int d, TreeNode o)
                {
                    return actionStream[d] == o.GetAction(inputAsset);
                }
                void AddElementAction(int d, TreeNode o)
                {
                    result.Add(o);
                }

                TravelTree(actionStream.Length, FilterToFindMatchedCommand, AddElementAction);
                return result;
            }

            public void TravelTree(int maxDepth = int.MaxValue, Func<int, TreeNode, bool> condition = null, Action<int, TreeNode> action = null)
            {
                TravelTree(root, 0, maxDepth, condition, action);
            }
            private void TravelTree(TreeNode node = null, int depth = 0, int maxDepth = int.MaxValue, Func<int, TreeNode, bool> condition = null, Action<int, TreeNode> action = null)
            {
                if (node == null) return;
                if (depth > maxDepth) return;
                if (visit.ContainsKey(node) && visit[node]) return;

                var canTravel = condition != null && condition.Invoke(depth, node);
                if (!canTravel) return;
                action?.Invoke(depth, node);
                visit[node] = true;
                for (var i = 0; i < node.CountOfChild; i++)
                {
                    TravelTree(node.children[i], ++depth, maxDepth, condition, action);
                }
            }

            public TreeNode AddChild(TreeNode parent, TreeNode child)
            {
                parent.AddChild(child);
                return child;
            }

            public void RemoveChild(TreeNode parent, TreeNode child)
            {
                parent.RemoveChild(child);
            }
        }
        [System.Serializable]
        public class TreeNode
        {
            public SkillCommand.InputDesc inputDesc;
            public SkillProfile skillProfile;
            //  InputSystem InputAction Name
            public string actionName = "";

            public TreeNode parent;
            private int depth = 0;
            public List<TreeNode> children;

            public int CountOfChild => children.Count;

            public TreeNode(int d, SkillCommand command, TreeNode[] children = null)
            {
                depth = d;
                this.children = children != null ? new List<TreeNode>(children) : new List<TreeNode>();

                if (command == null)
                    return;
                this.actionName = command.GetActionName(depth-1);
                this.inputDesc = command.GetDesc(depth-1);
            }
            public TreeNode(int d, TreeNode parent, SkillCommand command, TreeNode[] children)
            {
                depth = d;
                this.parent = parent;
                parent?.AddChild(this);
                if (children != null)
                {
                    this.children = new List<TreeNode>(children);
                }
                else
                {
                    this.children = new List<TreeNode>();
                }
                this.actionName = command.GetActionName(depth-1);
                this.inputDesc = command.GetDesc(depth-1);
            }

            public TreeNode AddChild(TreeNode child)
            {
                child.parent = this;
                children.Add(child);
                return child;
            }

            public void RemoveChild(TreeNode child)
            {
                children.AddRange(child.children);
                foreach (var cc in child.children)
                {
                    cc.parent = this;
                }

                children.Remove(child);
            }
            public InputAction GetAction(InputActionAsset asset)
            {
                return asset.FindAction(actionName);
            }

            public int Depth => depth;
        }
#if UNITY_EDITOR
        public static SkillCommandTree CreateInstance(string path, string name)
        {
            var scriptData = CreateInstance<SkillCommandTree>();
            var pathOfData = PathHelper.Bind(path, name + ".asset");
            UnityEditor.AssetDatabase.CreateAsset(
                scriptData,
                pathOfData
            );
            UnityEditor.EditorUtility.SetDirty(scriptData);

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return scriptData;
        }
#endif
    }
}
