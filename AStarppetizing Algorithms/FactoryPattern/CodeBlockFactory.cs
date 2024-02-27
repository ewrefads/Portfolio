using AStarppetizing_Algorithms.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.FactoryPattern
{
    public enum CODEBLOCKTYPES {empty, createLists, findShortestDistance, moveToClosedList, checkNeighbours, checkNeighboursEnd, checkForObstacles, isOnClosedList, isOnOpenList, setCurrentNodeAsParent, addToOpenList,calculateF, calculateG, calculateH, changeParent, isGoalReached, isOpenListEmpty}
    public class CodeBlockFactory : Factory
    {
        private static CodeBlockFactory instance;

        private CodeBlockFactory()
        {
        }

        public static CodeBlockFactory Instance { get {
                if (instance == null) {
                    instance = new CodeBlockFactory();
                }
                return instance;    
            } 
        }
        /// <summary>
        /// Laver en kodeblok
        /// </summary>
        /// <param name="type">Hvilken type kodeblok der skal bruges</param>
        /// <returns>Den færdige kodeblok med indbygget knap til at bevæge den</returns>
        public override GameObject Create(Enum type)
        {
            GameObject codeBlock = UIFactory.Instance.Create(UITYPE.BUTTON);
            CodeBlock c = new CodeBlock();
            Button b = (Button)codeBlock.GetComponent<Button>();
            b.OnClick = c.Move;
            switch (type) {
                case CODEBLOCKTYPES.createLists:
                    c.Method = CodeManager.Instance.CreateLists;
                    b.ButtonText = "Create Lists";
                    break;
                case CODEBLOCKTYPES.findShortestDistance:
                    c.Method = CodeManager.Instance.FindShortestDistance;
                    b.ButtonText = "Find Shortest Distance";
                    break;
                case CODEBLOCKTYPES.moveToClosedList:
                    c.Method = CodeManager.Instance.MoveToClosedList;
                    b.ButtonText = "Move To Closed List";
                    break;
                case CODEBLOCKTYPES.checkNeighbours:
                    c.Method = CodeManager.Instance.CheckNeighbours;
                    b.ButtonText = "Check Neighbours";
                    break;
                case CODEBLOCKTYPES.checkNeighboursEnd:
                    c.Method = CodeManager.Instance.CheckNeighboursEnd;
                    b.ButtonText = "Check Neighbours end";
                    break;
                case CODEBLOCKTYPES.checkForObstacles:
                    c.Method = CodeManager.Instance.checkForObstacles;
                    b.ButtonText = "Check for obstacles";
                    break;
                case CODEBLOCKTYPES.isOnClosedList:
                    c.Method = CodeManager.Instance.IsOnClosedList;
                    b.ButtonText = "is on closed List";
                    break;
                case CODEBLOCKTYPES.isOnOpenList:
                    c.Method = CodeManager.Instance.IsOnOpenList;
                    b.ButtonText = "is on open List";
                    break;
                case CODEBLOCKTYPES.calculateF:
                    c.Method = CodeManager.Instance.CalculateF;
                    b.ButtonText = "Calculate F";
                    break;
                case CODEBLOCKTYPES.calculateG:
                    c.Method = CodeManager.Instance.CalculateG;
                    b.ButtonText = "Calculate G";
                    break;
                case CODEBLOCKTYPES.calculateH:
                    c.Method = CodeManager.Instance.CalculateH;
                    b.ButtonText = "Calculate H";
                    break;
                case CODEBLOCKTYPES.setCurrentNodeAsParent:
                    c.Method = CodeManager.Instance.SetCurrentNodeAsParent;
                    b.ButtonText = "Set Current Node As Parent";
                    break;
                case CODEBLOCKTYPES.addToOpenList:
                    c.Method = CodeManager.Instance.AddToOpenList;
                    b.ButtonText = "Add to open list";
                    break;
                case CODEBLOCKTYPES.changeParent:
                    c.Method = CodeManager.Instance.ChangeParent;
                    b.ButtonText = "Change Parent";
                    break;
                case CODEBLOCKTYPES.isGoalReached:
                    c.Method = CodeManager.Instance.IsGoalReached;
                    b.ButtonText = "Is Goal Reached";
                    break;
                case CODEBLOCKTYPES.isOpenListEmpty:
                    c.Method = CodeManager.Instance.IsOpenListEmpty;
                    b.ButtonText = "Is Open List Empty";
                    break;
                case CODEBLOCKTYPES.empty:
                    codeBlock.RemoveComponent(b);
                    break;
                default:
                    break;
            }
            codeBlock.AddComponent(c);
            return codeBlock;
        }
    }
}
