using System.Collections.Generic;

public class RailUnion
{
    private bool isCircle = true;
    private bool isRemove = false;
    private int order = -1;

    private readonly List<Rail> rails = new List<Rail>();
    public int Power { get; private set; } = 1;

    public void Setting(int _order)
    {
        order = _order;
    }

    public void AddRail(Rail rail)
    {
        rails.Add(rail);
    }

    public void ConnectRail()
    {
        foreach (var rail in rails)
        {
            var matrix = rail.RailProperty.matrix;
            var nextMatrix = matrix;
            var preMatrix = matrix;

            switch ((EOneWay) rail.RailProperty.property.railKind)
            {
                case EOneWay.RIGHT_TO_LEFT:
                    preMatrix.x++;
                    nextMatrix.x--;
                    break;
                case EOneWay.LEFT_TO_RIGHT:
                    preMatrix.x--;
                    nextMatrix.x++;
                    break;
                case EOneWay.DOWN_TO_UP:
                    preMatrix.y++;
                    nextMatrix.y--;
                    break;
                case EOneWay.UP_TO_DOWN:
                    preMatrix.y--;
                    nextMatrix.y++;
                    break;
                case EOneWay.RIGHT_TO_DOWN:
                    preMatrix.x++;
                    nextMatrix.y++;
                    break;
                case EOneWay.DOWN_TO_LEFT:
                    preMatrix.y++;
                    nextMatrix.x--;
                    break;
                case EOneWay.DOWN_TO_RIGHT:
                    preMatrix.y++;
                    nextMatrix.x++;
                    break;
                case EOneWay.LEFT_TO_DOWN:
                    preMatrix.x--;
                    nextMatrix.y++;
                    break;
                case EOneWay.UP_TO_RIGHT:
                    preMatrix.y--;
                    nextMatrix.x++;
                    break;
                case EOneWay.LEFT_TO_UP:
                    preMatrix.x--;
                    nextMatrix.y--;
                    break;
                case EOneWay.RIGHT_TO_UP:
                    preMatrix.x++;
                    nextMatrix.y--;
                    break;
                case EOneWay.UP_TO_LEFT:
                    preMatrix.y--;
                    nextMatrix.x--;
                    break;
            }

            foreach (var anotherRail in rails)
            {
                if (anotherRail.RailProperty.matrix == preMatrix) rail.SetPreRail(anotherRail);
                if (anotherRail.RailProperty.matrix == nextMatrix) rail.SetNextRail(anotherRail);
            }
        }

        Rail firstRail = null;

        foreach (var rail in rails)
            if (rail.PreRail == null)
            {
                isCircle = false;
                firstRail = rail;
            }

        if (isCircle)
        {
            rails.Sort(delegate(Rail A, Rail B)
            {
                if (A.RailProperty.matrix.x > B.RailProperty.matrix.x) return -1;

                if (A.RailProperty.matrix.x < B.RailProperty.matrix.x) return 1;

                if (A.RailProperty.matrix.y > B.RailProperty.matrix.y) return 1;
                if (A.RailProperty.matrix.y < B.RailProperty.matrix.y) return -1;
                return 0;
            });
        }
        else
        {
            var tempRail = new List<Rail>();

            var nextRail = firstRail;
            while (nextRail != null)
            {
                tempRail.Add(nextRail);
                nextRail = nextRail.NextRail;
            }

            rails.Clear();
            foreach (var item in tempRail) rails.Add(item);
        }


        Power = rails[0].RailProperty.property.power;
    }

    public void Moving()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play(SoundManager.GetInstance.Gear);

        GameTile firstTile = null;
        Rail firstRail = null;

        if (!isCircle)
            foreach (var rail in rails)
                if (rail.PreRail == null)
                {
                    firstRail = rail;
                    firstTile = rail.Tile;
                }

        if (isCircle)
        {
            firstTile = rails[0].Tile;
            firstRail = rails[0];
        }

