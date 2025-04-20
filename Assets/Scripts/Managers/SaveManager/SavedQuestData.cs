using System;
using System.Collections.Generic;


[Serializable]
public class SavedQuestData
{
    public string questName;

    public string giverID;

    public List<RequirementProgress> progress;
}
