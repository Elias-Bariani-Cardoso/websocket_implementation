namespace Domain.ValueObjects.Character
{
    public record CharacterAttributes
    {
        public const int MaxAttributeValue = 10_000;

        public int Strength { get; }
        public int Intelligence { get; }
        public int Agility { get; }

        public CharacterAttributes(
        int strength,
        int intelligence,
        int agility)
        {
            Strength = ValidateAttribute(strength, nameof(strength));
            Intelligence = ValidateAttribute(intelligence, nameof(intelligence));
            Agility = ValidateAttribute(agility, nameof(agility));
        }

        private static int ValidateAttribute(int value, string paramName)
        {
            if (value < 0 || value > MaxAttributeValue)
                throw new ArgumentOutOfRangeException(
                    paramName,
                    $"O atributo deve estar entre 0 e {MaxAttributeValue}.");

            return value;
        }

        // método puro: retorna um novo record sem alterar o original
        public CharacterAttributes Increase(int strength = 0, int intelligence = 0, int agility = 0)
        {
            _ = (strength < 0 || intelligence < 0 || agility < 0)
            ? throw new ArgumentException("Atributos não podem ser negativos")
            : 0;

            return new CharacterAttributes
                (
                    checked(Strength + strength),
                    checked(Intelligence + intelligence),
                    checked(Agility + agility)
                );
        }
    }
}
