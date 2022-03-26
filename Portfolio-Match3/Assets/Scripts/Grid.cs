using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public List<GameObject> listItems;
    public int gridSize = 10;
    public float padding = 10.0f;
    public float timeRateBetweenItems = 0.05f;

    private Player _playerRef;
    private Slot[,] _grid;
    private GameObject _slotPrefab;
    private int _currentX = 0;
    private int _currentY = 0;
    private Slot _currentSlot;
    private bool _isMatching = false;


    private float _timer = 0.0f;
    private int _animationX = 0;
    private int _animationY = 0;
    private bool _initAnimation = true;
    
    
    void Start()
    {
        _playerRef = GetComponent<Player>();
        _slotPrefab = Resources.Load<GameObject>("Prefabs/Slot");
        _grid = new Slot[gridSize,gridSize];

        CreateGrid();
    }

    void HandleInputs()
    {
        if (_initAnimation) return;
        if (Input.GetKeyDown(KeyCode.W)) // move up
        {
            _currentY -= 1;
            if (_currentY < 0) _currentY = 0;
        }
        if (Input.GetKeyDown(KeyCode.S)) // move down
        {
            _currentY += 1;
            if (_currentY >= gridSize) _currentY = gridSize-1;
        }
        if (Input.GetKeyDown(KeyCode.D)) // move right
        {
            _currentX += 1;
            if (_currentX >= gridSize) _currentX = gridSize-1;
        }
        if (Input.GetKeyDown(KeyCode.A)) // move left
        {
            _currentX -= 1;
            if (_currentX < 0) _currentX = 0;
        }

        if (_currentSlot != _grid[_currentX, _currentY])
        {
            if (_isMatching)
            {
                //move items
                MoveItemsInBetweenSlots(_currentSlot, _grid[_currentX, _currentY]);

                //look for matching here.
                if (_currentSlot.item.itemType == _grid[_currentX, _currentY].item.itemType)
                {
                    List<Vector2Int> match3List = CheckIfMatch3(_currentX, _currentY);
                    if (match3List.Count > 2)
                    {
                        Match3(match3List);
                    }
                }
                else
                {
                    List<Vector2Int> match3List = CheckIfMatch3(_currentX, _currentY);
                    if (match3List.Count > 2)
                    {
                        Match3(match3List);
                    }
                    List<Vector2Int> match3List2 = CheckIfMatch3(_currentSlot.x, _currentSlot.y);
                    if (match3List2.Count > 2)
                    {
                        Match3(match3List2);
                    }
                }
                

                _currentSlot.SetSelected(false);
                _currentSlot = _grid[_currentX, _currentY];
                _currentSlot.SetFocused(true);
                _isMatching = false;
            }
            else
            {
                if(_currentSlot) _currentSlot.SetFocused(false);
                _currentSlot = _grid[_currentX, _currentY];
                _currentSlot.SetFocused(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isMatching)
            {
                _currentSlot.SetFocused(true);
                _isMatching = false;
            }
            else
            {
                _currentSlot.SetSelected(true);
                _isMatching = true;
            }
        }
        
    }
    private void Update()
    {
        HandleInputs();
        InitializeItems();
    }

    public void CreateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var temp = Instantiate(_slotPrefab, transform);
                _grid[x, y] = temp.GetComponent<Slot>();
                _grid[x, y].x = x;
                _grid[x, y].y = y;
                _grid[x, y].transform.localPosition = new Vector3(x * padding, y * -padding, 0.0f);
            }
        }
    }

    public void ClearItems()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                _grid[x, y].item.Match3();
            }
        }
    }
    public void InitializeItems()
    {
        if (!_initAnimation) return;
        if (_timer >= timeRateBetweenItems)
        {
            _grid[_animationX, _animationY].AddItem(GetRandomItemFromList());
            _animationX += 1;
            if (_animationX >= gridSize)
            {
                _animationX = 0;
                _animationY += 1;
                if (_animationY >= gridSize)
                {
                    _initAnimation = false;
                }
            }

            _timer = 0.0f;
        }

        _timer += Time.deltaTime;
    }

    private List<Vector2Int> CheckIfMatch3(int x, int y)
    {

        List<Vector2Int> verticalList = GetVerticalList(x, y);
        List<Vector2Int> horizontalList = GetHorizontalList(x, y);
        List<Vector2Int> match3List = new List<Vector2Int>();

        //itself
        match3List.Add(new Vector2Int(x, y));
        
        if (verticalList.Count > 1)
        {
            foreach (var item in verticalList)
            {
                match3List.Add(item);
            }
        }
        if (horizontalList.Count > 1)
        {
            foreach (var item in horizontalList)
            {
                match3List.Add(item);
            }
        }
        
        return match3List;
    }

    private List<Vector2Int> GetHorizontalList(int x, int y)
    {
        int currentX = x;
        int currentY = y;
        List<Vector2Int> horizontalList = new List<Vector2Int>();
        //left side
        while (NeighborExits(currentX - 1, y))
        {
            currentX -= 1;
            if (_grid[currentX, currentY].item.itemType == _grid[x, y].item.itemType)
            {
                horizontalList.Add(new Vector2Int(currentX,currentY));
            }
            else
            {
                break;
            }
        }
        //right side
        currentX = x;
        while (NeighborExits(currentX + 1, y))
        {
            currentX += 1;
            if (_grid[currentX, currentY].item.itemType == _grid[x, y].item.itemType)
            {
                horizontalList.Add(new Vector2Int(currentX,currentY));
            }
            else
            {
                break;
            }
        }
        
        return horizontalList;
    }
    private List<Vector2Int> GetVerticalList(int x, int y)
    {
        int currentX = x;
        int currentY = y;
        List<Vector2Int> verticalList = new List<Vector2Int>();
        //top side
        while (NeighborExits(x, currentY - 1))
        {
            currentY -= 1;
            if (_grid[currentX, currentY].item.itemType == _grid[x, y].item.itemType)
            {
                verticalList.Add(new Vector2Int(currentX,currentY));
            }
            else
            {
                break;
            }
        }
        //bot side
        currentY= y;
        while (NeighborExits(x, currentY + 1))
        {
            currentY += 1;
            if (_grid[currentX, currentY].item.itemType == _grid[x, y].item.itemType)
            {
                verticalList.Add(new Vector2Int(currentX,currentY));
            }
            else
            {
                break;
            }
        }
        
        return verticalList;
    }

    private void Match3(List<Vector2Int> match3List)
    {
        foreach (var item in match3List)
        {
            _playerRef.score += _grid[item.x, item.y].item.points;
            _grid[item.x, item.y].item.Match3();
            _grid[item.x, item.y].item = null;
        }
        //move everything down
        //get lowest point for each column.
        List<Vector3Int> listOfColumnsToAdjust = new List<Vector3Int>();
        foreach (var item1 in match3List)
        {
            if (listOfColumnsToAdjust.Exists(item => item.x == item1.x)) continue; //column already checked
            int lowestRow = item1.y;
            int amountOfItemsInThatColumn = 1;
            foreach (var item2 in match3List)
            {
                if (item1 == item2) continue;
                if (item1.x == item2.x)
                {
                    amountOfItemsInThatColumn += 1;
                    if (item2.y > item1.y)
                    {
                        lowestRow = item2.y;
                    }
                }
            }
            listOfColumnsToAdjust.Add(new Vector3Int(item1.x, lowestRow, amountOfItemsInThatColumn));
            
        }

        foreach (var item in listOfColumnsToAdjust)
        {
            for (int i = 0; i < item.z; i++)
            {
                AdjustColumn(item.x, item.y);
            }
        }
        
    }

    private void AdjustColumn(int column, int row)
    {
        while (row != 0)
        {
            row -= 1;
            MoveItemsInBetweenSlots(_grid[column,row + 1], _grid[column,row]);
        }
    }

    private void MoveItemsInBetweenSlots(Slot slot1, Slot slot2)
    {
        var temp = slot1.item;
        slot1.AssignNewItem(slot2.item);
        slot2.AssignNewItem(temp);
    }
    private GameObject GetRandomItemFromList()
    {
        int randomIndex = Random.Range(0, listItems.Count);
        return listItems[randomIndex];
    }

    private bool NeighborExits(int x, int y)
    {
        if (x >= 0 &&
            x < gridSize &&
            y >= 0 &&
            y < gridSize)
        {
            if (_grid[x, y].item != null)
            {
                return true;
            }
        }

        return false;
    }
}
