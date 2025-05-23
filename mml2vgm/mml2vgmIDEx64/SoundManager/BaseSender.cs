﻿using Corex64;
using mml2vgmIDEx64;

namespace SoundManager
{
    public class BaseSender : BaseMakerSender
    {
        protected outDatum od = null;
        protected long Counter = 0;
        protected Chip Chip = new Chip(1);
        protected EnmDataType Type = 0;
        protected int Address = 0;
        protected int Data = 0;
        protected object ExData = null;

        protected int ringBufferSize;
    }

}
