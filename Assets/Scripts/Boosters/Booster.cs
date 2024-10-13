using UnityEngine;

public abstract class Booster : MonoBehaviour
{
    private BoosterGenerator _boosterGenerator;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
            _boosterGenerator.ReleaseBooster(this);
    }

    public void SetBoosterGenerator(BoosterGenerator boosterGenerator) => _boosterGenerator = boosterGenerator;
}
