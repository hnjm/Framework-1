﻿namespace Framework.Tool
{
    public class CommandCheck : Command
    {
        public CommandCheck() 
            : base("check", "Test all ConnectionManager values")
        {

        }

        public override void Run()
        {
            ConnectionManagerCheck.Run();
        }
    }
}
