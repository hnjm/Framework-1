﻿namespace Framework.BuildTool
{
    public class CommandGenerate : Command
    {
        public CommandGenerate() 
            : base("generate", "Generate CSharp DTO's")
        {

        }

        public override void Run()
        {
            DataAccessLayer.Script.Run();
            UtilFramework.Log(string.Format("File updated. ({0})", DataAccessLayer.ConnectionManager.DatabaseGenerateFileName));
        }
    }
}
