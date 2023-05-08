using UnityEngine;

public class GhostBlock : MonoBehaviour
{
    Block block;
    BlockSpawner spawner;
    public void Init(BlockSpawner spawner, Block block)
    {
        this.spawner = spawner;
        this.block = block;
        block.OnBlockHitGround += _ => Destroy(gameObject);

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.3f);

            }
        }
    }

    public void UpdatePosition()
    {
        int yOffset = 0;
        while (true)
        {
            foreach (Transform child in block.transform.GetChild(0))
            {
                if (!IsValid(child.position.AddTo(y: yOffset - 1)))
                    goto End;
            }
            yOffset -= 1;
        }
        End:
        transform.position = block.transform.position.AddTo(y: yOffset);
        transform.rotation = block.transform.rotation;
    }

    bool IsValid(Vector2 pos)
    {
        if (pos.y < 0 || spawner.IsOccupied(pos.x, pos.y)) 
        { 
            return false;
        }
        return true;
    }
}
