namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using EnhancedUI.Helpers;

    /// <summary>
	/// This class converts linear data into a matrix of program, channels, and time slots.
	/// It will fill in empty slots to ensure each group (row) of the grid is fully filled out.
	///
	/// Note: the data doesn't have to be stored in any particular order. This class will ensure everything
	/// is sorted properly.
	///
	/// Note: this class logic isn't specific to EnhancedGrid, so comments will be minimal
	/// </summary>
    public class ModelProgramMatrix
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int TotalTimeSlots { get; private set; }
        public int TotalProgramCount { get; private set; }
        public int SelectedProgramIndex { get; private set; }
        public int SelectedTimeSlotIndex { get; private set; }
        public int SelectedChannelIndex { get; private set; }
        public ModelProgram SelectedProgram { get; private set; }
        public List<ModelChannel> Channels { get; private set; }
        public List<ModelTimeSlot> TimeSlots { get; private set; }

        public ModelProgramMatrix(TextAsset databaseData)
        {
            Reload(databaseData);
        }

        public void Reload(TextAsset databaseData)
        {
            StartTime = DateTime.MaxValue;
            EndTime = DateTime.MinValue;

            if (Channels == null) Channels = new List<ModelChannel>();
            if (TimeSlots == null) TimeSlots = new List<ModelTimeSlot>();

            Channels.Clear();
            TimeSlots.Clear();

            SelectedProgramIndex = -1;
            SelectedProgram = null;
            TotalProgramCount = 0;

            // load the database from the json file
            var database = JsonUtility.FromJson<TVDatabase>(databaseData.text);

            // sort the channels by their sort_order value
            database.channels = database.channels.OrderBy(c => c.sort_order).ThenBy(c => c.channel_id).ToList();

            // add channels

            for (var i = 0; i < database.channels.Count; i++)
            {
                var c = database.channels[i];

                Debug.Assert(Channels.Where(x => x.channelID == c.channel_id).FirstOrDefault() == null, $"There is already a channel with channel_id = {c.channel_id}");

                Channels.Add(new ModelChannel()
                {
                    index = i,
                    channelID = c.channel_id,
                    name = c.name,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    programs = new List<ModelProgram>()
                });
            }

            // add programs

            foreach (var program in database.programs)
            {
                var newProgram = new ModelProgram()
                {
                    channelID = program.channel_id,
                    startTime = _ConvertStringToTime(program.start_time),
                    endTime = _ConvertStringToTime(program.end_time),
                    title = program.title
                };

                newProgram.timeSlotCount = _CalculateTimeSlotCount(newProgram.startTime, newProgram.endTime);

                var channel = Channels.Where(c => c.channelID == newProgram.channelID).FirstOrDefault();

                Debug.Assert(channel != null, $"No channel found for channel ID {newProgram.channelID}");

                channel.programs.Add(newProgram);

                if (newProgram.startTime < channel.StartTime) channel.StartTime = newProgram.startTime;
                if (newProgram.endTime > channel.EndTime) channel.EndTime = newProgram.endTime;
            }

            // fill in empty slots so that each group (row) in the grid is fully stocked with program slots

            _FillInMatrix();

            // add in time slots at 30 minute intervals

            var currentTime = StartTime;
            for (var i = 0; i < TotalTimeSlots; i++)
            {
                TimeSlots.Add(new ModelTimeSlot()
                {
                    index = i,
                    startTime = currentTime
                });

                currentTime = currentTime.AddMinutes(ModelTimeSlot.TIME_SLOT_MINUTES);
            }

            var index = 0;

            foreach (var c in Channels)
            {
                foreach (var p in c.programs)
                {
                    p.index = index;
                    index++;
                }

                TotalProgramCount += c.programs.Count();
            }

            _SetSelection(0);

            SelectedTimeSlotIndex = 0;
        }

        /// <summary>
		/// Looks up a program by a data index, scanning through channel
		/// programs until it finds the correct one
		/// </summary>
		/// <param name="dataIndex">The data index to look up</param>
		/// <returns></returns>
        public ModelProgram GetProgram(int dataIndex)
        {
            foreach (var c in Channels)
            {
                var p = c.programs.Where(p => p.index == dataIndex).FirstOrDefault();
                if (p != null) return p;
            }

            return null;
        }

        public ModelChannel GetProgramChannel(int channelID)
        {
            return Channels.Where(c => c.channelID == channelID).FirstOrDefault();
        }

        public ModelTimeSlot GetTimeSlot(DateTime startTime)
        {
            return TimeSlots.Where(t => t.startTime == startTime).FirstOrDefault();
        }

        /// <summary>
		/// Changes the selection by going left one, unless the selection is
		/// at the first time slot of a channel
		/// </summary>
        public void MoveSelectionLeft()
        {
            if (SelectedProgramIndex == 0) return;

            var newProgram = GetProgram(SelectedProgramIndex - 1);

            if (SelectedProgram.channelID != newProgram.channelID) return;

            var timeSlot = GetTimeSlot(newProgram.startTime);
            SelectedTimeSlotIndex = timeSlot.index;

            _SetSelection(SelectedProgramIndex - 1);
        }

        /// <summary>
		/// Changes the selection by going right one, unless the selection is
		/// at the last time slot of a channel
		/// </summary>
        public void MoveSelectionRight()
        {
            if (SelectedProgramIndex == TotalProgramCount - 1) return;

            var newProgram = GetProgram(SelectedProgramIndex + 1);

            if (SelectedProgram.channelID != newProgram.channelID) return;

            var timeSlot = GetTimeSlot(newProgram.startTime);
            SelectedTimeSlotIndex = timeSlot.index;

            _SetSelection(SelectedProgramIndex + 1);
        }

        /// <summary>
		/// Changes the selection by switching channels up. It will try to stay in the same
		/// time slot unless the program spans out of that slot
		/// </summary>
        public void MoveSelectionUp()
        {
            if (SelectedChannelIndex == 0) return;

            var newChannel = Channels[SelectedChannelIndex - 1];

            var timeSlot = TimeSlots[SelectedTimeSlotIndex];

            foreach (var p in newChannel.programs)
            {
                if (p.startTime < timeSlot.MidPoint && p.endTime > timeSlot.MidPoint)
                {
                    _SetSelection(p);
                    return;
                }
            }
        }

        /// <summary>
		/// Changes the selection by switching channels down. It will try to stay in the same
		/// time slot unless the program spans out of that slot
		/// </summary>
        public void MoveSelectionDown()
        {
            if (SelectedChannelIndex == Channels.Count - 1) return;

            var newChannel = Channels[SelectedChannelIndex + 1];

            var timeSlot = TimeSlots[SelectedTimeSlotIndex];

            foreach (var p in newChannel.programs)
            {
                if (p.startTime < timeSlot.MidPoint && p.endTime > timeSlot.MidPoint)
                {
                    _SetSelection(p);
                    return;
                }
            }
        }

        private void _SetSelection(ModelProgram newProgram)
        {
            var dataIndex = 0;

            foreach (var c in Channels)
            {
                foreach (var p in c.programs)
                {
                    if (p == newProgram)
                    {
                        _SetSelection(dataIndex);
                        return;
                    }
                    else
                    {
                        dataIndex++;
                    }
                }
            }
        }

        private void _SetSelection(int programIndex)
        {
            programIndex = Mathf.Clamp(programIndex, 0, TotalProgramCount - 1);

            var program = GetProgram(programIndex);

            Debug.Assert(program != null, $"No program found at data index {programIndex}");

            SelectedProgramIndex = programIndex;
            SelectedProgram = program;

            var channel = GetProgramChannel(program.channelID);
            SelectedChannelIndex = channel.index;

            foreach (var c in Channels)
            {
                c.isSelected = (c.index == SelectedChannelIndex);

                c.programs.ForEach(p => p.isSelected = p == SelectedProgram);
            }

            TimeSlots.ForEach(t => t.isSelected = t.index == SelectedTimeSlotIndex);
        }

        /// <summary>
		/// Creats empty programs to fill in the matrix so that every channel uses the
		/// same time slots
		/// </summary>
        private void _FillInMatrix()
        {
            foreach (var c in Channels)
            {
                if (c.StartTime < StartTime) StartTime = c.StartTime;
                if (c.EndTime > EndTime) EndTime = c.EndTime;
            }

            TotalTimeSlots = _CalculateTimeSlotCount(StartTime, EndTime);

            foreach (var c in Channels)
            {
                if (c.StartTime == DateTime.MaxValue) c.StartTime = StartTime;
                if (c.EndTime == DateTime.MinValue) c.EndTime = StartTime;

                c.programs = c.programs.OrderBy(p => p.startTime).ToList();

                var p = 0;

                while (p < c.programs.Count())
                {
                    if (p == 0 && c.programs[p].startTime > StartTime)
                    {
                        c.programs.Insert(0, new ModelProgram()
                        {
                            channelID = c.channelID,
                            startTime = StartTime,
                            endTime = c.programs[p].startTime,
                            timeSlotCount = _CalculateTimeSlotCount(StartTime, c.programs[p].startTime),
                            title = "No Data"
                        });

                        c.StartTime = StartTime;
                    }
                    else if (p == c.programs.Count() - 1 && c.EndTime < EndTime)
                    {
                        c.programs.Add(new ModelProgram()
                        {
                            channelID = c.channelID,
                            startTime = c.EndTime,
                            endTime = EndTime,
                            timeSlotCount = _CalculateTimeSlotCount(c.EndTime, EndTime),
                            title = "No Data"
                        });

                        c.EndTime = EndTime;
                    }
                    else if (p > 0 && c.programs[p - 1].endTime != c.programs[p].startTime)
                    {
                        c.programs.Insert(p, new ModelProgram()
                        {
                            channelID = c.channelID,
                            startTime = c.programs[p - 1].endTime,
                            endTime = c.programs[p].startTime,
                            timeSlotCount = _CalculateTimeSlotCount(c.programs[p - 1].endTime, c.programs[p].startTime),
                            title = "No Data"
                        });
                    }
                    else
                    {
                        p++;
                    }
                }

                if (c.programs.Count() == 0)
                {
                    c.programs.Insert(p, new ModelProgram()
                    {
                        channelID = c.channelID,
                        startTime = StartTime,
                        endTime = EndTime,
                        timeSlotCount = _CalculateTimeSlotCount(StartTime, EndTime),
                        title = "No Data"
                    });
                }
            }
        }

        private DateTime _ConvertStringToTime(string time)
        {
            return DateTime.ParseExact(time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        private int _CalculateTimeSlotCount(DateTime StartTime, DateTime EndTime)
        {
            var minutes = Convert.ToInt32((EndTime - StartTime).TotalMinutes);
            return minutes / ModelTimeSlot.TIME_SLOT_MINUTES;
        }
    }

    public class ModelChannel
    {
        public int index;
        public int channelID;
        public string name;
        public List<ModelProgram> programs;
        public DateTime StartTime = DateTime.MaxValue;
        public DateTime EndTime = DateTime.MinValue;
        public bool isSelected;
    }

    public class ModelProgram
    {
        public int index;
        public int channelID;
        public DateTime startTime;
        public DateTime endTime;
        public int timeSlotCount;
        public string title;
        public bool isSelected;
    }

    public class ModelTimeSlot
    {
        public const int TIME_SLOT_MINUTES = 30;

        public int index;
        public DateTime startTime;
        public bool isSelected;

        public DateTime MidPoint
        {
            get
            {
                return (startTime.AddMinutes(TIME_SLOT_MINUTES / 2));
            }
        }
    }
}
