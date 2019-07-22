﻿namespace Framework.App
{
    using Framework.Server;
    using Framework.Json;
    using System.Threading.Tasks;
    using System.Linq;
    using Framework.Session;
    using System.Reflection;
    using Framework.DataAccessLayer;
    using System.Collections.Generic;
    using static Framework.Session.UtilSession;
    using System;

    internal static class UtilApp
    {
        /// <summary>
        /// Process button click.
        /// </summary>
        public static async Task ProcessButtonAsync()
        {
            var app = UtilServer.AppInternal;
            foreach (Button button in app.AppJson.ComponentListAll().OfType<Button>().Where(item => item.IsClick))
            {
                await button.ComponentOwner<Page>().ButtonClickAsync(button);
                button.IsClick = false;
            }
        }

        /// <summary>
        /// Process bootstrap modal dialog window.
        /// </summary>
        public static void ProcessBootstrapModal()
        {
            var app = UtilServer.AppInternal;
            app.AppJson.IsBootstrapModal = false;
            BootstrapModal.DivModalBackdropRemove(app.AppJson);
            bool isExist = false;
            foreach (var item in app.AppJson.ComponentListAll().OfType<BootstrapModal>())
            {
                item.ButtonClose()?.ComponentMoveLast();
                isExist = true;
            }
            if (isExist)
            {
                app.AppJson.IsBootstrapModal = true;
                BootstrapModal.DivModalBackdropCreate(app.AppJson);
            }
        }

        /// <summary>
        /// Process navbar button click.
        /// </summary>
        public static async Task ProcessBootstrapNavbarAsync()
        {
            var app = UtilServer.AppInternal;
            foreach (BootstrapNavbar navbar in app.AppJson.ComponentListAll().OfType<BootstrapNavbar>())
            {
                if (navbar.ButtonList != null)
                {
                    foreach (BootstrapNavbarButton button in navbar.ButtonList)
                    {
                        if (button.IsClick)
                        {
                            button.IsClick = false;
                            if (navbar.GridIndex != null)
                            {
                                GridItem gridItem = UtilSession.GridItemList().Where(item => item.GridIndex == navbar.GridIndex).First();

                                // Set IsSelect
                                // See also method ProcessGridRowIsClick();
                                foreach (GridRowItem gridRowItem in gridItem.GridRowList)
                                {
                                    if (gridRowItem.GridRowSession != null) // Outgoing grid might have less rows
                                    {
                                        gridRowItem.GridRowSession.IsSelect = false;
                                    }
                                }
                                foreach (GridRowItem gridRowItem in gridItem.GridRowList)
                                {
                                    if (gridRowItem.GridRowSession != null && gridRowItem.RowIndex == button.RowIndex)
                                    {
                                        gridRowItem.GridRowSession.IsSelect = true;
                                        break;
                                    }
                                }
                                if (gridItem.Grid == null)
                                {
                                    throw new Exception("Grid has been removed! Use property Grid.IsHide instead.");
                                }
                                await gridItem.Grid.ComponentOwner<Page>().GridRowSelectedAsync(gridItem.Grid);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Div.IsListDiv flag.
        /// </summary>
        public static void BootstrapRowRender()
        {
            var app = UtilServer.AppInternal;
            foreach (var bootstrapRow in app.AppJson.ComponentListAll().OfType<BootstrapRow>())
            {
                bootstrapRow.CssClassAdd("row");
                List<ComponentJson> listRemove = new List<ComponentJson>();
                foreach (var item in bootstrapRow.List)
                {
                    if (item.GetType() != typeof(Div)) // ComponentJson.Type is not evalueted on BootstrapRow children!
                    {
                        listRemove.Add(item);
                    }
                }
                foreach (var item in listRemove)
                {
                    bootstrapRow.List.Remove(item);
                }
            }
        }

        public static void BootstrapNavbarRender()
        {
            var app = UtilServer.AppInternal;
            foreach (BootstrapNavbar navbar in app.AppJson.ComponentListAll().OfType<BootstrapNavbar>())
            {
                navbar.ButtonList = new List<BootstrapNavbarButton>();
                if (navbar.GridIndex != null)
                {
                    GridSession gridSession = UtilSession.GridSessionFromIndex(navbar.GridIndex.Value);

                    PropertyInfo propertyInfo = UtilDalType.TypeRowToPropertyInfoList(gridSession.TypeRow).Where(item => item.Name == "Text" && item.PropertyType == typeof(string)).SingleOrDefault();
                    if (propertyInfo != null)
                    {
                        for (int rowIndex = 0; rowIndex < gridSession.GridRowSessionList.Count; rowIndex++)
                        {
                            GridRowSession gridRowSession = gridSession.GridRowSessionList[rowIndex];
                            if (gridRowSession.RowEnum == GridRowEnum.Index)
                            {
                                string text = (string)propertyInfo.GetValue(gridRowSession.Row);
                                bool isActive = gridRowSession.IsSelect;
                                BootstrapNavbarButton button = new BootstrapNavbarButton() { RowIndex = rowIndex, TextHtml = text, IsActive = isActive };
                                navbar.ButtonList.Add(button);
                            }
                        }
                    }
                }
            }
        }
    }
}
