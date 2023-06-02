using System.Collections.Generic;

public class ObConnectionUnion
{
    public List<ObConnection> connections = new List<ObConnection>();

    private bool isCircle = true;

    public int Order { get; set; }

    public void AddConnection(ObConnection connection)
    {
        connections.Add(connection);
    }

    public void ConnectConnections()
    {
        foreach (var connection in connections)
        {
            var matrix = connection.ConnectionProperty.matrix;
            var nextMatrix = matrix;
            var preMatrix = matrix;

            switch (connection.ConnectionProperty.property.direction)
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

            foreach (var anotherConnection in connections)
            {
                if (anotherConnection.ConnectionProperty.matrix == preMatrix)
                    connection.SetPreConnection(anotherConnection);
                if (anotherConnection.ConnectionProperty.matrix == nextMatrix)
                    connection.SetNextConnection(anotherConnection);
            }
        }

        ObConnection firstConnection = null;

        foreach (var connection in connections)
            if (connection.PreConnection == null)
            {
                isCircle = false;
                firstConnection = connection;
            }

        if (isCircle)
        {
            connections.Sort(delegate(ObConnection A, ObConnection B)
            {
                if (A.ConnectionProperty.matrix.x > B.ConnectionProperty.matrix.x) return -1;

                if (A.ConnectionProperty.matrix.x < B.ConnectionProperty.matrix.x) return 1;

                if (A.ConnectionProperty.matrix.y > B.ConnectionProperty.matrix.y) return 1;
                if (A.ConnectionProperty.matrix.y < B.ConnectionProperty.matrix.y) return -1;
                return 0;
            });
        }
        else
        {
            var tempConnection = new List<ObConnection>();
            var count = 0;
            var nextConnection = firstConnection;
            while (nextConnection != null)
            {
                count++;
                if (count > 1000) break;
                tempConnection.Add(nextConnection);
                nextConnection = nextConnection.NextConnection;
            }

            connections.Clear();
            foreach (var item in tempConnection) connections.Add(item);
        }
    }
}