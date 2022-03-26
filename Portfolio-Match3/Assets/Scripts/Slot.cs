using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Slot : MonoBehaviour
{
    public int x;
    public int y;
    public Item item;
    
    private GameObject _itemsParent;
    private SpriteRenderer _sprite;
    private Color _color;

    private readonly string _hexYellowColor = "#FFD700";
    private readonly string _hexRedColor = "#FF0006";
    private readonly float _alphaSelected = 0.4f;
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _itemsParent = GameObject.Find("Items");
        SetFocused(false);
    }

    public void AddItem(GameObject newItem)
    {
        var temp = Instantiate(newItem, _itemsParent.transform);
        var component = temp.GetComponent<Item>();
        component.slotPosition = transform.position;
        component.FallFromTop();
        item = component;
    }

    public void Match3()
    {
        //set item to destroy.

        item = null;
    }

    public Item GetItem()
    {
        return item;
    }

    public void AssignNewItem(Item newItem)
    {
        item = newItem;
        if(item)
            item.MoveToSlot(transform.position);
    }
    public void SetFocused(bool state)
    {
        SetColor(_hexYellowColor, state ? _alphaSelected : 0.0f);
    }

    public void SetSelected(bool state)
    {
        SetColor(_hexRedColor, state ? _alphaSelected : 0.0f);
    }

    private void SetColor(string hexColor, float alpha)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            _color = newColor;
            _color.a = alpha;
        }
        _sprite.color = _color;
    }
}
