using Domain.Enums.Character;
using Domain.ValueObjects.Character;
using System.Text.Json.Serialization;

namespace Domain.Models.Character
{
    public  class Character
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public CharacterClass CurrentClass { get; private set; }
        public CharacterAttributes Attributes { get; private set; }
        public int CurrentHealth { get; private set; }

        [JsonIgnore]
        public bool IsAlive => CurrentHealth > 0;

        [JsonIgnore]
        public int MaxHealth => CalculateMaxHealth(CurrentClass, Attributes);

        // construtor de criação de novo personagem na sessão
        public Character(
            string name,
            CharacterClass initialClass)
            : this(
                Guid.NewGuid(),
                name,
                initialClass,
                new CharacterAttributes(5, 5, 5),
                null)
        {
        }

        // reconstrução de um personagem existente.
        [JsonConstructor]
        public Character(
            Guid id,
            string name,
            CharacterClass currentClass,
            CharacterAttributes attributes,
            int currentHealth)
            : this(
                id,
                name,
                currentClass,
                attributes,
                (int?)currentHealth)
        {
        }

        // centralização das invariantes.
        private Character(
            Guid id,
            string name,
            CharacterClass currentClass,
            CharacterAttributes attributes,
            int? currentHealth)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(
                    "Identificador inválido.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Nome inválido.", nameof(name));

            ArgumentNullException.ThrowIfNull(currentClass);
            ArgumentNullException.ThrowIfNull(attributes);

            Id = id;
            Name = name;
            CurrentClass = currentClass;
            Attributes = attributes;

            var maxHealth = MaxHealth;
            var health = currentHealth ?? maxHealth;

            if (health < 0 || health > maxHealth)
                throw new ArgumentOutOfRangeException(
                    nameof(currentHealth),
                    "Vida atual fora dos limites permitidos.");

            CurrentHealth = health;
        }

        public int CalculateAttackDamage()
        {
            return CurrentClass.CalculateBasicAttack(
                Attributes.Strength,
                Attributes.Intelligence,
                Attributes.Agility
            );
        }

        public int Attack() =>
            IsAlive
            ? CalculateAttackDamage()
            : throw new InvalidOperationException("Um personagem morto não pode atacar.");

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            if (!IsAlive || damage == 0)
                return;

            CurrentHealth = Math.Max(
                0,
                CurrentHealth - damage
            );
        }

        public void LevelUpAttributes(
            int strength, 
            int intelligence, 
            int agility)
        {
            var updatedAttributes =
                Attributes.Increase(strength, intelligence, agility);

            // valida a nova vida máxima antes de alterar o estado
            _ = CalculateMaxHealth(
                CurrentClass,
                updatedAttributes);

            Attributes = updatedAttributes;
        }

        private static int CalculateMaxHealth(
            CharacterClass characterClass,
            CharacterAttributes attributes)
        {
            return checked(
                characterClass.BaseHealth +
                attributes.Strength * 5
            );
        }
    }
}
