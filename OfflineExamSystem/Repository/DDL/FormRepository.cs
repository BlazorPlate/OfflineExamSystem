//using OfflineExamSystem.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using OfflineExamSystem.Helpers;
//using System.Web.Caching;
//using System.Collections;

//namespace OfflineExamSystem.DDL.Repository
//{
//    public class FormRepository
//    {
//        private CuteFormsEntities db = new CuteFormsEntities();
//        public IQueryable<Form> form { get; set; }
//        public FormRepository()
//        {
//            form = Generateform();
//        }
//        //Return only the results we want
//        public List<Form> GetForm(string searchTerm, int pageSize, int pageNum)
//        {
//            return GetFormQuery(searchTerm).DistinctBy(r => r.Name)
//                .Skip(pageSize * (pageNum - 1))
//                .Take(pageSize)
//                .ToList();
//        }

//        //And the total count of records
//        public int GetFormCount(string searchTerm, int pageSize, int pageNum)
//        {
//            return GetFormQuery(searchTerm)
//                .Count();
//        }

//        //Our search term
//        private IQueryable<Form> GetFormQuery(string searchTerm)
//        {
//            searchTerm = searchTerm.ToLower();

//            return form
//                .Where(
//                    f =>
//                    f.Name.ToLower().Contains(searchTerm) || f.Description.Contains(searchTerm)
//                ).OrderBy(f => f.Name);
//        }

//        //Generate test data
//        private IQueryable<Form> Generateform()
//        {
//            //Check cache first before regenerating test data
//            string cacheKey = "form";
//            if (HttpContext.Current.Cache[cacheKey] != null)
//            {
//                return (IQueryable<Form>)HttpContext.Current.Cache[cacheKey];
//            }
//            var form = db.Forms;
//            var result = form.AsQueryable();
//            //Cache results
//            HttpContext.Current.Cache[cacheKey] = result;

//            return result;
//        }
//        public Select2PagedResult ToSelect2Format(List<Form> form, int total)
//        {
//            Select2PagedResult json = new Select2PagedResult();
//            json.Results = new List<Select2Result>();

//            //Loop through our models and translate it into a text value and an id for the select list
//            foreach (Form f in form)
//            {
//                json.Results.Add(new Select2Result { id = f.Id.ToString(), text = f.Name });
//            }
//            //Set the total count of the results from the query.
//            json.Total = total;

//            return json;
//        }
//    }
//}