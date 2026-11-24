using Domain.Enums.Character;

namespace Domain.ValueObjects.Character
{
    public sealed record ClassProgress
    {
        public const int ExperiencePerLevel = 1000;

        public CharacterClass CharacterClass { get; }
        public int Level { get; }
        public int Experience { get; }

        public ClassProgress
            (CharacterClass characterClass, 
            int level, 
            int experience)
        {
            ArgumentNullException.ThrowIfNull(characterClass);

            if (level < 1) 
                throw new ArgumentException("O nível mínimo é 1.");

            if (experience < 0 || experience >= ExperiencePerLevel) 
                throw new ArgumentException(
                    nameof(experience),
                    $"A experiência deve estar entre 0 e {ExperiencePerLevel - 1}.");

            CharacterClass = characterClass;
            Level = level;
            Experience = experience;
        }

        public ClassProgress GainExperience(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            var newXp = (long)Experience + amount;
            var levelsGained = (int)(newXp / ExperiencePerLevel);
            var remainingExperience = (int)(newXp % ExperiencePerLevel);

            return new ClassProgress
            (
                CharacterClass,
                checked(Level + levelsGained),
                remainingExperience
            );
        }
    }
}