        var preTile = firstTile;
        GameTile nextTile = null;

        GameBlock tempBlock = null;
        var moveWaitBlock = preTile.NormalBlock;

        GameBlock tempTopBlock = null;
        var moveWaitTopBlock = preTile.GetTopBlockOrNull();

        for (var i = 0; i < rails.Count; i++)
        {
            var isLastRail = false;

            if (firstRail.NextRail != null)
            {
                nextTile = firstRail.NextRail.Tile;
            }
            else if (isCircle && i == rails.Count - 1)
            {
                nextTile = firstTile;
            }
            else
            {
                isLastRail = true;
                nextTile = firstTile;
            }

            tempBlock = nextTile.NormalBlock;
            tempTopBlock = nextTile.GetTopBlockOrNull();


            if (moveWaitBlock != null)
            {
                if (!isLastRail || isCircle)
                {
                    nextTile.RemoveBlock(nextTile.NormalBlock);
                    moveWaitBlock.Drop(nextTile);
                }
                else
                {
                    // 마지막 순서를 가진 블록이 첫번째 순서로 이동하는 상황
                    // 블록 이동 필요
                    var targetMatrix = TileManager.GetInstance.GetTilePosition(firstTile.Matrix);

                    switch (rails[0].PreDirection)
                    {
                        case EDirection.DOWN:
                            targetMatrix.y -= TileManager.GetInstance.TileSize.y;
                            break;
                        case EDirection.UP:
                            targetMatrix.y += TileManager.GetInstance.TileSize.y;
                            break;
                        case EDirection.LEFT:
                            targetMatrix.x -= TileManager.GetInstance.TileSize.x;
                            break;
                        case EDirection.RIGHT:
                            targetMatrix.x += TileManager.GetInstance.TileSize.x;
                            break;
                    }

                    //moveWaitBlock.transform.position = targetMatrix;

                    nextTile.RemoveBlock(nextTile.NormalBlock);
                    moveWaitBlock.isDropMoving = false;
                    moveWaitBlock.EnterTunnel(nextTile, rails[i].NextDirection, firstRail.NextDirection);
                    //moveWaitBlock.Move(nextTile);
                }
            }
            else
            {
                nextTile.RemoveBlock(nextTile.NormalBlock);
            }

            if (moveWaitTopBlock != null)
            {
                if (!isLastRail || isCircle)
                {
                    nextTile.RemoveBlock(nextTile.GetTopBlockOrNull());
                    //moveWaitTopBlock.Drop(nextTile);
                    moveWaitTopBlock.Move(nextTile);
                }
                else
                {
                    // 마지막 순서를 가진 블록이 첫번째 순서로 이동하는 상황
                    // 블록 이동 필요
                    var targetMatrix = TileManager.GetInstance.GetTilePosition(firstTile.Matrix);

                    switch (rails[0].PreDirection)
                    {
                        case EDirection.DOWN:
                            targetMatrix.y -= TileManager.GetInstance.TileSize.y;
                            break;
                        case EDirection.UP:
                            targetMatrix.y += TileManager.GetInstance.TileSize.y;
                            break;
                        case EDirection.LEFT:
                            targetMatrix.x -= TileManager.GetInstance.TileSize.x;
                            break;
                        case EDirection.RIGHT:
                            targetMatrix.x += TileManager.GetInstance.TileSize.x;
                            break;
                    }

                    //moveWaitTopBlock.transform.position = targetMatrix;

                    nextTile.RemoveBlock(nextTile.GetTopBlockOrNull());
                    moveWaitTopBlock.isDropMoving = false;
                    moveWaitTopBlock.EnterTunnel(nextTile, rails[i].NextDirection, firstRail.NextDirection);
                }
            }
            else
            {
                nextTile.RemoveBlock(nextTile.GetTopBlockOrNull());
            }

            moveWaitBlock = tempBlock;
            moveWaitTopBlock = tempTopBlock;
            firstRail = firstRail.NextRail;
        }
    }
}