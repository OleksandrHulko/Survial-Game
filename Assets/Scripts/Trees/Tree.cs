using UnityEngine;
using Debug = UnityEngine.Debug;

public class Tree : MonoBehaviour , IDamageable
{
    #region Serialize Fields
    [SerializeField]
    private TreeType treeType = TreeType.None;
    [SerializeField]
    private int health = 0;
    [SerializeField] 
    private Deck deck = null;
    [SerializeField]
    private int decksCount = 0;
    #endregion

    #region Private Fields
    private Deck[] _decks = new Deck[10];
    private DamageableType _damageableType = DamageableType.Tree;
    private int _damage = 0;
    private bool _spawnedDecks = false;
    private int CurrentHealth => Mathf.Max(0, health - _damage);
    #endregion
    
    
    #region Public Methods
    public void HideTreeOnChunk()
    {
        if (_spawnedDecks)
            DestroyDecks();
        
        SetActive(false);
    }

    public void ShowTreeOnChunk(Transform parentalTransform, Vector3 localPosition)
    {
        SetActive(true);
        SetParent(parentalTransform);
        SetLocalPosition(localPosition);
        
        _damage = 0;
        _spawnedDecks = false;
    }

    public void GetDamaged(DamageType damageType)
    {
        _damage += new DamageDamageablePair(damageType, _damageableType).GetDamage();

        if (CurrentHealth == 0)
            Destroy();
        
        //Debug.Log(CurrentHealth);
    }
    #endregion
    
    #region Private Methods
    private void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void SetParent(Transform parentalTransform)
    {
        transform.SetParent(parentalTransform);
    }

    private void SetLocalPosition( Vector3 localPosition )
    {
        transform.localPosition = localPosition;
        transform.CorrectEulerAngles(y: localPosition.y * 1000, ySetMode: true);
    }
    
    private void Destroy()
    {
        SpawnDecks();
        SetActive(false);
        TreeSpawner.AddNewFelledTreesPositions(transform.position.ToVirtualPosition().ToVector2Int());
    }

    private void SpawnDecks()
    {
        Transform treeTransform = transform;
        
        float height      = 0.0f;
        float decksHeight = 3.0f;

        _spawnedDecks = true;
        
        for (int i = 0; i < decksCount; i++)
        {
            height = decksHeight / 2.0f + decksHeight * i;
            _decks[i] = Instantiate(deck, treeTransform.position + treeTransform.up * height, Quaternion.identity, treeTransform.parent);
        }
    }

    private void DestroyDecks()
    {
        foreach (Deck currentDeck in _decks)
        {
            if (currentDeck)
                currentDeck.Destroy();
        }
    }
    #endregion
}

#region Enum
public enum TreeType : byte
{
    None = 0 ,
    Palm     ,
    Oak      ,
    Juniper  ,
    Pine
}
#endregion