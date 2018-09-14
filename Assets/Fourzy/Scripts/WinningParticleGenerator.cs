using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningParticleGenerator : MonoBehaviour 
{
    [SerializeField]
    private GameObject particlePrefab;

    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private List<BoxCollider2D> particleAreas = new List<BoxCollider2D>();

    private List<int> areasIndices = new List<int>();

    private Coroutine spawnCoroutine;

	//void Start () 
 //   {
 //       this.ShowParticles();
	//}
	
    public void ShowParticles()
    {
        this.gameObject.SetActive(true);
        spawnCoroutine = this.StartCoroutine(StartSpawningParticles());
    }

    public void StopShowingParticles()
    {
        if (spawnCoroutine != null)
        {
            this.StopCoroutine(spawnCoroutine);
        }
        this.gameObject.SetActive(false);
    }

    private IEnumerator StartSpawningParticles()
    {
        float t = 0;
        float nextExplose = 0.0f;
        while (true)
        {
            t += Time.deltaTime;
            if (t > nextExplose)
            {
                GameObject go = Instantiate(particlePrefab, this.transform);
                go.transform.position = this.SelectRandomPosition();
                go.transform.rotation = particlePrefab.transform.rotation;
                Vector3 scale = particlePrefab.transform.localScale * 100;
                go.transform.localScale = scale;
                go.GetComponent<ParticleSystemRenderer>().sortingOrder = 100;

                Destroy(go, 6.0f);

                nextExplose = Random.Range(1.0f, 2.0f);
                t = 0;
            }
            yield return null;
        }
    }

    private Vector3 SelectRandomPosition()
    {
        this.UpdateIndicesList();

        int index = Random.Range(0, areasIndices.Count - 1);
        int areaIndex = areasIndices[index];
        areasIndices.RemoveAt(index);

        BoxCollider2D area = particleAreas[areaIndex];

        Bounds bounds = area.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(x, y);
    }

    private void UpdateIndicesList()
    {
        if (areasIndices.Count != 0)
        {
            return;
        }

        for (int i = 0; i < particleAreas.Count; i++)
        {
            areasIndices.Add(i);
        }
    }
}
