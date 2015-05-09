using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Reader;

namespace Model
{
    class QuestSystem
    {
        //QuestStatuses
        public const int PRE = 2;
        public const int MID = 3;
        public const int END = 4;

        //ObjektIndexes 
        public const int ENEMY = 0;
        public const int FRIEND = 1;
        public const int ARMOR = 2;
        public const int QUEST_ITEM = 3;

        //Variables
        private bool m_allQuestsCompleted;
        private int m_currentQuestIndex = 0;
        private List<Reader.Quest> m_questList;
        private List<Reader.Objective> m_objectiveList;
        private string m_currentMessage;
        private int m_activeNpc;

        //Properties
        public bool IsWatchingQuestLog { get; set; }
        public int QuestStatus { get { return CurrentQuest.Status; } set { CurrentQuest.Status = value; } }
       
        //Readonly properties
        public int ActiveNpc { get { return m_activeNpc; } } 
        public string CurrentMessage { get { return m_currentMessage; } }
        public Reader.Quest CurrentQuest { get { return m_questList[m_currentQuestIndex]; } }
        public List<Reader.Objective> ObjectiveList {get { return m_objectiveList; } }
        public List<Reader.Quest> QuestList { get { return m_questList; } }
        public bool AllQuestsCompleted { get { return m_allQuestsCompleted; } }
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public QuestSystem(ContentManager content)
        {
            this.IsWatchingQuestLog = false;
            this.m_questList = content.Load<List<Reader.Quest>>("XML/quest");
            this.m_objectiveList = new List<Reader.Objective>();
            LoadObjectives();
            this.QuestStatus = PRE;
        }

        /// <summary>
        /// Method for updating the current quest progress.
        /// </summary>
        /// <param name="enemyList">List of active enemies</param>
        /// <param name="friendList">List of available npc</param>
        /// <param name="inventoryList">List of current inventory</param>
        /// <param name="level">Level object</param>
        internal void UpdateActiveQuest(List<Enemy> enemyList, List<NonPlayerCharacter> friendList, List<Item> inventoryList, Level level)
        {
            UpdateMessagesAndQuestNpc(); 

            if (!IsQuestComplete()) 
            {
                if (CurrentQuest.Status != PRE)
                {
                    for (int i = 0; i < m_objectiveList.Count; i++) //Uppdating activ quest progress
                    {
                        if (m_objectiveList[i].Obj == ENEMY && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateEnemyStatus(enemyList, m_objectiveList[i]);

                        if (m_objectiveList[i].Obj == FRIEND)
                            UpdateNPCStatus(friendList, m_objectiveList[i]);

                        if (m_objectiveList[i].Obj == ARMOR && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateItemStatus(inventoryList, m_objectiveList[i], Model.GameModel.ARMOR);

                        if (m_objectiveList[i].Obj == QUEST_ITEM && m_objectiveList[i].Amount < CurrentQuest.Objectives[i].Amount)
                            UpdateItemStatus(inventoryList, m_objectiveList[i], Model.GameModel.QUEST_ITEM);
                    }
                }
            }
            else //Quest is complete
                QuestStatus = END;

            RemoveProgressObstacle(level);//Removes obstacles related to the quest
        }
      
        /// <summary>
        /// Checks if a quest related obstacle should be removed
        /// </summary>
        /// <param name="level">Lecel object</param>
        private void RemoveProgressObstacle(Level level)
        {
            if (m_currentQuestIndex == 2)
            {
                for (int i = 0; i < level.CollisionLayer.MapObjects.Length; i++)
                {
                    if (level.CollisionLayer.MapObjects[i].Name == "Gate1")
                        level.CollisionLayer.MapObjects[i].Name = "Open";
                }
            }
        }

        /// <summary>
        /// Updates the quests status in regard of enemies
        /// </summary>
        /// <param name="enemyList">List of enemy objects</param>
        /// <param name="objective">Current objective</param>
        private void UpdateEnemyStatus(List<Enemy> enemyList, Reader.Objective objective)
        {
            foreach (Enemy enemy in enemyList)
            {
                if (enemy.CanAddToQuest && enemy.Type == objective.ObjType)   //Om fienden är av rätt typ så läggs den till i questets progress
                {
                    objective.Amount++;
                    enemy.CanAddToQuest = false;
                }
            }
        }

        /// <summary>
        /// Updates the quests status in regard of npcs
        /// </summary>
        /// <param name="npcList">List of npc objects</param>
        /// <param name="objective">Current objective</param>
        private void UpdateNPCStatus(List<NonPlayerCharacter> npcList, Reader.Objective objective) 
        {
            NonPlayerCharacter questNPC = npcList.Find(Friend => Friend.UnitId == objective.ObjType && Friend.CanAddToQuest);

            //If the npc is locaded within the players screen yhe progress is changed
            if (questNPC != null)
                objective.Amount = 1;
            else
                objective.Amount = 0;
        }

        /// <summary>
        /// Updates the objectives Item status
        /// </summary>
        /// <param name="inventoryList">Current inventory</param>
        /// <param name="objective">Current objective</param>
        /// <param name="itemObjType">Type of item object</param>
        private void UpdateItemStatus(List<Item> inventoryList, Reader.Objective objective,  Type itemObjType)
        {
            objective.Amount = inventoryList.Count(Item => Item.GetType() == itemObjType && Item.Type == objective.ObjType);
        }
    
        /// <summary>
        /// Loads the objectives
        /// </summary>
        public void LoadObjectives()
        {
            m_objectiveList = new List<Objective>();
            Reader.Objective objective = new Objective();
            for (int i = 0; i < CurrentQuest.Objectives.Count; i++)//Loads the objectives for the current quest and sets the progress on the differenbt objectives.
            {
                objective.Obj = CurrentQuest.Objectives[i].Obj;
                objective.ObjType = CurrentQuest.Objectives[i].ObjType;
                objective.Amount = 0;
                objective.Name = CurrentQuest.Objectives[i].Name;

                m_objectiveList.Add(objective);
            }
        }
       
        /// <summary>
        /// Checks if a quest was completed
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsQuestComplete()
        {
            int nrOfObjectives = m_objectiveList.Count;
            int completedObjectives = 0;

            for (int i = 0; i < nrOfObjectives; i++)//If all objectives is compleded so is the quest
            {
                if (m_objectiveList[i].Amount >= CurrentQuest.Objectives[i].Amount)
                    completedObjectives++;
            }
            if (completedObjectives == nrOfObjectives)
                return true;

            return false;
        }
       
        /// <summary>
        /// Updates message and active quest npc
        /// </summary>
        public void UpdateMessagesAndQuestNpc()
        {
            switch (QuestStatus)
            {
                case PRE:
                    m_currentMessage = CurrentQuest.PreMessage;
                    m_activeNpc = CurrentQuest.QuestPickup;
                    break;
                case MID:
                    m_currentMessage = CurrentQuest.MidMessage;
                    m_activeNpc = CurrentQuest.QuestPickup;
                    break;
                case END:
                    m_currentMessage = CurrentQuest.EndMessage;
                    m_activeNpc = CurrentQuest.QuestTurnIn;
                    break;
            }
        }
      
        /// <summary>
        /// Loads and activates the next quest in the questchain
        /// </summary>
        public void ActivateNextQuest()
        {
            if (!(m_questList.Last().Id == CurrentQuest.Id))
            {
                m_currentQuestIndex++;
                CurrentQuest.Status = PRE;
                LoadObjectives();
            }
            else
                m_allQuestsCompleted = true;
        }

    }
}
