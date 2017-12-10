﻿using Discord;
using System.Collections.Generic;
using System.Linq;

namespace dotbot.Services
{
    public class PollService
    {
        public Dictionary<ulong, Poll> currentPolls;

        public PollService()
        {
            currentPolls = new Dictionary<ulong, Poll>();
        }
    }

    public class Poll
    {
        public List<PollOption> Options { get; set; }
        public bool IsOpen { get; set; }
        public IUser Owner { get; set; }

        public PollOption GetWinner => Options.OrderBy(o => o.Votes).Last();

    }

    public class PollOption
    {
        public string Text { get; set; }
        public int Votes { get; set; }
    }
}