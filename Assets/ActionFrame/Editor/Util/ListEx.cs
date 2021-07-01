using System.Collections.Generic;
using ActionFrame.Runtime;

namespace ActionFrame.Editor
{
    public static class ListEx
    {
        public static bool IsItemExist(this List<StateData> self, string state)
        {
            foreach (var stateData in self)
            {
                if (stateData.StateName == state)
                {
                    return true;
                }
            }
            return false;
        }

        public static StateData GetStateWithName(this List<StateData> self, string stateName)
        {
            foreach (var stateData in self)
            {
                if (stateData.StateName == stateName)
                {
                    return stateData;
                }
            }
            return null;
        }

        public static int GetIndexInStrList(this string[] self, string str)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i] == str)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}