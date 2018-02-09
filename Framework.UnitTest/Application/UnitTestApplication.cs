﻿namespace UnitTest.Application
{
    using Database.UnitTest.Application;
    using Framework;
    using Framework.Application;
    using Framework.Component;

    public class UnitTestApplication : UnitTestBase
    {
        public void Grid()
        {
            UtilFramework.UnitTest(typeof(MyApp)); // Enable InMemory database.
            var app = new MyApp();
            AppJson appJson = app.Run(null);

            GridName gridName = new GridName<MyRow>();
            string gridNameJson = UtilApplication.GridNameToJson(gridName);

            UtilFramework.Assert(appJson.GridDataJson.RowList[gridNameJson][0].Index == "Filter");
            UtilFramework.Assert(appJson.GridDataJson.CellList[gridNameJson]["IsActive"]["Filter"].T == null); // No text in filter if bool not nullable.
            {
                // string json = Framework.Json.JsonConvert.Serialize(appJson, app.TypeComponentInNamespace());
                // UtilServer.StartUniversalServer();
                // string url = "http://localhost:4000/Universal/index.js"; // Call Universal server when running in Visual Studio.
                // string html = UtilServer.WebPost(url, json, true).Result;
            }
        }
    }
}

namespace UnitTest.Application
{
    using Framework.Application;
    using Framework.Component;
    using Database.UnitTest.Application;
    using System;

    public class MyApp : App
    {
        protected internal override Type TypePageMain()
        {
            return typeof(MyPage);
        }
    }

    public class MyPage : Page
    {
        protected internal override void InitJson(App app)
        {
            new Grid(app.AppJson, new GridName<MyRow>());
        }
    }
}

namespace Database.UnitTest.Application
{
    using Framework.DataAccessLayer;

    [SqlTable("dbo", "MyRow")]
    public class MyRow : Row
    {
        [SqlColumn(null, null, true)]
        public int Id { get; set; }

        [SqlColumn("Text", typeof(MyRow_Text))]
        public string Text { get; set; }

        [SqlColumn("Text2", typeof(MyRow_Text))]
        public string Text2 { get; set; }

        [SqlColumn("IsActive", null)]
        public bool IsActive { get; set; }
    }

    public class MyRow_Text : Cell<MyRow>
    {

    }
}
