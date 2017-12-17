public static class PlayerAssignation
{
    public static bool[] playerSet = new bool[2];
    public static int[] playerControllerIndices = new int[2];

    public static bool isValid
    {
        get { return playerSet[0] && playerSet[1]; }
    }

    public static int GetPlayerControllerIndex(int index)
    {
        return playerSet[index] ? playerControllerIndices[index] : index;
    }

    public static void Set(int id, int controllerIndex)
    {
        if (playerControllerIndices[1 - id] == controllerIndex)
        {
            playerSet[1 - id] = false;
        }

        playerSet[id] = true;
        playerControllerIndices[id] = controllerIndex;
    }

    public static void Remove(int controllerIndex)
    {
        if (playerControllerIndices[0] == controllerIndex)
        {
            playerSet[0] = false;
        }
        else
        if (playerControllerIndices[1] == controllerIndex)
        {
            playerSet[1] = false;
        }
    }

    public static void Clear()
    {
        playerSet[0] = false;
        playerSet[1] = false;
    }
}
