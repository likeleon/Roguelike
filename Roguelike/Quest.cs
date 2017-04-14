using System;

namespace Roguelike
{
    public sealed class Quest
    {
        public QuestType QuestType { get; }
        public string Title { get; private set; }
        public int Goal { get; private set; }

        public static Quest CreateRandom()
        {
            var questType = (QuestType)Rand.Next(EnumExtensions.GetCount<QuestType>());
            return new Quest(questType);
        }

        private Quest(QuestType questType)
        {
            QuestType = questType;

            switch (QuestType)
            {
                case QuestType.CollectGem:
                    Goal = Rand.Next(5, 11);
                    break;

                case QuestType.CollectGold:
                    Goal = Rand.Next(50, 101);
                    break;

                case QuestType.KillEnemy:
                    Goal = Rand.Next(5, 11);
                    break;
            }

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            switch (QuestType)
            {
                case QuestType.CollectGem:
                    Title = $"Current Goal: Collect {Goal} gems!";
                    break;

                case QuestType.CollectGold:
                    Title = $"Current Goal: Collect {Goal} gold!";
                    break;

                case QuestType.KillEnemy:
                    Title = $"Current Goal: Kill {Goal} enemies!";
                    break;

                default:
                    throw new Exception($"Invalid QuestType ${QuestType}");
            }
        }

        public void DecreaseGoal(int amount)
        {
            Goal -= amount;
            UpdateTitle();
        }

        public bool IsCompleted => Goal <= 0;
    }

    public enum QuestType
    {
        CollectGem,
        CollectGold,
        KillEnemy
    }
}
