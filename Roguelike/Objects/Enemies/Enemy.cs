namespace Roguelike.Objects
{
    public abstract class Enemy : Entity
    {
        public Enemy()
        {
            Health = Rand.Next(40, 121);
            Attack = Rand.Next(4, 11);
            Defense = Rand.Next(4, 11);
            Strength = Rand.Next(4, 11);
            Dexterity = Rand.Next(4, 11);
            Stamina = Rand.Next(4, 11);

            Speed = Rand.Next(50, 201);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDead => Health <= 0;
    }
}
