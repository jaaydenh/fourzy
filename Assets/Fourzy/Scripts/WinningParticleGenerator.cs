//modded @vadym udod

using Fourzy._Updates.Tools;
using UnityEngine;

namespace Fourzy
{
    public class WinningParticleGenerator : RoutinesBase
    {
        public ParticleSystem _particleSystem;

        public void ShowParticles()
        {
            _particleSystem.Play(true);
            StartRoutine("particles", 5f, () => { _particleSystem.Stop(); });
        }

        public void StopShowingParticles()
        {
            CancelRoutine("particles");
        }
    }
}
