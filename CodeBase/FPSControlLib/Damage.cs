/*
jmpp: Does this actually require documentation? Seems like an incomplete version of Damageable; see notes for more comments on the comparison.
*/
namespace FPSControl
{
    public class Damage
    {
        public int              _health             = 5;    //--- initial health value
        public GameObject       _hideThisOnDeath;           //--- object to hide on death
        public GameObject       _animator;                  //--- animator for death animation
        public string           _deathAnimation;
        public AudioSource      _audioSource;
        public AudioClip        _deathSound;
        public string           _deathAction;

        public float        amount;
    
        private DamageSource source;
        private Vector3 origin;
        private Vector3 damageLocation;
    }
}
