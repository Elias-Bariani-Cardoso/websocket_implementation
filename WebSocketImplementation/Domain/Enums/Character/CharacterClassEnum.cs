namespace Domain.Enums.Character
{
    // classe SmartEnum
    public abstract class CharacterClass
    {
        public static readonly CharacterClass Warrior = new WarriorClass();
        public static readonly CharacterClass Mage = new MageClass();
        public static readonly CharacterClass Archer = new ArcherClass();

        public string Name { get; }
        public int BaseHealth { get; }
        public int BaseMana { get; }

        private CharacterClass(string name, int baseHealth, int baseMana)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentOutOfRangeException(nameof(name), "Nome inválido");
            BaseHealth = baseHealth > 0 ? baseHealth : throw new ArgumentOutOfRangeException(nameof(baseHealth));
            BaseMana = baseMana < 0 ? throw new ArgumentOutOfRangeException(nameof(baseMana)) : baseMana;
        }

        // regra de negócio específica de cada classe | Template Method
        public int CalculateBasicAttack(
        int strength,
        int intelligence,
        int agility)
        {
            if (strength < 0 || intelligence < 0 || agility < 0)
                throw new ArgumentException(
                    "Atributos não podem ser negativos.");

            return CalculateDamage(strength, intelligence, agility);
        }

        protected abstract int CalculateDamage(int strength, int intelligence, int agility);

        // factory method de resolução de classe pelo nome. Consumido pela persistência para reconstrução a partir do banco de dados
        public static CharacterClass FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Nome da classe inválido.",
                    nameof(name));

            return name.ToLowerInvariant() switch
            {
                "warrior" => Warrior,
                "mage" => Mage,
                "archer" => Archer,

                _ => throw new ArgumentException(
                    $"Classe de personagem inválida: {name}"
                    , nameof(name))
            };
        }

        // implementações da classe abstrata com Template Method

        private sealed class WarriorClass() 
            : CharacterClass("Warrior", 150, 30)
        {
            protected override int CalculateDamage(
                int strength, 
                int intelligence, 
                int agility) => checked(strength * 2);
        }

        private sealed class MageClass() 
            : CharacterClass("Mage", 80, 150)
        {
            protected override int CalculateDamage(
                int strength, 
                int intelligence, 
                int agility) => checked(intelligence * 3);
        }

        private sealed class ArcherClass() 
            : CharacterClass("Archer", 110, 70)
        {
            protected override int CalculateDamage(
                int strength, 
                int intelligence, 
                int agility) => checked(agility * 2 + strength / 2);
        }
    }
}
