namespace Roguelike.Objects
{
    public abstract class Enemy : Entity
    {
        public Enemy()
        {
            Health = RandomGenerator.Next(40, 121);
            Attack = RandomGenerator.Next(4, 11);
            Defense = RandomGenerator.Next(4, 11);
            Strength = RandomGenerator.Next(4, 11);
            Dexterity = RandomGenerator.Next(4, 11);
            Stamina = RandomGenerator.Next(4, 11);

            Speed = RandomGenerator.Next(50, 201);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDead => Health <= 0;
    }
}
