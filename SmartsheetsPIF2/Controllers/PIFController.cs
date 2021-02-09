﻿using Microsoft.AspNetCore.Mvc;
using SmartsheetsPIF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smartsheet.Api.Models;
using Smartsheet.Api;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartsheetsPIF2.Controllers
{
    public class PIFController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public static long sheetId = 508222156105604;

        [HttpGet]
        public IActionResult ListPIFs()
        {
            var sheet = LoadSheet(sheetId, initSheet());
            return View(GetRows(sheet));
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {
            return View(GetProject(id));
        }

        [HttpGet]
        public IActionResult Details(long id)
        {
            return View(GetProject(id));
        }

        [HttpPost]
        public IActionResult Save(PIFModel model)
        {
            updateRow(model);
            return View("Index");
        }
        public SmartsheetClient initSheet()
        {
            // Initialize client
            SmartsheetClient smartsheet_CL = new SmartsheetBuilder().SetAccessToken("voic91jwhahv2mws2cow5frz96")
                // TODO: Set your API access in environment variable SMARTSHEET_ACCESS_TOKEN or else here
                .Build();
            return smartsheet_CL;
        }
        public Sheet LoadSheet(long sheetId, SmartsheetClient smartsheet_CL)
        {
            // Load the entire sheet
            var sheet = smartsheet_CL.SheetResources.GetSheet(
                sheetId,           // long sheetId
                null,                       // IEnumerable<SheetLevelInclusion> includes
                null,                       // IEnumerable<SheetLevelExclusion> excludes
                null,                       // IEnumerable<long> rowIds
                null,                       // IEnumerable<int> rowNumbers
                null,                       // IEnumerable<long> columnIds
                null,                       // Nullable<long> pageSize
                null                        // Nullable<long> page
            );
            Console.WriteLine("Loaded " + sheet.Rows.Count + " rows from sheet: " + sheet.Name);
            return sheet;
        }
        public List<PIFModel> GetRows(Sheet sheet)
        {
            List<PIFModel> pif_list = new List<PIFModel>();
            //Get_lobs_picklist(sheet.GetColumnByIndex(0));

            foreach (var row in sheet.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row.Cells.ElementAt(0).DisplayValue)) {
                    PIFModel pif = new PIFModel();
                    pif.pif_Id = (long)row.Id;
                    foreach (var cell in row.Cells)
                    {
                        long columnid = cell.ColumnId.Value;
                        string columnName = sheet.GetColumnById(columnid).Title.ToString();
                        Console.WriteLine("Column Name: " + columnName + " -- Cell Value: " + cell.DisplayValue);
                        switch (columnName)
                        {

                            case "LOB":
                                pif.lob = cell.DisplayValue;
                                break;

                            case "Project":
                                pif.projectName = cell.DisplayValue;
                                break;

                            case "Status":
                                pif.status = cell.DisplayValue;
                                break;

                            case "Team":
                                pif.team = cell.DisplayValue;                                //pif.endDate = DateTime.ParseExact(cell.Value.ToString(), "mm,dd,yyyy", null);
                                break;

                            case "Start":
                                pif.startDate = Convert.ToDateTime(cell.Value);
                                //pif.startDate = DateTime.ParseExact( cell.Value.ToString(), "mm,dd,yyyy",null);
                                break;

                            case "Ship":
                                pif.endDate = Convert.ToDateTime(cell.Value);
                                //pif.endDate = DateTime.ParseExact(cell.Value.ToString(), "mm,dd,yyyy", null);
                                break;

                        }
                    }
                    pif_list.Add(pif);
                }
            }
            return pif_list;
        }
        public PIFModel GetProject(long row_id)
        {
            PIFModel pif = new PIFModel();
            pif.pif_Id = row_id;
            Sheet sheet = LoadSheet(sheetId, initSheet());

            //Get LOB options
            pif.lob_options = Get_lobs_picklist(sheet.GetColumnByIndex(0));
            //Get Status options
            pif.status_options = Get_status_picklist(sheet.GetColumnByIndex(3));
            //Get Team options
            pif.team_options = Get_status_picklist(sheet.GetColumnByIndex(12));


            foreach (var row in sheet.Rows)
            {
                if (row.Id == row_id) {
                    foreach (var cell in row.Cells)
                    {
                        long columnid = cell.ColumnId.Value;
                        string columnName = sheet.GetColumnById(columnid).Title.ToString();
                        Console.WriteLine("Column Name: " + columnName + " -- Cell Value: " + cell.DisplayValue);
                        switch (columnName)
                        {
                            case "LOB":
                                pif.lob = cell.DisplayValue;
                                break;

                            case "Project":
                                pif.projectName = cell.DisplayValue;
                                break;

                            case "Status":
                                pif.status = cell.DisplayValue;
                                break;

                            case "Start":
                                pif.startDate = Convert.ToDateTime(cell.Value);
                                //pif.startDate = DateTime.ParseExact( cell.Value.ToString(), "mm,dd,yyyy",null);
                                break;

                            case "Ship":
                                pif.endDate = Convert.ToDateTime(cell.Value);
                                //pif.endDate = DateTime.ParseExact(cell.Value.ToString(), "mm,dd,yyyy", null);
                                break;
                            case "PM":
                                pif.producer = cell.DisplayValue;
                                break;

                            case "Type":
                                pif.requestType = cell.DisplayValue;
                                break;

                            case "Team":
                                pif.team = cell.DisplayValue;
                                break;

                        }

                    }
                }
            }
            return pif;
        }
        public void updateRow(PIFModel pif)
        {
            SmartsheetClient smartsheet_CL = initSheet();
            Sheet sheet = LoadSheet(sheetId, smartsheet_CL);

            int row_number = 0;
            foreach (var row_b in sheet.Rows)
            {
                if (row_b.Id == pif.pif_Id) 
                {
                    row_number = row_b.RowNumber.Value;
                }
            }

            Row row = sheet.GetRowByRowNumber(row_number);
            var rowToTupdate = new Row ();

            var lob_cell = new Cell();
            var project_cell = new Cell();
            var status_cell = new Cell();
            //var pm_cell = new Cell();
            var start_cell = new Cell();
            var end_cell = new Cell();
            var team_cell = new Cell();

            foreach (var cell in row.Cells)
            {
                long columnid = cell.ColumnId.Value;
                string columnName = sheet.GetColumnById(columnid).Title.ToString();
               
                switch (columnName)
                {
                    case "LOB":
                        lob_cell.ColumnId = columnid;
                        lob_cell.Value = pif.lob;
                        break;

                    case "Project":
                        project_cell.ColumnId = columnid;
                        project_cell.Value = pif.projectName;
                        break;

                    case "Status":
                        status_cell.ColumnId = columnid;
                        status_cell.Value = pif.status;
                        break;

                    case "Start":
                        start_cell.ColumnId = columnid;
                        start_cell.Value = pif.startDate;
                        break;

                    case "Ship":
                        //var end_cell = new Cell
                        //{
                        //    ColumnId = columnid,
                        //    Value = pif.endDate
                        //};
                        end_cell.ColumnId = columnid;
                        end_cell.Value = pif.endDate;
                        break;

                    //case "PM":
                    //    pm_cell.ColumnId = columnid;
                    //    pm_cell.Value = pif.producer;
                    //    break;

                    case "Team":
                        team_cell.ColumnId = columnid;
                        team_cell.Value = pif.team;
                        break;

                }
            }

            rowToTupdate = new Row
            {
                Id = pif.pif_Id,
                Cells = new Cell[] { lob_cell, project_cell, status_cell, start_cell,team_cell }
            };

            try
            {
                IList<Row> updatedRow = smartsheet_CL.SheetResources.RowResources.UpdateRows(
                sheet.Id.Value,
                new Row[] { rowToTupdate }
            );
            }
            catch (Exception e)
            {
                Console.WriteLine("Update row Failed: " + e.Message);
            };
        }
        public void runthroughallsheets()
        {
            /*
            // List all sheets
            PaginatedResult<Sheet> sheets = smartsheet_CL.SheetResources.ListSheets(
                null,               // IEnumerable<SheetInclusion> includes
                null,               // PaginationParameters
                null                // Nullable<DateTime> modifiedSince = null
            );
            //Console.WriteLine("Found " + sheets.TotalCount + " sheets");
            //iteration through all Sheet IDs
            for (int i = 0; sheets.TotalCount > i; i++) {
                ///Console.WriteLine("Loading sheet position: " + i);
                //Console.WriteLine("Loading sheet id: " + (long)sheets.Data[i].Id);
                GetSheet((long)sheets.Data[i].Id,smartsheet);
            }
            long sheetId = 1478730146178948; //Display TEST
            */
        }

        public IEnumerable<SelectListItem> Get_lobs_picklist(Column lob_col)
        {
            List<SelectListItem> options = new List<SelectListItem>();
            foreach(var lob in lob_col.Options)
            {
                options.Add(new SelectListItem { Text = lob, Value = lob });
            }
            return options;
        }
        public IEnumerable<SelectListItem> Get_status_picklist(Column status_col)
        {
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var status in status_col.Options)
            {
                options.Add(new SelectListItem { Text = status, Value = status });
            }
            return options;
        }
        public IEnumerable<SelectListItem> Get_team_picklist(Column team_col)
        {
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var team in team_col.Options)
            {
                options.Add(new SelectListItem { Text = team, Value = team });
            }
            return options;
        }
    }
}
