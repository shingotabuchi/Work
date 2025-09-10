namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class TVDatabase
    {
        public List<TVChannel> channels;
        public List<TVProgram> programs;
    }

    [Serializable]
    public class TVChannel
    {
        public int channel_id;
        public string name;
        public int sort_order;
    }

    [Serializable]
    public class TVProgram
    {
        public int channel_id;
        public string start_time;
        public string end_time;
        public string title;
    }
}
