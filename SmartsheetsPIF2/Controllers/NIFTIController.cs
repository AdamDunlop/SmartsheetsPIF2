using Microsoft.AspNetCore.Mvc;
using SmartsheetsPIF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smartsheet.Api.Models;
using Smartsheet.Api;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace SmartsheetsPIF.Controllers
{
    public class NIFTIController : Controller
    {

        public static long sheetId = 4495384983693188;


        [HttpGet]
        public IActionResult List()
        {
            var sheet = LoadSheet(sheetId, initSheet());
            return View(GetRows(sheet));
        }


        [HttpGet]
        public IActionResult Edit(long id)
        {
            NIFTIModel model = new NIFTIModel();
            model = GetProjectEdit(id);
            return View(model);
        }

        //[HttpPost]
        //public IActionResult Edit(NIFTIModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        NIFTIModel model_lists = GetPickLists(model.pif_Id);
        //        model.lob_options = model_lists.lob_options;
        //        model.status_options = model_lists.status_options;
        //        model.SpecsList = model_lists.SpecsList;
        //        return View(model);
        //    }
        //    else
        //    {
        //        updateProject(model);
        //        return RedirectToAction("List");
        //    }
        //}
        [HttpGet]
        public IActionResult Details(long id)
        {
            NIFTIModel model = new NIFTIModel();
            model = GetProjectDetails(id);
            return View(model);
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

        public List<NIFTIModel> GetRows(Sheet sheet)
        {
            List<NIFTIModel> pif_list = new List<NIFTIModel>();

            foreach (var row in sheet.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row.Cells.ElementAt(0).DisplayValue))
                {
                    NIFTIModel pif = new NIFTIModel();
                    pif.pif_Id = (long)row.Id;
                    foreach (var cell in row.Cells)
                    {
                        long columnid = cell.ColumnId.Value;
                        string columnName = sheet.GetColumnById(columnid).Title.ToString();
                        Console.WriteLine("Column Name: " + columnName + " -- Cell Value: " + cell.DisplayValue);
                        switch (columnName)
                        {



                            case "Tenrox Code":
                                pif.tenroxCode = cell.DisplayValue;
                                break;

                            case "Project":
                                pif.projectName = cell.DisplayValue;
                                break;


                        }
                    }
                    pif_list.Add(pif);
                }
            }
            return pif_list;
        }

        public NIFTIModel GetProjectEdit(long row_id)
        {
            NIFTIModel pif = new NIFTIModel();
            pif.pif_Id = row_id;
            Sheet sheet = LoadSheet(sheetId, initSheet());



            foreach (var row in sheet.Rows)
            {
                if (row.Id == row_id)
                {
                    foreach (var cell in row.Cells)
                    {
                        long columnid = cell.ColumnId.Value;
                        string columnName = sheet.GetColumnById(columnid).Title.ToString();
                        Console.WriteLine("Column Name: " + columnName + " -- Cell Value: " + cell.DisplayValue);
                        switch (columnName)
                        {


                            case "Tenrox Code":
                                pif.tenroxCode = cell.DisplayValue;
                                break;

                            case "Project":
                                pif.projectName = cell.DisplayValue;
                                break;


                        }
                    }
                }
                return pif;
            }

            public NIFTIModel GetProjectDetails(long row_id)
            {
                NIFTIModel pif = new NIFTIModel();
                pif.pif_Id = row_id;
                Sheet sheet = LoadSheet(sheetId, initSheet());


                foreach (var row in sheet.Rows)
                {
                    if (row.Id == row_id)
                    {
                        foreach (var cell in row.Cells)
                        {
                            long columnid = cell.ColumnId.Value;
                            string columnName = sheet.GetColumnById(columnid).Title.ToString();
                            Console.WriteLine("Column Name: " + columnName + " -- Cell Value: " + cell.DisplayValue);
                            switch (columnName)
                            {


                                case "Tenrox Code":
                                    pif.tenroxCode = cell.DisplayValue;
                                    break;

                                case "Project":
                                    pif.projectName = cell.DisplayValue;
                                    break;
                            }
                        }
                    }
                    return pif;
                }

                public void updateProject(NIFTIModel pif)
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
                    var rowToTupdate = new Row();

                    var tenrox_cell = new Cell();
                    var project_cell = new Cell();


                    foreach (var cell in row.Cells)
                    {
                        long columnid = cell.ColumnId.Value;
                        string columnName = sheet.GetColumnById(columnid).Title.ToString();

                        switch (columnName)
                        {

                            case "Project":
                                project_cell.ColumnId = columnid;
                                project_cell.Value = pif.projectName;
                                break;

                            case "Tenrox Code":
                                tenrox_cell.ColumnId = columnid;
                                tenrox_cell.Value = pif.tenroxCode;
                                break;


                        }

                        rowToTupdate = new Row
                        {
                            Id = pif.pif_Id,
                            Cells = new Cell[] {
                    //lob_cell,
                    tenrox_cell,
                    //type_cell,
                    project_cell,
                    //status_cell,
                    //start_cell,
                    //end_cell,
                    //wbs_cell,
                    //deliverables_tracker_cell,
                    //final_delivery_cell,
                    //deliverables_cell,
                    //description_cell,
                    //specs_cell
                }
                        };

                        try
                        {
                            IList<Row> updatedRow = smartsheet_CL.SheetResources.RowResources.UpdateRows(
                            sheet.Id.Value,
                            new Row[] { rowToTupdate }
                        );
                            TempData["Result"] = "Success";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Project Update has Failed: " + e.Message + e.Data.ToString());
                            TempData["Result"] = "Failed";
                        };
                    }
                }
            }
        }
    }
}

                    //public NIFTIModel GetPickLists(long row_id)
                    //{
                    //    NIFTIModel pif = new NIFTIModel();
                    //    pif.pif_Id = row_id;
                    //    Sheet sheet = LoadSheet(sheetId, initSheet());

                    //    pif.lob_options = Get_lobs_picklist(sheet.GetColumnByIndex(0));

                    //    pif.status_options = Get_status_picklist(sheet.GetColumnByIndex(3));

                    //    pif.SpecsList = Get_Specs_List(sheet.GetColumnByIndex(17));

                    //    return pif;
                    //}
                    //        public void runthroughallsheets()
                    //{
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
                    //}
                    //    public ICollection<SelectListItem> Get_Specs_List(Column specs_col)
                    //    {
                    //        List<SelectListItem> list = new List<SelectListItem>();
                    //        //int cont = 0;
                    //        foreach (var spec in specs_col.Options)
                    //        {
                    //            //cont++;
                    //            //list.Add(new SelectListItem { Text = spec, Value = cont.ToString()});
                    //            list.Add(new SelectListItem { Text = spec, Value = spec });
                    //        }
                    //        return list;
                    //    }
                    //    public IEnumerable<SelectListItem> Get_lobs_picklist(Column lob_col)
                    //    {
                    //        List<SelectListItem> options = new List<SelectListItem>();
                    //        foreach (var lob in lob_col.Options)
                    //        {
                    //            options.Add(new SelectListItem { Text = lob, Value = lob });
                    //        }
                    //        return options;
                    //    }
                    //    public IEnumerable<SelectListItem> Get_status_picklist(Column status_col)
                    //    {
                    //        List<SelectListItem> options = new List<SelectListItem>();
                    //        foreach (var status in status_col.Options)
                    //        {
                    //            options.Add(new SelectListItem { Text = status, Value = status });
                    //        }
                    //        return options;
                    //    }

            //    }

            //}
