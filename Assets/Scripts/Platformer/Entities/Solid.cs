using Binocle;

namespace App.Platformer
{
    public class Solid : GameEntity
    {
        public Hero GetHeroRiding()
        {
            foreach (Hero hero in FindObjectsOfType(typeof(Hero)))
            {
                if (hero.IsRiding(this))
                {
                    return hero;
                }
            }
            return null;
        }

        public bool IsHeroRiding()
        {
            return GetHeroRiding() != null;
        }

    }
}